# Hướng Dẫn Setup Settings Scene

## Bước 1: Tạo Settings Scene

1. **File → New Scene** → chọn Basic (Built-in)
2. **File → Save As** → đặt tên `Settings` → lưu vào `Assets/Scenes/`
3. **File → Build Settings** → kéo scene `Settings` vào danh sách

---

## Bước 2: Tạo UI Canvas

1. Hierarchy → Click chuột phải → **UI → Canvas**
2. Cấu hình Canvas:
   - Render Mode: Screen Space - Overlay
   - Canvas Scaler:
     - UI Scale Mode: Scale With Screen Size
     - Reference Resolution: 1920 x 1080

---

## Bước 3: Tạo Background Panel

1. Click chuột phải vào Canvas → **UI → Panel**
2. Đặt tên: `Background`
3. Image Color: màu nền tùy chọn

---

## Bước 4: Tạo Title

1. Click chuột phải vào Canvas → **UI → Text - TextMeshPro**
2. Đặt tên: `TitleText`
3. Cấu hình:
   - Text: "CÀI ĐẶT"
   - Font Size: 60
   - Alignment: Center
   - Pos Y: 300

---

## Bước 5: Tạo Sound Toggle

### 5.1 Container
1. Click chuột phải vào Canvas → **Create Empty**
2. Đặt tên: `SoundSetting`
3. Add Component → **Horizontal Layout Group**
4. Pos Y: 100

### 5.2 Label
1. Click chuột phải vào SoundSetting → **UI → Text - TextMeshPro**
2. Đặt tên: `SoundLabel`
3. Text: "Âm thanh"
4. Font Size: 36

### 5.3 Toggle
1. Click chuột phải vào SoundSetting → **UI → Toggle**
2. Đặt tên: `SoundToggle`
3. Is On: ✅ (mặc định bật)

### 5.4 Icons (tùy chọn)
1. Tạo 2 Image trong SoundSetting:
   - `SoundOnIcon` - sprite loa có sóng
   - `SoundOffIcon` - sprite loa gạch chéo

---

## Bước 6: Tạo Buttons

### 6.1 Save Button
1. Click chuột phải vào Canvas → **UI → Button - TextMeshPro**
2. Đặt tên: `SaveButton`
3. Pos Y: -100
4. Text: "LƯU"

### 6.2 Back Button
1. Duplicate SaveButton → đặt tên `BackButton`
2. Pos Y: -200
3. Text: "QUAY LẠI"

---

## Bước 7: Tạo Loading Indicator (tùy chọn)

1. Click chuột phải vào Canvas → **UI → Image**
2. Đặt tên: `LoadingIndicator`
3. Sprite: loading spinner
4. Đặt ở giữa màn hình
5. Mặc định: SetActive(false)

---

## Bước 8: Setup SettingsManager

1. Hierarchy → Create Empty → đặt tên `SettingsManager`
2. Add Component → `SettingsManager`
3. Kéo thả references:
   - **Sound Toggle**: SoundToggle
   - **Sound On Icon**: SoundOnIcon (nếu có)
   - **Sound Off Icon**: SoundOffIcon (nếu có)
   - **Back Button**: BackButton
   - **Save Button**: SaveButton
   - **Loading Indicator**: LoadingIndicator (nếu có)

---

## Bước 9: Thêm nút Settings vào Level Selection

### Trong scene LevelSelection:

1. Tạo Button mới trong Canvas
2. Đặt tên: `SettingsButton`
3. Text: "⚙️" hoặc "Cài đặt"
4. Đặt ở góc phải trên
5. Trong Inspector của Button:
   - On Click (+) → kéo `LevelSelectionManager` vào
   - Chọn function: `LevelSelectionManager.OpenSettings`

---

## Hierarchy Cuối Cùng (Settings Scene)

```
Settings Scene
├── Main Camera
├── EventSystem
├── SettingsManager        (SettingsManager script)
└── Canvas
    ├── Background
    ├── TitleText
    ├── SoundSetting
    │   ├── SoundLabel
    │   ├── SoundToggle
    │   ├── SoundOnIcon
    │   └── SoundOffIcon
    ├── SaveButton
    ├── BackButton
    └── LoadingIndicator
```

---

## Kiểm Tra

1. Từ LevelSelection, click nút Settings → chuyển sang Settings scene
2. Toggle Sound on/off → âm thanh game bật/tắt
3. Click Save hoặc Back → quay lại LevelSelection
4. Settings được lưu và sync với server

---

## Mở Rộng (Tương Lai)

Có thể thêm các settings khác:
- Music volume (Slider)
- SFX volume (Slider)
- Language selection (Dropdown)
- Notifications (Toggle)

Chỉ cần update `GameSettings` model và UI tương ứng.
