# 🎮 دليل تركيب نظام الـ HUD — خطوة بخطوة

> هذا الدليل يشرح كيفية إنشاء وتركيب نظام الـ HUD الجديد بالكامل في Unity.
> اتبع الخطوات بالترتيب من البداية للنهاية.

---

## 📋 قبل البدء

### تأكد من الآتي:

- [ ] نسخت ملفات الـ Scripts الجديدة إلى `Assets/Scripts/`:
  - `HUDManager.cs` ← (جديد)
  - `StatueRaycastDetector.cs` ← (معدّل)
  - `StatueInfo.cs` ← (معدّل)
  - `PlayerController.cs` ← (معدّل)
  - `AudioPlayer.cs` ← (بدون تغيير)
  - `MicrophoneCapture.cs` ← (بدون تغيير)
  - `MuseumManager.cs` ← (بدون تغيير)
- [ ] Unity مش بيظهر أي Compile Errors (تحقق من Console)
- [ ] حذفت أي UI قديم (InteractionPanel / StatueInteractionUI) لو موجود

---

## الخطوة 1: إنشاء الـ Canvas الرئيسي

1. في الـ **Hierarchy** → **Right Click** → **UI** → **Canvas**
2. سمّيه: `HUDCanvas`
3. في الـ **Inspector** اضبط الإعدادات التالية:

| الخاصية         | القيمة                               |
| --------------- | ------------------------------------ |
| **Render Mode** | `Screen Space - Overlay`             |
| **Sort Order**  | `100` (عشان يكون فوق أي Canvas تاني) |

4. على الـ **Canvas Scaler** component (موجود تلقائي على الـ Canvas):

| الخاصية                   | القيمة                   |
| ------------------------- | ------------------------ |
| **UI Scale Mode**         | `Scale With Screen Size` |
| **Reference Resolution**  | `1920 x 1080`            |
| **Match Width Or Height** | `0.5`                    |

---

## الخطوة 2: إنشاء حاوية الأزرار الثابتة (Persistent Buttons Container)

1. **Right Click** على `HUDCanvas` → **Create Empty** → سمّيه `PersistentButtons`
2. في الـ **Rect Transform**:

| الخاصية    | القيمة                                                  |
| ---------- | ------------------------------------------------------- |
| **Anchor** | Bottom-Left (اضغط على مربع الـ Anchor واختار أسفل يسار) |
| **Pivot**  | `(0, 0)`                                                |
| **Pos X**  | `30`                                                    |
| **Pos Y**  | `30`                                                    |
| **Width**  | `600`                                                   |
| **Height** | `50`                                                    |

3. أضف component: **Horizontal Layout Group**

| الخاصية                       | القيمة           |
| ----------------------------- | ---------------- |
| **Spacing**                   | `20`             |
| **Child Alignment**           | `Middle Left`    |
| **Child Force Expand Width**  | ❌ (أزل التفعيل) |
| **Child Force Expand Height** | ❌ (أزل التفعيل) |

---

## الخطوة 3: إنشاء زر [ESC] Menu

1. **Right Click** على `PersistentButtons` → **UI** → **Panel** → سمّيه `MenuButtonGroup`
2. في الـ **Rect Transform**:

| الخاصية    | القيمة |
| ---------- | ------ |
| **Width**  | `150`  |
| **Height** | `45`   |

3. على الـ **Image** component (الموجود تلقائي على الـ Panel):
   - **Color**: `(30, 30, 30, 180)` — أسود شبه شفاف

4. أضف component: **Horizontal Layout Group**

| الخاصية                      | القيمة                  |
| ---------------------------- | ----------------------- |
| **Spacing**                  | `8`                     |
| **Padding**                  | Left: `10`, Right: `10` |
| **Child Alignment**          | `Middle Left`           |
| **Child Force Expand Width** | ❌                      |

5. بداخل `MenuButtonGroup`:

#### أ) مربع المفتاح:

- **Right Click** → **UI** → **Image** → سمّيه `KeyBadge`
- **Width**: `40` | **Height**: `35`
- **Color**: `(60, 60, 60, 220)`
- بداخل `KeyBadge`:
  - **Right Click** → **UI** → **Text - TextMeshPro** → سمّيه `KeyText`
  - **Text**: `ESC`
  - **Font Size**: `14`
  - **Alignment**: Center + Middle
  - **Color**: أبيض `(255, 255, 255)`
  - **Font Style**: Bold

#### ب) نص الوظيفة:

- **Right Click** على `MenuButtonGroup` → **UI** → **Text - TextMeshPro** → سمّيه `Label`
- **Text**: `Menu`
- **Font Size**: `18`
- **Alignment**: Left + Middle
- **Color**: أبيض `(255, 255, 255)`

---

## الخطوة 4: إنشاء زر [M] Map

1. **Right Click** على `PersistentButtons` → **UI** → **Panel** → سمّيه `MapButtonGroup`
2. **نفس إعدادات الخطوة 3 بالظبط** (Width: 130, Height: 45, نفس الألوان)
3. بداخله:
   - `KeyBadge` → `KeyText`: **`M`** (Font Size: `18`)
   - `Label`: **`Map`** (Font Size: `18`)

---

## الخطوة 5: إنشاء زر [I] Information

1. **Right Click** على `PersistentButtons` → **UI** → **Panel** → سمّيه `InfoButtonGroup`
2. **نفس إعدادات الخطوة 3 بالظبط** (Width: 190, Height: 45, نفس الألوان)
3. بداخله:
   - `KeyBadge` → `KeyText`: **`I`** (Font Size: `18`)
   - `Label`: **`Information`** (Font Size: `18`)

---

## الخطوة 6: إنشاء زر [T] Talk (الزر الديناميكي)

> هذا الزر لن يكون داخل `PersistentButtons` — سيكون منفصل في أسفل يمين الشاشة.

1. **Right Click** على `HUDCanvas` → **UI** → **Panel** → سمّيه `TalkButtonGroup`
2. في الـ **Rect Transform**:

| الخاصية    | القيمة                           |
| ---------- | -------------------------------- |
| **Anchor** | Bottom-Right (أسفل يمين)         |
| **Pivot**  | `(1, 0)`                         |
| **Pos X**  | `-30`                            |
| **Pos Y**  | `80` (فوق الأزرار الثابتة بشوية) |
| **Width**  | `300`                            |
| **Height** | `45`                             |

3. على الـ **Image** component:
   - **Color**: `(30, 30, 30, 180)`

4. أضف component: **Horizontal Layout Group**

| الخاصية                      | القيمة                  |
| ---------------------------- | ----------------------- |
| **Spacing**                  | `8`                     |
| **Padding**                  | Left: `10`, Right: `10` |
| **Child Alignment**          | `Middle Left`           |
| **Child Force Expand Width** | ❌                      |

5. بداخل `TalkButtonGroup`:

#### أ) مربع المفتاح:

- **Right Click** → **UI** → **Image** → سمّيه `KeyBadge`
- **Width**: `35` | **Height**: `35`
- **Color**: `(60, 60, 60, 220)`
- بداخله: **Text - TextMeshPro** → `KeyText` → Text: **`T`** | Font Size: `18` | Bold | أبيض | Center

#### ب) نص الوظيفة (ديناميكي):

- **Right Click** على `TalkButtonGroup` → **UI** → **Text - TextMeshPro**
- ⚠️ **سمّيه `TalkLabel`** ← (هذا الاسم مهم — هنربطه بالـ HUDManager)
- **Text**: `Talk to Statue` (placeholder — هيتغير تلقائي في الكود)
- **Font Size**: `18`
- **Alignment**: Left + Middle
- **Color**: أبيض
- **Rect Transform Width**: `240`

---

## الخطوة 7: إنشاء Menu Panel

1. **Right Click** على `HUDCanvas` → **UI** → **Panel** → سمّيه `MenuPanel`
2. في الـ **Rect Transform**:

| الخاصية                      | القيمة                           |
| ---------------------------- | -------------------------------- | ----------------- |
| **Anchor**                   | Stretch-Stretch (يغطي كل الشاشة) |
| أو **Anchor**: Middle-Center | **Width**: `450`                 | **Height**: `400` |

3. على الـ **Image** component:
   - **Color**: `(0, 0, 0, 200)` — أسود شفاف

4. بداخل `MenuPanel` أنشئ **Vertical Layout Group**:
   - **Spacing**: `15`
   - **Padding**: الكل `30`
   - **Child Alignment**: `Upper Center`

5. بداخله أنشئ العناصر التالية بالترتيب:

#### أ) العنوان:

- **UI** → **Text - TextMeshPro** → Text: **`Menu`**
- Font Size: `36` | Bold | أبيض | Center
- Height: `60`

#### ب) خط فاصل (اختياري):

- **UI** → **Image** → Height: `2` | Color: `(100, 100, 100)`

#### ج) زر Resume:

- **UI** → **Button - TextMeshPro** → سمّيه **`ResumeButton`**
- غيّر النص الداخلي لـ: **`Resume`**
- Font Size: `22` | Width: `250` | Height: `50`
- Button Color (Normal): `(50, 50, 50)` | Highlighted: `(80, 80, 80)`

#### د) زر Settings:

- **UI** → **Button - TextMeshPro** → سمّيه **`SettingsButton`**
- غيّر النص لـ: **`Settings`**
- نفس الإعدادات

#### هـ) شريط الصوت (اختياري):

- **UI** → **Slider** → سمّيه **`VolumeSlider`**
- **Value**: `1`
- **Min Value**: `0` | **Max Value**: `1`
- Width: `250`

#### و) زر Quit:

- **UI** → **Button - TextMeshPro** → سمّيه **`QuitButton`**
- غيّر النص لـ: **`Quit`**
- Button Color (Normal): `(120, 30, 30)` — أحمر غامق

---

## الخطوة 8: إنشاء Information Panel

1. **Right Click** على `HUDCanvas` → **UI** → **Panel** → سمّيه `InfoPanel`
2. في الـ **Rect Transform**:

| الخاصية    | القيمة        |
| ---------- | ------------- |
| **Anchor** | Middle-Center |
| **Width**  | `500`         |
| **Height** | `350`         |

3. على الـ **Image** component:
   - **Color**: `(20, 20, 20, 220)` — أسود غامق شفاف

4. بداخل `InfoPanel` أنشئ **Vertical Layout Group**:
   - **Spacing**: `15`
   - **Padding**: الكل `25`
   - **Child Alignment**: `Upper Center`

5. بداخله:

#### أ) عنوان التمثال:

- **UI** → **Text - TextMeshPro** → سمّيه **`InfoTitleText`**
- **Text**: (فاضي — هيتملى تلقائي)
- Font Size: `30` | Bold | لون ذهبي `(255, 200, 50)` | Center
- Height: `50`

#### ب) خط فاصل:

- **UI** → **Image** → Height: `2` | Color: `(100, 100, 100)`

#### ج) وصف التمثال:

- **UI** → **Text - TextMeshPro** → سمّيه **`InfoDescriptionText`**
- **Text**: (فاضي — هيتملى تلقائي)
- Font Size: `20` | أبيض | Left Alignment
- **Text Wrapping**: Enabled ✅
- **Overflow**: Overflow أو Scroll
- Height: `200`

#### د) تعليمة الإغلاق (اختياري):

- **UI** → **Text - TextMeshPro** → Text: **`Press ESC or I to close`**
- Font Size: `14` | رمادي `(150, 150, 150)` | Center

---

## الخطوة 9: إنشاء Map Panel

1. **Right Click** على `HUDCanvas` → **UI** → **Panel** → سمّيه `MapPanel`
2. في الـ **Rect Transform**:

| الخاصية    | القيمة        |
| ---------- | ------------- |
| **Anchor** | Middle-Center |
| **Width**  | `700`         |
| **Height** | `500`         |

3. على الـ **Image** component:
   - **Color**: `(20, 20, 20, 220)`

4. بداخله:
   - **UI** → **Text - TextMeshPro** → Text: **`Museum Map`**
   - Font Size: `30` | Bold | أبيض | Center
   - **UI** → **Text - TextMeshPro** → Text: **`Coming Soon...`**
   - Font Size: `20` | رمادي | Center

   - **(اختياري)**: **UI** → **Raw Image** → لو عندك صورة للخريطة اسحبها هنا

   - **UI** → **Text - TextMeshPro** → Text: **`Press ESC or M to close`**
   - Font Size: `14` | رمادي | Center

---

## الخطوة 10: إضافة سكربت HUDManager

1. **Right Click** على `HUDCanvas` → **Create Empty** → سمّيه `HUDManager`
   - أو ممكن تضيفه على الـ `HUDCanvas` نفسه
2. أضف component: **HUDManager** (من Add Component → اكتب HUDManager)

---

## الخطوة 11: ربط كل العناصر بالـ HUDManager ⚡

> هذه أهم خطوة — اسحب كل عنصر من الـ Hierarchy للمكان المخصص في الـ Inspector.

اختار الـ `HUDManager` GameObject وفي الـ Inspector:

### Persistent Buttons (Always Visible):

| الـ Field             | اسحب هذا العنصر   |
| --------------------- | ----------------- |
| **Menu Button Group** | `MenuButtonGroup` |
| **Map Button Group**  | `MapButtonGroup`  |
| **Info Button Group** | `InfoButtonGroup` |

### Dynamic Buttons (Raycast Dependent):

| الـ Field             | اسحب هذا العنصر                                        |
| --------------------- | ------------------------------------------------------ |
| **Talk Button Group** | `TalkButtonGroup`                                      |
| **Talk Label**        | `TalkLabel` (الـ TextMeshPro اللي جوا TalkButtonGroup) |

### Menu Panel:

| الـ Field           | اسحب هذا العنصر          |
| ------------------- | ------------------------ |
| **Menu Panel**      | `MenuPanel`              |
| **Resume Button**   | `ResumeButton`           |
| **Settings Button** | `SettingsButton`         |
| **Quit Button**     | `QuitButton`             |
| **Volume Slider**   | `VolumeSlider` (اختياري) |

### Information Panel:

| الـ Field                 | اسحب هذا العنصر       |
| ------------------------- | --------------------- |
| **Info Panel**            | `InfoPanel`           |
| **Info Title Text**       | `InfoTitleText`       |
| **Info Description Text** | `InfoDescriptionText` |

### Map Panel:

| الـ Field     | اسحب هذا العنصر |
| ------------- | --------------- |
| **Map Panel** | `MapPanel`      |

---

## الخطوة 12: ربط StatueRaycastDetector بالـ HUDManager

1. اختار الـ **Camera** (أو الـ GameObject اللي عليه `StatueRaycastDetector`)
2. في الـ Inspector تحت **HUD**:
   - **Hud Manager** → اسحب `HUDManager` GameObject هنا
3. ⚠️ تأكد أن **Interaction Key** = `T` (المفروض يكون كده تلقائي)

> 💡 ملاحظة: لو مربطتش الـ HUDManager يدوي، الكود هيعمل auto-find — لكن الأفضل تربطه يدوي.

---

## الخطوة 13: إضافة Description لكل تمثال

لكل تمثال في الـ Scene:

1. اختار الـ **GameObject** بتاع التمثال
2. في الـ **StatueInfo** component، هتلاقي field جديد: **Description**
3. اكتب وصف **بالإنجليزي**، مثلاً:

| التمثال          | الوصف                                                                                                                   |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------- |
| Ramesses II      | `One of the greatest pharaohs of ancient Egypt. Ruled for 66 years and built the famous Abu Simbel temples.`            |
| Tutankhamun Mask | `The iconic golden funerary mask of the young pharaoh Tutankhamun, made of solid gold and weighing 11 kg.`              |
| Cleopatra        | `The last active ruler of the Ptolemaic Kingdom of Egypt. Known for her intelligence, beauty, and political alliances.` |
| Sekhmet          | `The lioness-headed goddess of war and healing. Daughter of Ra, feared for her fierce nature.`                          |
| Anubis           | `The jackal-headed god of mummification and the afterlife. Guide of souls to the underworld.`                           |

> 💡 يمكنك نسخ الأوصاف من ملفات JSON الموجودة في `StatueConfigs/` (حقل `description`).

---

## الخطوة 14: حذف الـ UI القديم

1. **ابحث في الـ Hierarchy** عن أي من هذه العناصر واحذفها:
   - [ ] `InteractionPanel`
   - [ ] `StatueInteractionUI`
   - [ ] `StatueNameText` (القديم)
   - [ ] `PromptText` (القديم)
   - [ ] `DistanceText` (القديم)
   - [ ] أي Canvas قديم كان مخصص للتفاعل مع التماثيل

2. **في الـ StatueRaycastDetector** Inspector:
   - تأكد أن الـ fields القديمة (Screen UI, Statue Name Text, إلخ) **مش موجودة** (الكود الجديد حذفها تلقائي)

---

## الخطوة 15: اختبار النظام ✅

### اضغط ▶️ Play واختبر الآتي:

#### اختبار الأزرار الثابتة:

- [ ] الأزرار الـ 3 (ESC, M, I) ظاهرين في أسفل الشاشة
- [ ] زر الـ Talk **مش ظاهر** في البداية

#### اختبار زر الـ Talk:

- [ ] امشِ ناحية تمثال ← لما تقرب (≤ 4 متر) وتبص عليه ← يظهر `[T] Talk to [اسم التمثال]`
- [ ] لف وشك عن التمثال ← يختفي زر الـ Talk
- [ ] اضغط **T** وأنت باصص على التمثال ← تبدأ المحادثة الصوتية ← النص يتغير لـ `Stop talking to [اسم التمثال]`
- [ ] اضغط **T** تاني ← تنتهي المحادثة ← النص يرجع `Talk to [اسم التمثال]`

#### اختبار الـ Menu:

- [ ] اضغط **ESC** ← يفتح Menu Panel
- [ ] الماوس يظهر ← اللاعب مش بيتحرك ← الكاميرا مش بتلف
- [ ] اضغط **Resume** أو **ESC** تاني ← يقفل الـ Menu ← يرجع كل حاجة طبيعي
- [ ] اضغط **Quit** ← اللعبة تقفل (في Editor بيوقف الـ Play mode)

#### اختبار الـ Information:

- [ ] وأنت **مش** واقف قدام تمثال ← اضغط **I** ← **مفيش حاجة بتحصل** ✅
- [ ] وأنت واقف قدام تمثال ← اضغط **I** ← يفتح panel باسم التمثال ووصفه
- [ ] اضغط **I** أو **ESC** ← يقفل الـ Info panel

#### اختبار الـ Map:

- [ ] اضغط **M** ← يفتح Map panel
- [ ] اضغط **M** أو **ESC** ← يقفل

#### اختبار أولويات الـ Panels:

- [ ] لو Menu مفتوح واضغطت **M** ← **مش هيحصل حاجة** (لازم تقفل Menu الأول بالـ ESC)
- [ ] لو Info مفتوح واضغطت **ESC** ← يقفل الـ Info (مش يفتح Menu)
- [ ] لو Map مفتوح واضغطت **ESC** ← يقفل الـ Map

---

## 🔧 استكشاف الأخطاء (Troubleshooting)

| المشكلة                            | الحل                                                                                     |
| ---------------------------------- | ---------------------------------------------------------------------------------------- |
| **الأزرار مش ظاهرة**               | تأكد إن الـ Canvas نوعه `Screen Space - Overlay` وإن الـ GameObjects مش disabled         |
| **زر Talk مش بيظهر**               | تأكد إن `TalkButtonGroup` مربوط في HUDManager + التمثال عليه `StatueInfo` + الـ Layer صح |
| **الـ Menu مش بيفتح**              | تأكد إن `MenuPanel` مربوط في HUDManager + مفيش script تاني بيستهلك ESC                   |
| **الـ Information فاضي**           | تأكد إنك ملّيت الـ `Description` field في `StatueInfo` على كل تمثال                      |
| **اللاعب بيتحرك والـ Menu مفتوح**  | تأكد إن `PlayerController` هو النسخة المعدلة (فيها subscribe للـ events)                 |
| **الكاميرا بتلف والـ Panel مفتوح** | تأكد إن الـ CinemachineCamera اسمه `CinemachineCamera` في الـ Hierarchy                  |
| **Time.timeScale stuck at 0**      | اضغط Resume أو أعد تشغيل الـ Play mode                                                   |

---

## 📁 Hierarchy النهائي المتوقع

```
HUDCanvas
├── PersistentButtons
│   ├── MenuButtonGroup
│   │   ├── KeyBadge
│   │   │   └── KeyText ("ESC")
│   │   └── Label ("Menu")
│   ├── MapButtonGroup
│   │   ├── KeyBadge
│   │   │   └── KeyText ("M")
│   │   └── Label ("Map")
│   └── InfoButtonGroup
│       ├── KeyBadge
│       │   └── KeyText ("I")
│       └── Label ("Information")
├── TalkButtonGroup
│   ├── KeyBadge
│   │   └── KeyText ("T")
│   └── TalkLabel ("Talk to Statue")
├── MenuPanel
│   ├── Title ("Menu")
│   ├── Separator
│   ├── ResumeButton
│   ├── SettingsButton
│   ├── VolumeSlider
│   └── QuitButton
├── InfoPanel
│   ├── InfoTitleText
│   ├── Separator
│   ├── InfoDescriptionText
│   └── CloseHint ("Press ESC or I to close")
├── MapPanel
│   ├── Title ("Museum Map")
│   ├── Placeholder ("Coming Soon...")
│   └── CloseHint ("Press ESC or M to close")
└── HUDManager (Script)
```

---

كده خلصت! 🎉 اتبع الخطوات بالترتيب وهتلاقي الـ HUD شغال بشكل احترافي.
