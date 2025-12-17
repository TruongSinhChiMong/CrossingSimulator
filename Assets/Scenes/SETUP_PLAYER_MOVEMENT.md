# Hướng dẫn Setup Player Movement & Animation

## Phần 1: Setup Animator Controller cho Player

### Bước 1: Mở Animator Controller
1. Mở file `Assets/Animations/Characters/Player/Player.controller`
2. Trong Animator window, bạn sẽ thấy các state: Idle, Walk, Cross, Stop, Hit, Dizzy

### Bước 2: Thêm Parameters (nếu chưa có)
Trong tab **Parameters**, thêm:
- `Speed` (Float) - Tốc độ di chuyển
- `IsIdle` (Bool) - Đang đứng yên
- `IsWalking` (Bool) - Đang đi bộ

### Bước 3: Setup Transitions
1. **Any State → Idle**
   - Condition: `IsIdle == true`
   - Has Exit Time: Uncheck
   - Transition Duration: 0.1

2. **Any State → Walk**
   - Condition: `IsWalking == true`
   - Has Exit Time: Uncheck
   - Transition Duration: 0.1

3. **Idle → Walk**
   - Condition: `Speed > 0.1`
   - Has Exit Time: Uncheck

4. **Walk → Idle**
   - Condition: `Speed < 0.1`
   - Has Exit Time: Uncheck

## Phần 2: Setup Player GameObject trong Scene

### Bước 1: Thêm Player vào Scene (TỰ ĐỘNG)
1. Mở scene **Map1** (hoặc scene bạn đang làm)
2. Vào menu: **Tools → Setup Player in Scene**
3. Player sẽ tự động được tạo ở vị trí (0, 2.5, 0)

**Hoặc thêm thủ công:**
1. Kéo prefab `Assets/Prefabs/Character/Player.prefab` vào Hierarchy
2. Đổi tên thành "Player"
3. Đặt Position: X=0, Y=2.5, Z=0

### Bước 2: Kiểm tra Components
Player cần có:
- **Transform**: Đặt vị trí phía trên đường (Y cao hơn Student)
- **Sprite Renderer**: Hiển thị sprite của Joe
- **Animator**: Gắn `Player.controller`
- **Rigidbody2D**: 
  - Body Type: Dynamic
  - Gravity Scale: 0
  - Collision Detection: Continuous
- **Collider2D**: (Box hoặc Capsule)
- **Player Controller (Script)**: Script vừa update
- **Player Input**: Để nhận input từ bàn phím

### Bước 3: Điều chỉnh vị trí Player
Để Player ở phía trên Student:
1. Chọn Player trong Hierarchy
2. Trong Inspector, điều chỉnh **Transform Position**:
   - X: 0 (hoặc vị trí bắt đầu)
   - Y: **2 đến 3** (cao hơn Student)
   - Z: 0

### Bước 4: Điều chỉnh Sorting Layer
Để Player hiển thị đúng thứ tự:
1. Chọn Player
2. Trong **Sprite Renderer**:
   - Sorting Layer: "Characters" (hoặc tạo mới)
   - Order in Layer: **10** (cao hơn Student)

## Phần 3: Setup Student Position

### Bước 1: Tìm Student Prefab
1. Mở `Assets/Prefabs/Student.prefab` (hoặc tìm trong scene)

### Bước 2: Điều chỉnh vị trí Student
1. Student nên ở vị trí Y thấp hơn Player:
   - Y: **0 đến 1** (ở mặt đường)

### Bước 3: Điều chỉnh Sorting Layer
1. Trong **Sprite Renderer**:
   - Sorting Layer: "Characters"
   - Order in Layer: **5** (thấp hơn Player)

## Phần 4: Test Game

### Bước 1: Chạy game
1. Nhấn Play trong Unity
2. Dùng **WASD** hoặc **Arrow Keys** để di chuyển Player

### Bước 2: Kiểm tra
- ✅ Player di chuyển theo chiều ngang (trái/phải)
- ✅ Player có animation Idle khi đứng yên
- ✅ Player có animation Walk khi di chuyển
- ✅ Player lật sprite theo hướng di chuyển
- ✅ Player ở phía trên Student (Y cao hơn)
- ✅ Player hiển thị đúng thứ tự (không bị che bởi Student)

## Phần 5: Điều chỉnh tốc độ (Optional)

Nếu muốn Player di chuyển nhanh/chậm hơn:
1. Chọn Player trong Hierarchy
2. Trong **Player Controller (Script)**:
   - Điều chỉnh **Move Speed**: 3 (mặc định)
   - Tăng lên 5-6 nếu muốn nhanh hơn
   - Giảm xuống 2 nếu muốn chậm hơn

## Troubleshooting

### Player không có animation
- Kiểm tra Animator component có gắn Player.controller không
- Kiểm tra các animation clips có tồn tại không

### Player bị che bởi Student
- Tăng Order in Layer của Player lên cao hơn
- Hoặc điều chỉnh Y position của Player cao hơn

### Player không di chuyển
- Kiểm tra Player Input component
- Kiểm tra Input System Actions có được setup đúng không
- Xem Console có lỗi gì không

