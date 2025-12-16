# Hướng Dẫn Setup End Game System

## Tổng Quan
Hệ thống End Game gồm:
- **GameManager**: Quản lý trạng thái game, đếm học sinh an toàn/bị tai nạn
- **EndGameUI**: Hiển thị màn hình kết quả với sao và buttons
- **SafeZone**: Vùng trigger để xác định học sinh qua đường an toàn
- **Vehicle Tag**: Để xác định xe đâm học sinh

---

## Bước 1: Tạo GameManager GameObject

1. Trong Hierarchy, click chuột phải → **Create Empty**
2. Đặt tên: `GameManager`
3. Trong Inspector, click **Add Component** → tìm `GameManager`
4. Cấu hình trong Inspector:
   - **Current Level**: Số level hiện tại (1, 2, 3...)
   - **Total Students**: Tổng số học sinh trong màn (phải khớp với StudentSpawner)
   - **Win Threshold**: Tỷ lệ học sinh cần qua an toàn để thắng (0.5 = 50%)
   - **Progress Manager**: Kéo thả ProgressManager từ scene vào (nếu có)
   - **End Game UI**: Để trống, sẽ gán ở bước sau

---

## Bước 2: Tạo SafeZone (Vùng An Toàn)

SafeZone là nơi học sinh đến khi qua đường thành công.

1. Trong Hierarchy, click chuột phải → **Create Empty**
2. Đặt tên: `SafeZone`
3. Đặt vị trí ở **bên trái màn hình** (nơi học sinh đi tới)
   - Ví dụ Position: X = -10, Y = 0, Z = 0
4. Add Component → **Box Collider 2D**
5. Cấu hình Box Collider 2D:
   - ✅ Check **Is Trigger** = true
   - Size: X = 2, Y = 10 (đủ cao để cover đường đi)
6. **Quan trọng**: Tạo Tag mới
   - Click vào dropdown Tag → **Add Tag...**
   - Click dấu **+** → nhập `SafeZone` → Save
   - Quay lại SafeZone object → chọn Tag = `SafeZone`

```
Vị trí SafeZone trong scene:

    [SafeZone]     [Đường]     [Spawn Point]
        ←          học sinh         →
    X = -10                      X = 3
```

---

## Bước 3: Setup Tag cho Xe (Vehicle)

Nếu game có xe cộ chạy qua:

1. Chọn Prefab xe trong Project window
2. Trong Inspector, click Tag dropdown → **Add Tag...**
3. Tạo tag mới: `Vehicle`
4. Gán tag `Vehicle` cho tất cả prefab xe

---

## Bước 4: Tạo End Game UI Panel

### 4.1 Tạo Canvas (nếu chưa có)
1. Hierarchy → Click chuột phải → **UI → Canvas**
2. Đặt tên: `EndGameCanvas`
3. Canvas Scaler:
   - UI Scale Mode: **Scale With Screen Size**
   - Reference Resolution: 1920 x 1080

### 4.2 Tạo Panel chính
1. Click chuột phải vào Canvas → **UI → Panel**
2. Đặt tên: `EndGamePanel`
3. Cấu hình RectTransform:
   - Anchor: Stretch-Stretch (giữ Alt để set cả position)
   - Left, Right, Top, Bottom: 0
4. Image Color: RGBA(0, 0, 0, 200) - màu đen trong suốt

### 4.3 Tạo Content Container
1. Click chuột phải vào EndGamePanel → **Create Empty**
2. Đặt tên: `Content`
3. RectTransform:
   - Width: 600, Height: 400
   - Pos X: 0, Pos Y: 0

### 4.4 Tạo Title Text
1. Click chuột phải vào Content → **UI → Text - TextMeshPro**
2. Đặt tên: `TitleText`
3. Cấu hình:
   - Text: "HOÀN THÀNH!"
   - Font Size: 60
   - Alignment: Center
   - Pos Y: 120

### 4.5 Tạo 3 Stars
1. Click chuột phải vào Content → **UI → Image**
2. Đặt tên: `Star1`
3. Cấu hình:
   - Width: 80, Height: 80
   - Pos X: -100, Pos Y: 40
   - Source Image: sprite ngôi sao (tự chuẩn bị)
4. Duplicate (Ctrl+D) 2 lần → đặt tên `Star2`, `Star3`
5. Điều chỉnh vị trí:
   - Star1: Pos X = -100
   - Star2: Pos X = 0
   - Star3: Pos X = 100

### 4.6 Tạo Score Text
1. Click chuột phải vào Content → **UI → Text - TextMeshPro**
2. Đặt tên: `ScoreText`
3. Cấu hình:
   - Text: "0/6 học sinh an toàn"
   - Font Size: 30
   - Alignment: Center
   - Pos Y: -40

### 4.7 Tạo Buttons
**Restart Button:**
1. Click chuột phải vào Content → **UI → Button - TextMeshPro**
2. Đặt tên: `RestartButton`
3. Cấu hình:
   - Width: 180, Height: 50
   - Pos X: -100, Pos Y: -120
4. Đổi text con thành: "CHƠI LẠI"

**Next Level Button:**
1. Duplicate RestartButton → đặt tên `NextLevelButton`
2. Pos X: 100, Pos Y: -120
3. Đổi text: "TIẾP TỤC"

**Menu Button:**
1. Duplicate → đặt tên `MenuButton`
2. Pos X: 0, Pos Y: -180
3. Đổi text: "MENU"

### 4.8 Gắn EndGameUI Script
1. Chọn `EndGamePanel`
2. Add Component → `EndGameUI`
3. Kéo thả các references:
   - **Panel**: EndGamePanel (chính nó)
   - **Title Text**: TitleText
   - **Star 1, 2, 3**: Star1, Star2, Star3
   - **Star On**: Sprite sao sáng
   - **Star Off**: Sprite sao tối
   - **Score Text**: ScoreText
   - **Restart Button**: RestartButton
   - **Next Level Button**: NextLevelButton
   - **Menu Button**: MenuButton

---

## Bước 5: Kết Nối GameManager với EndGameUI

1. Chọn `GameManager` trong Hierarchy
2. Kéo `EndGamePanel` vào field **End Game UI**
3. Kéo `ProgressManager` vào field **Progress Manager** (nếu có)

---

## Bước 6: Cấu Hình StudentSpawner

1. Chọn StudentSpawner trong scene
2. Đảm bảo **Count** khớp với **Total Students** trong GameManager

---

## Hierarchy Cuối Cùng

```
Scene
├── GameManager          (GameManager script)
├── SafeZone             (Box Collider 2D - Is Trigger)
├── StudentSpawner       (StudentSpawner script)
├── Player               (PlayerController script)
├── Vehicles             (các xe với tag "Vehicle")
└── Canvas
    ├── ProgressBar      (UI tiến độ)
    └── EndGamePanel     (EndGameUI script)
        └── Content
            ├── TitleText
            ├── Star1
            ├── Star2
            ├── Star3
            ├── ScoreText
            ├── RestartButton
            ├── NextLevelButton
            └── MenuButton
```

---

## Kiểm Tra Hoạt Động

1. **Play** game
2. Để học sinh đi qua SafeZone → Console log: `[Student] Reached safety!`
3. Khi tất cả học sinh đã xử lý → EndGamePanel hiện lên
4. Test các button:
   - Chơi Lại → reload scene
   - Tiếp Tục → load Map tiếp theo
   - Menu → về LevelSelection

---

## Troubleshooting

### Học sinh không trigger SafeZone
- Kiểm tra SafeZone có **Is Trigger = true**
- Kiểm tra tag đúng là `SafeZone`
- Kiểm tra học sinh có **Rigidbody2D**

### EndGameUI không hiện
- Kiểm tra đã gán EndGameUI vào GameManager
- Kiểm tra Total Students đúng số lượng

### Buttons không hoạt động
- Kiểm tra đã gán đúng button references
- Kiểm tra Canvas có **Graphic Raycaster**
- Kiểm tra scene có **EventSystem**

---

## Tùy Chỉnh

### Đổi điều kiện thắng
Trong GameManager:
- `winThreshold = 0.5f` → cần 50% học sinh an toàn để thắng
- `winThreshold = 1f` → cần 100% học sinh an toàn

### Đổi cách tính sao
Sửa trong `GameManager.CalculateStars()`:
```csharp
int CalculateStars()
{
    float ratio = (float)safeStudents / totalStudents;
    if (ratio >= 1f) return 3;      // 100% = 3 sao
    if (ratio >= 0.66f) return 2;   // 66%+ = 2 sao
    if (ratio >= 0.33f) return 1;   // 33%+ = 1 sao
    return 0;
}
```
