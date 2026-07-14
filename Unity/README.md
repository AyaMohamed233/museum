# 🎮 Unity Voice Integration - المرشد المصري

تكامل Unity مع Gemini Live Audio API عبر WebSocket Gateway.

## 📋 المتطلبات

- Unity 2021.3 أو أحدث
- .NET 4.x أو .NET Standard 2.1
- WebSocket support (مدمج في Unity)
- Microphone permissions

## 📦 التثبيت

1. انسخ مجلد `Scripts` إلى مشروع Unity الخاص بك
2. تأكد من تفعيل Microphone permissions في Player Settings

### Windows:
- Player Settings → Other Settings → Microphone Usage Description

### Android:
- Player Settings → Android → Permissions → Microphone

### iOS:
- Player Settings → iOS → Microphone Usage Description

## 🎯 الإعداد السريع

### الطريقة 1: استخدام VoiceController (موصى به)

1. أنشئ GameObject فارغ في المشهد
2. أضف `VoiceController` script
3. اضبط WebSocket URL في Inspector: `ws://localhost:8080`
4. شغّل المشهد واضغط Connect

### الطريقة 2: Setup يدوي

1. أنشئ 3 GameObjects:
   - `WebSocketManager` → أضف `WebSocketClient.cs`
   - `MicrophoneManager` → أضف `MicrophoneCapture.cs`
   - `AudioManager` → أضف `AudioPlayer.cs` + `AudioSource`

2. اربط المكونات في الكود أو عبر Inspector

## 🎨 إضافة UI (اختياري)

```csharp
// في VoiceController Inspector:
- Connect Button → Button component
- Disconnect Button → Button component
- Status Text → TextMeshProUGUI
- Transcription Text → TextMeshProUGUI
- Mic Indicator → Image
```

## ⚙️ الإعدادات

### WebSocketClient
- **Server URL**: عنوان Gateway (default: `ws://localhost:8080`)

### MicrophoneCapture
- **Sample Rate**: 16000 Hz (لا تغيّر)
- **Chunk Size**: 4096 samples (يمكن تعديله للأداء)
- **Device Name**: اتركه فارغ للميكروفون الافتراضي

### AudioPlayer
- **Sample Rate**: 24000 Hz (لا تغيّر)
- **Buffer Size**: 4096 samples
- **Max Buffer Duration**: 2 seconds

## 🚀 الاستخدام

### من الكود:

```csharp
// الحصول على المكونات
var voiceController = GetComponent<VoiceController>();

// الاتصال
await voiceController.Connect();

// قطع الاتصال
await voiceController.Disconnect();

// الاستماع للأحداث
webSocketClient.OnConnected += () => Debug.Log("Connected!");
webSocketClient.OnAudioReceived += (audio) => Debug.Log("Audio received");
webSocketClient.OnTranscriptionReceived += (sender, text) => 
    Debug.Log($"{sender}: {text}");
```

### من UI:
- اضغط "Connect" للاتصال
- تكلم في الميكروفون
- استمع للرد الصوتي
- اضغط "Disconnect" لقطع الاتصال

## 🔊 تدفق الصوت

```
Microphone → MicrophoneCapture → WebSocketClient → Gateway
                                                      ↓
                                                   Gemini API
                                                      ↓
AudioPlayer ← WebSocketClient ← Gateway ← Audio Response
```

## 🎯 الأداء

### نصائح لتقليل Latency:

1. **Chunk Size**: قلل إلى 2048 للاستجابة الأسرع
2. **Buffer**: قلل Max Buffer Duration إلى 1 second
3. **Network**: استخدم اتصال سلكي بدلاً من WiFi
4. **Gateway**: شغّل Gateway على نفس الجهاز

### الأداء المتوقع:
- **Latency**: 300-700ms (حسب الإنترنت)
- **CPU Usage**: منخفض (~5%)
- **Memory**: ~50MB

## 🐛 استكشاف الأخطاء

### "No microphone detected"
**الحل**: 
- تحقق من توصيل الميكروفون
- تحقق من Permissions في Player Settings

### "Connection failed"
**الحل**:
- تأكد أن Gateway يعمل
- تحقق من عنوان WebSocket URL
- تحقق من Firewall settings

### صوت متقطع أو بطيء
**الحل**:
- قلل Chunk Size
- تحقق من سرعة الإنترنت
- أغلق التطبيقات الأخرى

### لا يوجد صوت
**الحل**:
- تحقق من AudioSource volume
- تحقق من Audio Mixer settings
- تحقق من System volume

## 📱 Build Settings

### Windows:
```
- Architecture: x86_64
- Scripting Backend: IL2CPP أو Mono
```

### Android:
```
- Minimum API Level: 24 (Android 7.0)
- Target API Level: 33+
- Internet Access: Required
- Microphone Permission: Required
```

### iOS:
```
- Target iOS Version: 12.0+
- Microphone Usage Description: مطلوب
- Background Modes: Audio (إذا أردت العمل في الخلفية)
```

## 🔧 التخصيص

### تغيير System Instruction:
عدّل في `gateway/config.json`:

```json
{
  "gemini": {
    "systemInstruction": "أنت مساعد صوتي..."
  }
}
```

### تغيير الصوت:
```json
{
  "gemini": {
    "voiceName": "Zephyr" // أو Puck, Charon, Kore, Fenrir, Aoede
  }
}
```

## 📊 مراقبة الأداء

```csharp
// في Update() أو UI
Debug.Log($"Queued Audio: {audioPlayer.QueuedSamples} samples");
Debug.Log($"Recording: {microphoneCapture.IsRecording}");
Debug.Log($"Connected: {webSocketClient.IsConnected}");
```

## 🎓 أمثلة متقدمة

### إضافة Voice Activity Detection:

```csharp
// في MicrophoneCapture
private bool DetectVoice(float[] samples)
{
    float sum = 0;
    foreach (float s in samples)
        sum += Mathf.Abs(s);
    
    float average = sum / samples.Length;
    return average > 0.01f; // threshold
}
```

### إضافة Audio Visualization:

```csharp
// في AudioPlayer
public float GetCurrentVolume()
{
    lock (queueLock)
    {
        if (audioQueue.Count == 0) return 0;
        
        float sum = 0;
        foreach (float sample in audioQueue)
            sum += Mathf.Abs(sample);
        
        return sum / audioQueue.Count;
    }
}
```

## 📞 الدعم

للمشاكل أو الأسئلة:
1. تحقق من Console logs في Unity
2. تحقق من Gateway logs
3. راجع هذا الدليل
