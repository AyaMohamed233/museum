# إعداد HUD UI System

## 📺 النظام الجديد: HUD Buttons

تم تغيير النظام بالكامل من Panel/Text إلى **أزرار HUD احترافية** في أسفل الشاشة.

---

## 🎨 التصميم النهائي:

```
┌──────────────────────────────────────────────────────────────────┐
│                                                                  │
│                          (Game View)                             │
│                                                                  │
│                                                                  │
│                                          [T] Talk to Ramesses    │
│                                          ↑ يظهر فقط مع Raycast  │
│  [ESC] Menu    [M] Map    [I] Information                        │
│  ← دايماً ظاهرين ───────────────────────────────────────────── →│
└──────────────────────────────────────────────────────────────────┘
```

---

## 🏗️ خطوات الإنشاء:

### 1️⃣ إنشاء Canvas

1. **Right Click** في الـ Hierarchy
2. اختار **UI → Canvas**
3. سميه `HUDCanvas`
4. **في الـ Inspector**:
   - **Render Mode**: `Screen Space - Overlay` ✅
   - **Canvas Scaler** → **UI Scale Mode**: `Scale With Screen Size`
   - **Reference Resolution**: `1920 x 1080`
   - **Match Width Or Height**: `0.5`

---

### 2️⃣ إنشاء HUDManager GameObject

1. أضف **Empty GameObject** كـ child للـ `HUDCanvas`
2. سميه `HUDManager`
3. أضف عليه سكربت **HUDManager.cs**

---

### 3️⃣ إنشاء الأزرار الثابتة (Persistent Buttons)

#### أ) أزرار أسفل يسار الشاشة:

أنشئ **3 مجموعات** كـ children للـ `HUDCanvas`:

##### [ESC] Menu Button Group:
1. **Right Click** على `HUDCanvas` → **UI → Panel** → سميه `MenuButtonGroup`
2. **Anchor**: Bottom-Left
3. أضف بداخله:
   - **UI → Image** → مربع صغير بلون رمادي/أسود شفاف → اكتب فيه "ESC"
   - **UI → Text - TextMeshPro** → اكتب "Menu"
4. **Position**: أسفل يسار الشاشة

##### [M] Map Button Group:
1. نفس الطريقة → سميه `MapButtonGroup`
2. أضف مربع بحرف "M" + نص "Map"
3. **Position**: بجانب الـ Menu button

##### [I] Information Button Group:
1. نفس الطريقة → سميه `InfoButtonGroup`
2. أضف مربع بحرف "I" + نص "Information"
3. **Position**: بجانب الـ Map button

#### 💡 نصيحة: استخدم **Horizontal Layout Group** على panel أب يحتوي على الثلاثة لترتيبهم تلقائياً.

---

### 4️⃣ إنشاء الزر الديناميكي (Talk Button)

##### [T] Talk Button Group:
1. **Right Click** على `HUDCanvas` → **UI → Panel** → سميه `TalkButtonGroup`
2. **Anchor**: Bottom-Right
3. أضف بداخله:
   - **UI → Image** → مربع بحرف "T"
   - **UI → Text - TextMeshPro** → سميه `TalkLabel` (فاضي — هيتملى تلقائي بـ "Talk to [Statue Name]")
4. **Position**: أسفل يمين الشاشة (فوق الأزرار الثابتة قليلاً)

---

### 5️⃣ إنشاء لوحات الـ Panels

#### أ) Menu Panel:
1. **Right Click** على `HUDCanvas` → **UI → Panel** → سميه `MenuPanel`
2. اجعله يغطي وسط الشاشة
3. **لون خلفي**: أسود شفاف (0, 0, 0, 200)
4. أضف بداخله:
   - **عنوان**: "Menu" (TextMeshPro, حجم 36)
   - **زر Resume**: UI → Button - TextMeshPro → سميه `ResumeButton`
   - **زر Settings**: UI → Button - TextMeshPro → سميه `SettingsButton`
   - **زر Quit**: UI → Button - TextMeshPro → سميه `QuitButton`
   - **(اختياري) Slider للصوت**: UI → Slider → سميه `VolumeSlider`

#### ب) Information Panel:
1. **Right Click** على `HUDCanvas` → **UI → Panel** → سميه `InfoPanel`
2. اجعله يظهر كنافذة في وسط أو يمين الشاشة
3. أضف بداخله:
   - **عنوان**: TextMeshPro → سميه `InfoTitleText` (هيتملى باسم التمثال)
   - **وصف**: TextMeshPro → سميه `InfoDescriptionText` (هيتملى بوصف التمثال)

#### ج) Map Panel:
1. **Right Click** على `HUDCanvas` → **UI → Panel** → سميه `MapPanel`
2. أضف بداخله:
   - **نص Placeholder**: "Museum Map - Coming Soon"
   - **(اختياري)**: Image للخريطة لو متاحة

---

### 6️⃣ ربط كل حاجة بالـ HUDManager

1. اختار `HUDManager` GameObject
2. في الـ **Inspector**، تحت **HUDManager** script:

   **Persistent Buttons:**
   - **Menu Button Group** → اسحب `MenuButtonGroup`
   - **Map Button Group** → اسحب `MapButtonGroup`
   - **Info Button Group** → اسحب `InfoButtonGroup`

   **Dynamic Buttons:**
   - **Talk Button Group** → اسحب `TalkButtonGroup`
   - **Talk Label** → اسحب `TalkLabel` (TextMeshPro)

   **Menu Panel:**
   - **Menu Panel** → اسحب `MenuPanel`
   - **Resume Button** → اسحب `ResumeButton`
   - **Settings Button** → اسحب `SettingsButton`
   - **Quit Button** → اسحب `QuitButton`
   - **Volume Slider** → اسحب `VolumeSlider` (اختياري)

   **Information Panel:**
   - **Info Panel** → اسحب `InfoPanel`
   - **Info Title Text** → اسحب `InfoTitleText`
   - **Info Description Text** → اسحب `InfoDescriptionText`

   **Map Panel:**
   - **Map Panel** → اسحب `MapPanel`

---

### 7️⃣ ربط StatueRaycastDetector بالـ HUDManager

1. اختار الكاميرا (اللي عليها **StatueRaycastDetector**)
2. في الـ **Inspector** تحت **HUD**:
   - **Hud Manager** → اسحب `HUDManager` GameObject هنا
3. **ملاحظة**: الـ StatueRaycastDetector بيعمل auto-find لو مش مربوط يدوي

---

### 8️⃣ إضافة Description لكل تمثال

1. لكل تمثال في الـ Scene، اختار الـ **StatueInfo** component
2. في الـ field الجديد **Description**:
   - اكتب وصف بالإنجليزي (مثل: "One of the greatest pharaohs of ancient Egypt, ruled for 66 years and built the famous Abu Simbel temples")

---

## 🎮 المفاتيح:

| المفتاح | الوظيفة |
|---------|---------|
| **ESC** | فتح/قفل Menu (أو قفل أي panel مفتوح) |
| **M** | فتح/قفل Map |
| **I** | فتح/قفل Information (لو واقف قدام تمثال) |
| **T** | بدء/إنهاء محادثة مع التمثال |
| **WASD** | التحرك |
| **Mouse** | النظر (Cinemachine) |

---

## ✅ النتيجة المتوقعة:

- **دايماً**: تشوف 3 أزرار في أسفل الشاشة (ESC, M, I)
- **لما تبص على تمثال**: يظهر زر [T] Talk to [اسم التمثال]
- **لما تبعد**: يختفي زر الـ Talk
- **لما تضغط T**: تبدأ محادثة صوتية
- **لما تضغط T مرة تانية**: تنتهي المحادثة
- **لما تضغط ESC**: يفتح Menu + الماوس يظهر + اللعبة تتوقف
- **لما تضغط I**: يفتح panel بتفاصيل التمثال اللي قدامك
- **لما تضغط M**: يفتح خريطة المتحف

---

## 🗑️ إزالة الـ UI القديم:

1. احذف أي `InteractionPanel` أو `StatueInteractionUI` Canvas قديم
2. احذف أي `StatueNameText` / `PromptText` / `DistanceText` قديمة
3. الـ `StatueRaycastDetector` لم يعد يحتاج الـ fields القديمة (تم حذفها من الكود)

---

كده تمام! الـ HUD هيشتغل بشكل احترافي! 🎨✨
