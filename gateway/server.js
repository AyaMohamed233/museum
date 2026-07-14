import { WebSocketServer } from 'ws';
import http from 'http';
import { GoogleGenAI, Modality } from '@google/genai';
import dotenv from 'dotenv';
import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';

dotenv.config();

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const config = JSON.parse(fs.readFileSync('./config.json', 'utf8'));
const PORT = process.env.PORT || config.port;
const API_KEY = process.env.GEMINI_API_KEY;

// C5: Auth token (must match Unity client)
const AUTH_TOKEN = process.env.AUTH_TOKEN || 'egyptian-guide-2025';

// C4: Whitelist of valid statue IDs (prevents path traversal)
const VALID_STATUE_IDS = new Set(
  fs.readdirSync(path.join(__dirname, 'instructions'))
    .filter(f => f.endsWith('.txt'))
    .map(f => f.replace('.txt', ''))
);
console.log(`Valid statue IDs: ${[...VALID_STATUE_IDS].join(', ')}`);

if (!API_KEY || API_KEY === 'YOUR_API_KEY_HERE') {
  console.error('Error: GEMINI_API_KEY not set in .env file');
  process.exit(1);
}

// Visitor state management
const visitors = new Map();

// M3: Rate limiting — max concurrent visitors
const MAX_VISITORS = 50;

// Grace period in milliseconds
const GRACE_PERIOD_MS = 3000;

// Session creation locks to prevent race conditions
const sessionLocks = new Map();

// H8: Pending requests queue for locked sessions
const pendingRequests = new Map();

// M1: Audio logging throttle
let audioMessageCount = 0;
const AUDIO_LOG_INTERVAL = 100; // Log every 100th audio message

// Load instruction for a statue
function loadInstruction(statueId) {
  // C4: Validate against whitelist — prevents path traversal
  if (!VALID_STATUE_IDS.has(statueId)) {
    console.log(`Invalid statue ID: "${statueId}", using default`);
    // Try default
    if (VALID_STATUE_IDS.has('default')) {
      return fs.readFileSync(path.join(__dirname, 'instructions', 'default.txt'), 'utf8');
    }
    // Fallback: use first available
    const firstId = [...VALID_STATUE_IDS][0];
    if (firstId) {
      return fs.readFileSync(path.join(__dirname, 'instructions', `${firstId}.txt`), 'utf8');
    }
    return 'You are an Egyptian museum guide.';
  }

  const filePath = path.join(__dirname, 'instructions', `${statueId}.txt`);
  const instruction = fs.readFileSync(filePath, 'utf8');
  console.log(`Loaded instruction for: ${statueId}`);
  return instruction;
}

// Create Gemini session for a statue
async function createGeminiSession(visitorState, statueId, voiceName, ws) {
  try {
    const instruction = loadInstruction(statueId);
    const ai = new GoogleGenAI({ apiKey: API_KEY });

    // Use provided voice or fallback to config default
    const selectedVoice = voiceName || config.gemini.voiceName;

    console.log(`Creating Gemini session for ${statueId} with voice: ${selectedVoice}...`);

    const session = await ai.live.connect({
      model: config.gemini.model,
      callbacks: {
        onopen: () => {
          console.log(`✅ Gemini session opened for visitor ${visitorState.visitorId} -> ${statueId} (voice: ${selectedVoice})`);
          visitorState.isConnected = true;

          // H7: Check WebSocket is still open before sending
          if (ws.readyState === 1) { // WebSocket.OPEN
            ws.send(JSON.stringify({
              type: 'statue_activated',
              statueId: statueId
            }));
            console.log(`Sent statue_activated for ${statueId}`);
          }
        },

        onmessage: async (message) => {
          // Only process if this is still the active session
          if (visitorState.activeStatue !== statueId) {
            return;
          }

          const audioData = message.serverContent?.modelTurn?.parts?.[0]?.inlineData?.data;

          if (audioData) {
            // H7: Check WebSocket before sending
            if (ws.readyState === 1) {
              // Only send audio if the visitor is still actively engaged
              if (visitorState.state === 'ACTIVE') {
                const audioBuffer = Buffer.from(audioData, 'base64');
                ws.send(audioBuffer);
              }
            }
          }

          if (message.serverContent?.outputTranscription) {
            if (ws.readyState === 1) {
              ws.send(JSON.stringify({
                type: 'transcription',
                sender: 'model',
                statueId: statueId,
                text: message.serverContent.outputTranscription.text
              }));
            }
          }

          if (message.serverContent?.inputTranscription) {
            if (ws.readyState === 1) {
              ws.send(JSON.stringify({
                type: 'transcription',
                sender: 'user',
                text: message.serverContent.inputTranscription.text
              }));
            }
          }

          if (message.serverContent?.turnComplete) {
            console.log(`✅ Turn complete for ${statueId} - Statue finished speaking`);
            if (ws.readyState === 1) {
              ws.send(JSON.stringify({
                type: 'turn_complete',
                statueId: statueId
              }));
            }
          }

          if (message.serverContent?.interrupted) {
            if (ws.readyState === 1) {
              ws.send(JSON.stringify({ type: 'clear_audio' }));
            }
            console.log(`Interrupted - ${statueId}`);
          }
        },

        onclose: () => {
          console.log(`Gemini session closed for ${statueId}`);
          if (visitorState.activeStatue === statueId) {
            visitorState.isConnected = false;
          }
        },

        onerror: (error) => {
          console.error(`❌ Gemini error for ${statueId}:`, error?.message || error);
          if (ws.readyState === 1) {
            ws.send(JSON.stringify({ type: 'error', message: error?.message || 'Gemini error' }));
          }
        }
      },
      config: {
        responseModalities: [Modality.AUDIO],
        systemInstruction: instruction,
        speechConfig: {
          voiceConfig: {
            prebuiltVoiceConfig: {
              voiceName: selectedVoice
            }
          }
        }
      }
    });

    console.log(`Gemini session object created, waiting for connection...`);
    return session;
  } catch (error) {
    console.error(`❌ Failed to create Gemini session for ${statueId}:`, error?.message || error);
    if (ws.readyState === 1) {
      ws.send(JSON.stringify({ type: 'error', message: 'Failed to connect to Gemini' }));
    }
    return null;
  }
}

// Close current session for a visitor
async function closeCurrentSession(visitorState) {
  if (visitorState.graceTimer) {
    clearTimeout(visitorState.graceTimer);
    visitorState.graceTimer = null;
  }

  if (visitorState.geminiSession) {
    try {
      const session = visitorState.geminiSession;
      visitorState.geminiSession = null;

      if (typeof session.close === 'function') {
        await session.close();
      } else if (typeof session.disconnect === 'function') {
        await session.disconnect();
      }
    } catch (error) {
      console.error('Error closing Gemini session:', error.message);
    }
  }

  visitorState.activeStatue = null;
  visitorState.isConnected = false;
  visitorState.state = 'IDLE';
}

// Handle statue ENTER
async function handleStatueEnter(visitorState, statueId, voiceName, ws) {
  const visitorId = visitorState.visitorId;

  // C4: Validate statue ID against whitelist
  if (!VALID_STATUE_IDS.has(statueId)) {
    console.log(`Invalid statue ID from client: "${statueId}"`);
    if (ws.readyState === 1) {
      ws.send(JSON.stringify({ type: 'error', message: `Unknown statue: ${statueId}` }));
    }
    return;
  }

  // H8: If session creation locked, queue the request instead of dropping
  if (sessionLocks.get(visitorId)) {
    console.log(`Session creation in progress for ${visitorId}, queuing request for ${statueId}`);
    pendingRequests.set(visitorId, { statueId, voiceName });
    return;
  }

  console.log(`Visitor ${visitorId} entering ${statueId} with voice: ${voiceName || 'default'}`);

  // If already active with same statue, ignore
  if (visitorState.activeStatue === statueId && visitorState.state === 'ACTIVE') {
    console.log(`Already active with ${statueId}`);
    return;
  }

  // If in grace period for same statue, cancel grace and reactivate
  if (visitorState.activeStatue === statueId && visitorState.state === 'GRACE_PERIOD') {
    console.log(`Canceling grace period, reactivating ${statueId}`);
    clearTimeout(visitorState.graceTimer);
    visitorState.graceTimer = null;
    visitorState.state = 'ACTIVE';
    if (ws.readyState === 1) {
      ws.send(JSON.stringify({
        type: 'statue_reactivated',
        statueId: statueId
      }));
    }
    return;
  }

  // Set lock to prevent race conditions
  sessionLocks.set(visitorId, true);

  try {
    // Close any existing session (different statue)
    if (visitorState.activeStatue && visitorState.activeStatue !== statueId) {
      console.log(`Closing session with ${visitorState.activeStatue} to switch to ${statueId}`);
      if (ws.readyState === 1) {
        ws.send(JSON.stringify({
          type: 'statue_deactivated',
          statueId: visitorState.activeStatue
        }));
      }
      await closeCurrentSession(visitorState);
    }

    // Create new session
    visitorState.activeStatue = statueId;
    visitorState.state = 'ACTIVE';

    const session = await createGeminiSession(visitorState, statueId, voiceName, ws);
    if (session) {
      visitorState.geminiSession = session;
      console.log(`Session created, waiting for Gemini to be ready...`);
    } else {
      visitorState.activeStatue = null;
      visitorState.state = 'IDLE';
      if (ws.readyState === 1) {
        ws.send(JSON.stringify({
          type: 'error',
          message: `Failed to activate ${statueId}`
        }));
      }
    }
  } finally {
    // Always release lock
    sessionLocks.delete(visitorId);

    // H8: Process any pending request that was queued during lock
    const pending = pendingRequests.get(visitorId);
    if (pending) {
      pendingRequests.delete(visitorId);
      // Only process if it's a different statue than what we just activated
      if (pending.statueId !== visitorState.activeStatue) {
        console.log(`Processing queued request for ${pending.statueId}`);
        await handleStatueEnter(visitorState, pending.statueId, pending.voiceName, ws);
      }
    }
  }
}

// Handle statue EXIT
function handleStatueExit(visitorState, statueId, ws) {
  console.log(`Visitor ${visitorState.visitorId} exiting ${statueId}`);

  // Only handle if this is the active statue
  if (visitorState.activeStatue !== statueId) {
    return;
  }

  // Start grace period
  visitorState.state = 'GRACE_PERIOD';
  if (ws.readyState === 1) {
    ws.send(JSON.stringify({
      type: 'grace_period_started',
      statueId: statueId,
      duration: GRACE_PERIOD_MS
    }));
  }

  visitorState.graceTimer = setTimeout(async () => {
    // H7: Check if still in grace period AND WebSocket still open
    if (visitorState.activeStatue === statueId && visitorState.state === 'GRACE_PERIOD') {
      console.log(`Grace period ended, closing session with ${statueId}`);

      try {
        if (ws.readyState === 1) {
          ws.send(JSON.stringify({
            type: 'statue_deactivated',
            statueId: statueId
          }));
        }
        await closeCurrentSession(visitorState);
        if (ws.readyState === 1) {
          ws.send(JSON.stringify({ type: 'status', status: 'idle' }));
        }
      } catch (error) {
        console.error(`Error in grace period callback: ${error.message}`);
      }
    }
  }, GRACE_PERIOD_MS);
}

// Create an HTTP server to handle both health checks (for Render anti-sleep) and WebSockets
const server = http.createServer((req, res) => {
  if (req.url === '/health') {
    res.writeHead(200, { 'Content-Type': 'text/plain' });
    res.end('OK');
  } else {
    res.writeHead(404);
    res.end();
  }
});

const wss = new WebSocketServer({ server });

console.log(`Audio Config: ${config.audio.inputSampleRate}Hz input, ${config.audio.outputSampleRate}Hz output`);
console.log(`Gemini Model: ${config.gemini.model}`);

wss.on('connection', async (ws, req) => {
  // C5: Verify auth token
  const clientToken = req.headers['x-auth-token'];
  if (AUTH_TOKEN && clientToken !== AUTH_TOKEN) {
    console.log(`❌ Unauthorized connection attempt (token: ${clientToken ? 'invalid' : 'missing'})`);
    ws.close(4001, 'Unauthorized');
    return;
  }

  // M3: Rate limit — max visitors
  if (visitors.size >= MAX_VISITORS) {
    console.log(`❌ Max visitors reached (${MAX_VISITORS}), rejecting connection`);
    ws.close(4002, 'Server full');
    return;
  }

  // Generate visitor ID
  const visitorId = `visitor_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;

  console.log(`New visitor connected: ${visitorId}`);

  // Initialize visitor state
  const visitorState = {
    visitorId: visitorId,
    activeStatue: null,
    geminiSession: null,
    state: 'IDLE',
    isConnected: false,
    graceTimer: null
  };

  visitors.set(visitorId, visitorState);

  // Send visitor ID to client
  ws.send(JSON.stringify({
    type: 'visitor_registered',
    visitorId: visitorId
  }));

  // Handle incoming messages
  ws.on('message', async (data, isBinary) => {
    try {
      // M2: Use the isBinary flag directly instead of trying JSON.parse on audio
      if (isBinary) {
        // Binary audio data — no JSON parse attempt needed
        if (!visitorState.isConnected || !visitorState.geminiSession || visitorState.state !== 'ACTIVE') {
          return;
        }

        // M1: Throttled audio logging
        audioMessageCount++;
        if (audioMessageCount % AUDIO_LOG_INTERVAL === 0) {
          console.log(`📤 Audio: ${data.length} bytes (total chunks: ${audioMessageCount})`);
        }

        const base64Audio = data.toString('base64');
        await visitorState.geminiSession.sendRealtimeInput({
          media: {
            data: base64Audio,
            mimeType: `audio/pcm;rate=${config.audio.inputSampleRate}`
          }
        });
        return;
      }

      // Text message — JSON control message
      const messageStr = data.toString();
      const message = JSON.parse(messageStr);
      console.log(`Received from ${visitorId}: ${message.action} ${message.statueId || ''}`);

      switch (message.action) {
        case 'ENTER':
          console.log(`Processing ENTER for ${message.statueId}`);
          await handleStatueEnter(visitorState, message.statueId.trim(), message.voiceName, ws);
          break;

        case 'EXIT':
          console.log(`Processing EXIT for ${message.statueId}`);
          handleStatueExit(visitorState, message.statueId.trim(), ws);
          break;

        case 'ping':
          ws.send(JSON.stringify({ type: 'pong' }));
          break;

        default:
          console.log(`Unknown action: ${message.action}`);
      }
    } catch (error) {
      console.error('Error processing message:', error.message);
    }
  });

  ws.on('close', async () => {
    console.log(`Visitor disconnected: ${visitorId}`);
    await closeCurrentSession(visitorState);
    visitors.delete(visitorId);
    pendingRequests.delete(visitorId);
    sessionLocks.delete(visitorId);
  });

  ws.on('error', (error) => {
    console.error(`WebSocket error for ${visitorId}:`, error.message);
  });
});

// Graceful shutdown
process.on('SIGINT', async () => {
  console.log('\nShutting down gateway...');

  // Close all visitor sessions
  const closePromises = [];
  for (const [visitorId, state] of visitors) {
    closePromises.push(closeCurrentSession(state));
  }
  await Promise.all(closePromises);

  wss.close(() => {
    server.close(() => {
      console.log('Gateway closed');
      process.exit(0);
    });
  });
});

// Start listening on the port
server.listen(PORT, () => {
  console.log(`Gateway (HTTP + WebSocket) running on port ${PORT}`);
});
