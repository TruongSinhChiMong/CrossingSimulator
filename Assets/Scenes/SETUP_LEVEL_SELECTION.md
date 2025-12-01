# Hướng dẫn Setup Level Selection Scene

## Bước 1: Mở Scene
1. Mở scene `LevelSelection.unity` trong Unity Editor

## Bước 2: Tạo Canvas
1. Chuột phải trong Hierarchy → UI → Canvas
2. Đổi tên thành "LevelSelectionCanvas"
3. Canvas Scaler:
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920 x 1080

## Bước 3: Tạo Background Panel
1. Chuột phải trên Canvas → UI → Panel
2. Đổi tên thành "Background"
3. Rect Transform: Stretch to fill (Left: 0, Top: 0, Right: 0, Bottom: 0)
4. Image Color: Màu tối (ví dụ: #1A1A24)

## Bước 4: Tạo Title
1. Chuột phải trên Canvas → UI → Text
2. Đổi tên thành "Title"
3. Text: "Select Level"
4. Font Size: 60
5. Alignment: Center
6. Pos Y: 400

## Bước 5: Tạo Level Button (Làm 5 lần cho 5 levels)
Cho mỗi level button:

1. Chuột phải trên Canvas → UI → Button
2. Đổi tên: "LevelButton_1" (2, 3, 4, 5)
3. Rect Transform:
   - Width: 180
   - Height: 220
   - Pos X: -400, -200, 0, 200, 400 (cho 5 buttons)
   - Pos Y: 50

4. Thêm component **LevelButton** script vào button

5. Trong button, tạo Star Image:
   - Chuột phải trên button → UI → Image
   - Đổi tên: "StarImage"
   - Width: 120, Height: 120
   - Pos Y: 20
   - Preserve Aspect: Check

6. Trong button, tạo Level Number Text:
   - Chuột phải trên button → UI → Text
   - Đổi tên: "LevelNumberText"
   - Text: "1" (2, 3, 4, 5)
   - Font Size: 32
   - Pos Y: -70

7. Gắn references vào LevelButton component:
   - Button: Kéo Button component vào
   - Star Image: Kéo StarImage vào
   - Level Number Text: Kéo LevelNumberText vào
   - Block Star Sprite: Kéo `block_star.png` từ Assets/Art/Sprites/UI
   - One Star Sprite: Kéo `1_star.png`
   - Two Star Sprite: Kéo `2_star.png`
   - Three Star Sprite: Kéo `3_star.png`

## Bước 6: Tạo Back Button
1. Chuột phải trên Canvas → UI → Button
2. Đổi tên: "BackButton"
3. Pos Y: -350
4. Text: "Back to Login"
5. OnClick: Kéo LevelSelectionManager → Chọn BackToLogin()

## Bước 7: Setup LevelSelectionManager
1. Tìm GameObject "LevelSelectionManager" trong Hierarchy
2. Trong Inspector:
   - Level Buttons: Size = 5
   - Kéo 5 LevelButton vào array (Element 0-4)
   - Total Levels: 5

## Xong!
Bây giờ anh có thể test game:
- Login với test/test
- Chọn level để chơi
- Level 1 sẽ mở khóa mặc định
