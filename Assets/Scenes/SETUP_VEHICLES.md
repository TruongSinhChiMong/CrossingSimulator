# Hướng Dẫn Setup Hệ Thống Xe Cộ

## Tổng Quan
- **VehicleController**: Điều khiển xe di chuyển
- **VehicleSpawner**: Spawn xe tự động theo làn đường

---

## Bước 1: Tạo Tag "Vehicle"

1. Vào **Edit → Project Settings → Tags and Layers**
2. Mở phần **Tags**
3. Click dấu **+** → nhập `Vehicle` → Enter

---

## Bước 2: Tạo Vehicle Prefab

### 2.1 Tạo xe trong Scene
1. Hierarchy → Click chuột phải → **2D Object → Sprite**
2. Đặt tên: `Car_01`
3. Trong Inspector:
   - **Sprite Renderer**: Chọn sprite xe (anh cần import sprite xe vào Assets/Art)
   - **Sorting Layer**: Đặt layer phù hợp để xe hiện trên đường

### 2.2 Thêm Components
1. Chọn `Car_01`
2. **Add Component → Rigidbody 2D**
   - Body Type: Dynamic
   - Gravity Scale: 0
   - Freeze Rotation Z: ✅
3. **Add Component → Box Collider 2D**
   - Is Trigger: ✅ (để detect va chạm với học sinh)
   - Size: Điều chỉnh vừa với sprite xe
4. **Add Component → Vehicle Controller**
   - Move Speed: 5
   - Move Right: true (sẽ được set tự động bởi Spawner)
   - Despawn Distance: 15

### 2.3 Gán Tag
1. Chọn `Car_01`
2. Tag dropdown → chọn **Vehicle**

### 2.4 Tạo Prefab
1. Kéo `Car_01` từ Hierarchy vào **Assets/Prefabs/Vehicles**
2. Xóa `Car_01` khỏi Hierarchy (prefab đã lưu)

### 2.5 Tạo thêm loại xe (tùy chọn)
- Duplicate prefab → đổi sprite → đặt tên `Car_02`, `Truck_01`, v.v.

---

## Bước 3: Setup Spawn Points

### 3.1 Tạo container
1. Hierarchy → Create Empty → đặt tên `VehicleSpawnPoints`

### 3.2 Tạo spawn point cho mỗi làn đường
1. Click chuột phải vào `VehicleSpawnPoints` → Create Empty
2. Đặt tên: `Lane1_Top` (xe spawn từ trên, đi xuống)
3. Position: X = [vị trí làn 1], Y = 8 (trên màn hình)

4. Tạo thêm spawn point:
   - `Lane2_Top`: X = [làn 2], Y = 8 (xe đi xuống)
   - `Lane3_Bottom`: X = [làn 3], Y = -8 (xe đi lên)
   - `Lane4_Bottom`: X = [làn 4], Y = -8 (xe đi lên)

```
Ví dụ layout 2 làn đường dọc:

    Spawn_Top_Lane1 (X=-2, Y=8)     Spawn_Top_Lane2 (X=2, Y=8)
              ↓                              ↓
              ↓  xe đi xuống                 ↓  xe đi xuống
              ↓                              ↓
    ─────────────────────────────────────────────────────────
              ↑                              ↑
              ↑  xe đi lên                   ↑  xe đi lên  
              ↑                              ↑
    Spawn_Bot_Lane3 (X=-2, Y=-8)   Spawn_Bot_Lane4 (X=2, Y=-8)
```

---

## Bước 4: Setup Vehicle Spawner

1. Hierarchy → Create Empty → đặt tên `VehicleSpawner`
2. **Add Component → Vehicle Spawner**
3. Cấu hình trong Inspector:

### Vehicle Prefabs
- Size: số loại xe (ví dụ: 2)
- Element 0: Kéo `Car_01` prefab vào
- Element 1: Kéo `Car_02` prefab vào

### Spawn Points
- Size: số spawn points
- Kéo các spawn point (`Lane1_Left`, `Lane2_Right`, v.v.) vào

### Spawn Settings
- Min Spawn Delay: 2 (giây tối thiểu giữa các xe)
- Max Spawn Delay: 5 (giây tối đa)

### Vehicle Settings
- Min Speed: 3
- Max Speed: 7

---

## Bước 5: Điều Chỉnh Độ Khó Theo Level

### Map 1 (Dễ)
```
Min Spawn Delay: 4
Max Spawn Delay: 7
Min Speed: 2
Max Speed: 4
Spawn Points: 2 làn
```

### Map 3 (Trung bình)
```
Min Spawn Delay: 2
Max Spawn Delay: 5
Min Speed: 3
Max Speed: 6
Spawn Points: 3 làn
```

### Map 5 (Khó)
```
Min Spawn Delay: 1
Max Spawn Delay: 3
Min Speed: 5
Max Speed: 8
Spawn Points: 4 làn
```

---

## Hierarchy Cuối Cùng

```
Scene
├── GameManager
├── SafeZone
├── StudentSpawner
├── VehicleSpawner          ← MỚI
├── VehicleSpawnPoints      ← MỚI
│   ├── Lane1_Left
│   ├── Lane1_Right
│   ├── Lane2_Left
│   └── Lane2_Right
├── Player
└── Canvas
    └── EndGamePanel
```

---

## Kiểm Tra Hoạt Động

1. **Play** game
2. Xe sẽ spawn từ các spawn points
3. Xe di chuyển qua màn hình và tự hủy khi ra khỏi
4. Khi xe chạm học sinh:
   - Console: `[Student] Got hit by vehicle!`
   - Học sinh dừng lại và bị đếm là "hit"

---

## Troubleshooting

### Xe không spawn
- Kiểm tra đã gán Vehicle Prefabs
- Kiểm tra đã gán Spawn Points
- Kiểm tra Is Spawning = true

### Xe không va chạm với học sinh
- Kiểm tra xe có tag `Vehicle`
- Kiểm tra xe có Collider 2D với Is Trigger = true
- Kiểm tra học sinh có Rigidbody 2D

### Xe đi sai hướng
- Spawn point ở trên (Y > 0) → xe đi xuống
- Spawn point ở dưới (Y < 0) → xe đi lên

---

## Import Sprite Xe

Nếu chưa có sprite xe:
1. Tìm free assets trên Unity Asset Store hoặc itch.io
2. Import vào **Assets/Art/Vehicles**
3. Cấu hình sprite:
   - Texture Type: Sprite (2D and UI)
   - Pixels Per Unit: 100 (điều chỉnh theo kích thước)
