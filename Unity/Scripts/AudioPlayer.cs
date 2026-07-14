using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private int sampleRate = 24000;
    [SerializeField] private int bufferSize = 4096; // Increased for smoother playback (170ms)
    [SerializeField] private float maxBufferDuration = 20f; // Increased to 20 seconds for more room
    [SerializeField] private float minBufferBeforePlay = 0.5f; // Wait for 0.5s of audio before starting
    
    [Header("Buffer Management")]
    [SerializeField] private bool enableAutoBufferClear = false; // Disabled by default to prevent audio loss
    private float aggressiveClearThreshold = 0.98f; // Clear only when reaching 98%
    private float clearPercentage = 0.2f; // Clear 20% when threshold reached
    
    [Header("Status")]
    [SerializeField] private bool isPlaying = false;
    [SerializeField] private int queuedSamples = 0;
    [SerializeField] private float bufferLevel = 0f; // Current buffer level (0-1)
    [SerializeField] private bool isSpeaking = false; // Is statue currently speaking?
    
    private AudioSource audioSource;
    private Queue<float> audioQueue = new Queue<float>();
    private object queueLock = new object();
    private AudioClip streamClip;
    private bool isInitialized = false;
    
    // Voice Activity Detection
    private float lastAudioTime = 0f;
    private float silenceThreshold = 1.5f; // 1.5 seconds of silence = finished speaking
    private float audioActivityThreshold = 0.01f; // Minimum volume to consider as speech
    
    // Events
    public event Action OnStartedSpeaking;
    public event Action OnFinishedSpeaking;
    
    public bool IsPlaying => isPlaying;
    public int QueuedSamples => queuedSamples;
    public bool IsSpeaking => isSpeaking;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        InitializeAudioStream();
    }

    private void InitializeAudioStream()
    {
        // Create streaming audio clip with larger buffer for smoother playback
        int clipSamples = sampleRate / 2; // 0.5 seconds buffer for smooth playback
        streamClip = AudioClip.Create("StreamClip", clipSamples, 1, sampleRate, true, OnAudioRead);
        
        audioSource.clip = streamClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        
        isInitialized = true;
        Debug.Log($"🔊 Audio player initialized: {sampleRate}Hz, {clipSamples} samples buffer");
    }

    public void AddAudioData(byte[] pcmData)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("Audio player not initialized");
            return;
        }

        // Convert PCM16 to float32
        float[] samples = ConvertPCM16ToFloat(pcmData);
        
        // Detect if there's actual audio (Voice Activity Detection)
        bool hasAudio = DetectAudioActivity(samples);
        
        if (hasAudio)
        {
            lastAudioTime = Time.time;
            
            // If we just started speaking, notify
            if (!isSpeaking)
            {
                isSpeaking = true;
                Debug.Log("🗣️ Statue started speaking - Muting microphone");
                OnStartedSpeaking?.Invoke();
            }
        }
        
        lock (queueLock)
        {
            int maxQueueSize = (int)(sampleRate * maxBufferDuration);
            
            // NEVER clear buffer during playback - only prevent overflow
            if (audioQueue.Count + samples.Length > maxQueueSize)
            {
                // Drop NEW samples instead of clearing old ones
                int availableSpace = maxQueueSize - audioQueue.Count;
                if (availableSpace > 0)
                {
                    // Add only what fits
                    for (int i = 0; i < Mathf.Min(availableSpace, samples.Length); i++)
                    {
                        audioQueue.Enqueue(samples[i]);
                    }
                    Debug.LogWarning($"⚠️ Buffer full - dropped {samples.Length - availableSpace} samples");
                }
                else
                {
                    Debug.LogWarning($"⚠️ Buffer completely full - dropped entire chunk");
                }
            }
            else
            {
                // Normal case: add all samples
                foreach (float sample in samples)
                {
                    audioQueue.Enqueue(sample);
                }
            }
            
            queuedSamples = audioQueue.Count;
        }

        // Start playback if not playing and we have enough data
        float minSamplesBeforePlay = sampleRate * minBufferBeforePlay;
        if (!isPlaying && audioQueue.Count >= minSamplesBeforePlay)
        {
            StartPlayback();
        }
    }
    
    private bool DetectAudioActivity(float[] samples)
    {
        // Calculate RMS (Root Mean Square) to detect audio activity
        float sum = 0f;
        foreach (float sample in samples)
        {
            sum += sample * sample;
        }
        float rms = Mathf.Sqrt(sum / samples.Length);
        
        return rms > audioActivityThreshold;
    }

    private void StartPlayback()
    {
        if (isPlaying)
            return;

        audioSource.Play();
        isPlaying = true;
        Debug.Log("▶️ Audio playback started");
    }

    public void StopPlayback()
    {
        if (!isPlaying)
            return;

        audioSource.Stop();
        isPlaying = false;
        
        lock (queueLock)
        {
            audioQueue.Clear();
            queuedSamples = 0;
        }
        
        Debug.Log("⏹️ Audio playback stopped");
    }

    private void OnAudioRead(float[] data)
    {
        lock (queueLock)
        {
            int samplesNeeded = data.Length;
            int samplesAvailable = audioQueue.Count;
            
            if (samplesAvailable >= samplesNeeded)
            {
                // Normal case: enough samples available
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = audioQueue.Dequeue();
                }
            }
            else if (samplesAvailable > 0)
            {
                // Buffer underrun: fill what we can, then silence
                int i = 0;
                while (audioQueue.Count > 0 && i < data.Length)
                {
                    data[i] = audioQueue.Dequeue();
                    i++;
                }
                // Fill rest with silence
                while (i < data.Length)
                {
                    data[i] = 0f;
                    i++;
                }
                Debug.LogWarning($"⚠️ Buffer underrun: had {samplesAvailable}/{samplesNeeded} samples");
            }
            else
            {
                // No samples at all - complete silence
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = 0f;
                }
            }
            
            queuedSamples = audioQueue.Count;
        }
    }

    private float[] ConvertPCM16ToFloat(byte[] pcmData)
    {
        int sampleCount = pcmData.Length / 2;
        float[] samples = new float[sampleCount];
        
        for (int i = 0; i < sampleCount; i++)
        {
            // Little-endian 16-bit PCM
            short pcmSample = (short)(pcmData[i * 2] | (pcmData[i * 2 + 1] << 8));
            
            // Convert to float [-1, 1]
            samples[i] = pcmSample / 32768f;
        }
        
        return samples;
    }

    public void ClearBuffer()
    {
        lock (queueLock)
        {
            audioQueue.Clear();
            queuedSamples = 0;
        }
        
        // Stop current playback
        if (isPlaying && audioSource != null)
        {
            audioSource.Stop();
            isPlaying = false;
        }
        
        // Reset speaking state when buffer is cleared (interruption)
        if (isSpeaking)
        {
            isSpeaking = false;
            Debug.Log("🤐 Speaking state reset (interrupted)");
        }
        
        Debug.Log("🗑️ Audio buffer cleared and playback stopped");
    }
    
    public void PrepareForNewResponse()
    {
        // Don't clear immediately - let current audio finish playing
        // The buffer will be managed by Update() automatically
        Debug.Log("🔄 Ready for new response (current audio will finish first)");
    }
    
    public void ResetSpeakingState()
    {
        // Called when turn is complete from Gemini
        if (isSpeaking)
        {
            isSpeaking = false;
            Debug.Log("🤐 Speaking state reset (turn complete)");
        }
    }

    private void Update()
    {
        // Update buffer level for monitoring
        if (isInitialized)
        {
            int maxQueueSize = (int)(sampleRate * maxBufferDuration);
            bufferLevel = (float)audioQueue.Count / maxQueueSize;
            
            // Monitor buffer health
            if (isPlaying && bufferLevel < 0.1f)
            {
                Debug.LogWarning($"⚠️ Low buffer: {bufferLevel * 100:F0}% - may cause stuttering");
            }
        }
        
        // NOTE: Silence detection is DISABLED
        // We now rely on Gemini's turnComplete signal instead
        // This prevents premature unmuting when audio chunks have gaps between them
        
        // Keep isSpeaking flag active while audio is being received
        // It will be reset by turn_complete message from server
    }


    private void OnDestroy()
    {
        StopPlayback();
    }
}
