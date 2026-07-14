# 🎧 Gemini Voice Gateway - WebSocket Server

WebSocket Gateway للربط بين Unity و Gemini Live Audio API بدون أي تأخير.

## ⚡ المميزات

- ✅ Voice-to-Voice في الوقت الحقيقي
- ✅ بدون تجميع نصوص أو تأخير
- ✅ Audio Streaming مباشر
- ✅ دعم PCM 16-bit audio
- ✅ معالجة سريعة جدًا

## 📦 التثبيت

```bash
cd gateway
npm install
```

## ⚙️ الإعداد

1. افتح ملف `.env`
2. ضع مفتاح Gemini API الخاص بك:

```env
GEMINI_API_KEY=your_actual_api_key_here
PORT=8080
```

## 🚀 التشغيل

```bash
npm start
```

سيعمل السيرفر على: `ws://localhost:8080`

## 🔧 التكوين

يمكنك تعديل الإعدادات في `config.json`:

```json
{
  "port": 8080,
  "audio": {
    "inputSampleRate": 16000,
    "outputSampleRate": 24000,
    "channels": 1,
    "bitDepth": 16
  },
  "gemini": {
    "model": "gemini-2.5-flash-native-audio-preview-09-2025",
    "voiceName": "Zephyr",
    "systemInstruction": "..."
  }
}
```

## 📡 البروتوكول

### من Unity → Gateway:
- **Binary**: PCM audio chunks (16-bit, 16kHz)
- **Text**: JSON control messages

### من Gateway → Unity:
- **Binary**: PCM audio chunks (16-bit, 24kHz)
- **Text**: JSON status/transcription messages

### رسائل JSON:

```json
// Status
{"type": "status", "status": "connected"}

// Transcription (optional)
{"type": "transcription", "sender": "user", "text": "..."}
{"type": "transcription", "sender": "model", "text": "..."}

// Error
{"type": "error", "message": "..."}
```

## 🧪 الاختبار

يمكنك اختبار السيرفر باستخدام أي WebSocket client:

```javascript
const ws = new WebSocket('ws://localhost:8080');

ws.onopen = () => {
  console.log('Connected!');
};

ws.onmessage = (event) => {
  if (event.data instanceof Blob) {
    console.log('Received audio chunk');
  } else {
    console.log('Received message:', event.data);
  }
};
```

## 🐛 استكشاف الأخطاء

### المشكلة: "GEMINI_API_KEY not set"
**الحل**: تأكد من وضع المفتاح في `.env`

### المشكلة: "Connection refused"
**الحل**: تأكد أن السيرفر يعمل على نفس البورت المحدد في Unity

### المشكلة: صوت متقطع
**الحل**: تحقق من سرعة الإنترنت وقلل حجم الـ chunks

## 📊 الأداء

- **Latency**: ~200-500ms (حسب الإنترنت)
- **Audio Quality**: 24kHz, 16-bit PCM
- **Throughput**: ~50KB/s per connection

## 🔒 الأمان

⚠️ هذا السيرفر للتطوير فقط. للإنتاج:
- استخدم HTTPS/WSS
- أضف authentication
- أضف rate limiting
- استخدم environment variables آمنة
