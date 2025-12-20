# Hướng Dẫn Setup Settings Popup

## Bước 1: Tạo Settings Popup trong LevelSelection Scene

1. Mở scene `LevelSelection.unity`
2. Trong Canvas, click chuột phải → **Create Empty**
3. Đặt tên: `SettingsPopup`
4. Add Component → `SettingsPopup` script

---

## Bước 2: Tạo Overlay Background

1. Click chuột phải vào `SettingsPopup` → **UI → Image**
2. Đặt tên: `OverlayBackground`
3. Cấu hình RectTransform:
   - Anchor: Stretch-Stretch (giữ Alt kéo góc dưới phải)
   - Left, Top, Right, Bottom: 0
4. Image Color: `rgba(0, 0, 0, 0.7)` (đen trong suốt)

---

## Bước 3: Tạo Popup Panel

1. Click chuột phải vào `SettingsPopup` → **UI → Panel**
2. Đặt tên: `PopupPanel`
3. Cấu hình:
   - Width: 500, Height: 400
   - Pos: (0, 0) - giữa màn hình
   - Image Color: màu nền panel (ví dụ: trắng hoặc xám đậm)

---

## Bước 4: Tạo Title

1. Click chuột phải vào `PopupPanel` → **UI → Text - TextMeshPro**
2. Đặt tên: `TitleText`
3. Cấu hình:
   - Text: "CÀI ĐẶT"
   - Font Size: 48
   - Alignment: Center
   - Pos Y: 140

---

## Bước 5: Tạo Sound Setting

### 5.1 Container
1. Click chuột phải vào `PopupPanel` → **Create Empty**
2. Đặt tên: `SoundSetting`
3. Add Component → **Horizontal Layout Group**
4. Pos Y: 40

### 5.2 Label
1. Click chuột phải vào `SoundSetting` → **UI → Text - TextMeshPro**
2. Text: "Âm thanh"
3. Font Size: 32

### 5.3 Toggle
1. Click chuột phải vào `SoundSetting` → **UI → Toggle**
2. Đặt tên: `SoundToggle`
3. Is On: ✅

### 5.4 Icons (tùy chọn)
1. Tạo 2 Image:
   - `SoundOnIcon` - sprite loa có sóng
   - `SoundOffIcon` - sprite loa gạch chéo

---

## Bước 6: Tạo Buttons

### 6.1 Save Button
1. Click chuột phải vào `PopupPanel` → **UI → Button - TextMeshPro**
2. Đặt tên: `SaveButton`
3. Pos Y: -80
4. Text: "LƯU"

### 6.2 Close Button
1. Click chuột phải vào `PopupPanel` → **UI → Button - TextMeshPro**
2. Đặt tên: `CloseButton`
3. Pos Y: -150
4. Text: "ĐÓNG" hoặc icon "X"

---

## Bước 7: Tạo Loading Indicator (tùy chọn)

1. Click chuột phải vào `PopupPanel` → **UI → Image**
2. Đặt tên: `LoadingIndicator`
3. Sprite: loading spinner
4. Đặt ở giữa panel

---

## Bước 8: Gán References cho SettingsPopup

Chọn `SettingsPopup` object, trong Inspector kéo thả:
- **Popup Panel**: PopupPanel
- **Overlay Background**: OverlayBackground
- **Sound Toggle**: SoundToggle
- **Sound On Icon**: SoundOnIcon (nếu có)
- **Sound Off Icon**: SoundOffIcon (nếu có)
- **Close Button**: CloseButton
- **Save Button**: SaveButton
- **Loading Indicator**: LoadingIndicator (nếu có)

---

## Bước 9: Tạo Settings Button

1. Trong Canvas (ngoài SettingsPopup), tạo **UI → Button - TextMeshPro**
2. Đặt tên: `SettingsButton`
3. Text: "⚙️" hoặc "Cài đặt"
4. Đặt ở góc phải trên màn hình

---

## Bước 10: Gán References cho LevelSelectionManager

Chọn object có `LevelSelectionManager`, trong Inspector:
- **Settings Popup**: kéo `SettingsPopup` vào
- **Settings Button**: kéo `SettingsButton` vào

---

## Hierarchy Cuối Cùng

```
LevelSelection Scene
├── Main Camera
├── EventSystem
├── LevelSelectionManager
└── Canvas
    ├── ... (các UI khác)
    ├── SettingsButton          ← Nút mở popup
    └── SettingsPopup           ← SettingsPopup script
        ├── OverlayBackground   ← Image đen trong suốt
        └── PopupPanel
            ├── TitleText
            ├── SoundSetting
            │   ├── SoundLabel
            │   ├── SoundToggle
            │   ├── SoundOnIcon
            │   └── SoundOffIcon
            ├── SaveButton
            ├── CloseButton
            └── LoadingIndicator
```

---

## Kiểm Tra

1. Play scene LevelSelection
2. Click nút Settings → popup hiện lên với overlay tối
3. Toggle Sound → âm thanh bật/tắt
4. Click Save hoặc Close → popup đóng lại
5. Click vào overlay (vùng tối) → popup cũng đóng

---

## Lưu Ý

- Popup sẽ tự ẩn khi scene load (trong Awake)
- Settings được auto-save khi đóng popup
- Có thể xóa scene `Settings.unity` nếu không cần nữa
