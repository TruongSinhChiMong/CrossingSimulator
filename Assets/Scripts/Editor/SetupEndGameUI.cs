using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Editor tool để tự động thêm EndGameUI vào scene hiện tại.
/// Menu: Tools > Setup > Add EndGame UI
/// </summary>
public static class SetupEndGameUI
{
    [MenuItem("Tools/Setup/Add EndGame UI")]
    public static void AddEndGameUI()
    {
        // 1) Kiểm tra đã có EndGameUI chưa
        var existingUI = Object.FindObjectOfType<EndGameUI>();
        if (existingUI != null)
        {
            Debug.LogWarning("[SetupEndGameUI] EndGameUI đã tồn tại trong scene!");
            Selection.activeGameObject = existingUI.gameObject;
            return;
        }

        // 2) Tạo Canvas
        var canvasGO = new GameObject("EndGameCanvas");
        Undo.RegisterCreatedObjectUndo(canvasGO, "Create EndGameCanvas");

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000; // Hiển thị trên cùng (cao hơn PopupService = 999)

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasGO.AddComponent<GraphicRaycaster>();

        // 3) Tạo Panel (background tối)
        var panelGO = CreateUIElement("EndGamePanel", canvasGO.transform);
        var panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        var panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);

        // 4) Tạo Content container
        var contentGO = CreateUIElement("Content", panelGO.transform);
        var contentRect = contentGO.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(600, 500);

        var contentImage = contentGO.AddComponent<Image>();
        contentImage.color = new Color(0.2f, 0.2f, 0.2f, 0.95f);

        // 5) Tạo Title
        var titleGO = CreateUIElement("Title", contentGO.transform);
        var titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 180);
        titleRect.sizeDelta = new Vector2(500, 60);

        var titleText = titleGO.AddComponent<TextMeshProUGUI>();
        titleText.text = "HOÀN THÀNH!";
        titleText.fontSize = 48;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;

        // 6) Tạo Stars container
        var starsGO = CreateUIElement("Stars", contentGO.transform);
        var starsRect = starsGO.GetComponent<RectTransform>();
        starsRect.anchoredPosition = new Vector2(0, 80);
        starsRect.sizeDelta = new Vector2(300, 80);

        var star1 = CreateStarImage("Star1", starsGO.transform, new Vector2(-80, 0));
        var star2 = CreateStarImage("Star2", starsGO.transform, new Vector2(0, 0));
        var star3 = CreateStarImage("Star3", starsGO.transform, new Vector2(80, 0));

        // 7) Tạo Score text
        var scoreGO = CreateUIElement("Score", contentGO.transform);
        var scoreRect = scoreGO.GetComponent<RectTransform>();
        scoreRect.anchoredPosition = new Vector2(0, 0);
        scoreRect.sizeDelta = new Vector2(400, 40);

        var scoreText = scoreGO.AddComponent<TextMeshProUGUI>();
        scoreText.text = "0/6 học sinh an toàn";
        scoreText.fontSize = 28;
        scoreText.alignment = TextAlignmentOptions.Center;
        scoreText.color = Color.white;

        // 8) Tạo Buttons
        var restartBtn = CreateButton("RestartButton", contentGO.transform, new Vector2(-120, -100), "Chơi lại");
        var nextBtn = CreateButton("NextLevelButton", contentGO.transform, new Vector2(120, -100), "Tiếp tục");
        var menuBtn = CreateButton("MenuButton", contentGO.transform, new Vector2(0, -180), "Menu");

        // 9) Thêm EndGameUI component
        var endGameUI = panelGO.AddComponent<EndGameUI>();

        // 10) Assign references qua SerializedObject
        var so = new SerializedObject(endGameUI);
        so.FindProperty("panel").objectReferenceValue = panelGO;
        so.FindProperty("endGameCanvas").objectReferenceValue = canvas;
        so.FindProperty("canvasSortingOrder").intValue = 1000;
        so.FindProperty("titleText").objectReferenceValue = titleText;
        so.FindProperty("star1").objectReferenceValue = star1;
        so.FindProperty("star2").objectReferenceValue = star2;
        so.FindProperty("star3").objectReferenceValue = star3;
        so.FindProperty("scoreText").objectReferenceValue = scoreText;
        so.FindProperty("restartButton").objectReferenceValue = restartBtn;
        so.FindProperty("nextLevelButton").objectReferenceValue = nextBtn;
        so.FindProperty("menuButton").objectReferenceValue = menuBtn;
        so.ApplyModifiedProperties();

        // 11) Link EndGameUI vào GameManager
        var gameManager = Object.FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            var gmSO = new SerializedObject(gameManager);
            gmSO.FindProperty("endGameUI").objectReferenceValue = endGameUI;
            gmSO.ApplyModifiedProperties();
            Debug.Log("[SetupEndGameUI] Đã link EndGameUI vào GameManager!");
        }
        else
        {
            Debug.LogWarning("[SetupEndGameUI] Không tìm thấy GameManager trong scene. Hãy assign endGameUI thủ công.");
        }

        // 12) Ẩn panel ban đầu
        panelGO.SetActive(false);

        // 13) Đánh dấu scene dirty
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Selection.activeGameObject = canvasGO;
        Debug.Log("[SetupEndGameUI] Đã tạo EndGameUI! Nhớ assign sprite cho Star On/Off trong Inspector.");
    }

    private static GameObject CreateUIElement(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);

        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        return go;
    }

    private static Image CreateStarImage(string name, Transform parent, Vector2 position)
    {
        var go = CreateUIElement(name, parent);
        var rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(60, 60);

        var image = go.AddComponent<Image>();
        image.color = Color.yellow;

        return image;
    }

    private static Button CreateButton(string name, Transform parent, Vector2 position, string label)
    {
        var go = CreateUIElement(name, parent);
        var rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(200, 50);

        var image = go.AddComponent<Image>();
        image.color = new Color(0.2f, 0.5f, 0.8f, 1f);

        var button = go.AddComponent<Button>();
        button.targetGraphic = image;

        // Text
        var textGO = CreateUIElement("Text", go.transform);
        var textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        var text = textGO.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = 24;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;

        return button;
    }
}
