using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Runtime.UI;

/// <summary>
/// Editor tool để tự động thêm Settings Popup vào scene LevelSelection.
/// Menu: Tools > Setup > Add Settings Popup
/// </summary>
public static class SetupSettingsPopup
{
    [MenuItem("Tools/Setup/Add Settings Popup")]
    public static void AddSettingsPopup()
    {
        // 1) Kiểm tra đã có SettingsPopup chưa
        var existingPopup = Object.FindObjectOfType<SettingsPopup>(true);
        if (existingPopup != null)
        {
            Debug.LogWarning("[SetupSettingsPopup] SettingsPopup đã tồn tại trong scene!");
            Selection.activeGameObject = existingPopup.gameObject;
            return;
        }

        // 2) Tìm Canvas hiện có hoặc tạo mới
        var canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            var canvasGO = new GameObject("Canvas");
            Undo.RegisterCreatedObjectUndo(canvasGO, "Create Canvas");

            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            canvasGO.AddComponent<GraphicRaycaster>();
        }

        // 3) Tạo Settings Button
        var settingsBtn = CreateSettingsButton(canvas.transform);

        // 4) Tạo Settings Popup
        var popupGO = new GameObject("SettingsPopup");
        Undo.RegisterCreatedObjectUndo(popupGO, "Create SettingsPopup");
        popupGO.transform.SetParent(canvas.transform, false);

        var popupRect = popupGO.AddComponent<RectTransform>();
        popupRect.anchorMin = Vector2.zero;
        popupRect.anchorMax = Vector2.one;
        popupRect.offsetMin = Vector2.zero;
        popupRect.offsetMax = Vector2.zero;

        // 5) Tạo Overlay Background
        var overlayGO = CreateUIElement("OverlayBackground", popupGO.transform);
        var overlayRect = overlayGO.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;

        var overlayImage = overlayGO.AddComponent<Image>();
        overlayImage.color = new Color(0, 0, 0, 0.7f);

        // 6) Tạo Popup Panel
        var panelGO = CreateUIElement("PopupPanel", popupGO.transform);
        var panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(500, 400);

        var panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0.15f, 0.15f, 0.2f, 0.98f);

        // 7) Tạo Title
        var titleGO = CreateUIElement("TitleText", panelGO.transform);
        var titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 150);
        titleRect.sizeDelta = new Vector2(400, 60);

        var titleText = titleGO.AddComponent<TextMeshProUGUI>();
        titleText.text = "CÀI ĐẶT";
        titleText.fontSize = 48;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        titleText.fontStyle = FontStyles.Bold;

        // 8) Tạo Sound Setting container
        var soundSettingGO = CreateUIElement("SoundSetting", panelGO.transform);
        var soundSettingRect = soundSettingGO.GetComponent<RectTransform>();
        soundSettingRect.anchoredPosition = new Vector2(0, 40);
        soundSettingRect.sizeDelta = new Vector2(350, 60);

        var hlg = soundSettingGO.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 20;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlWidth = false;
        hlg.childControlHeight = false;

        // 8.1) Sound Label
        var soundLabelGO = CreateUIElement("SoundLabel", soundSettingGO.transform);
        var soundLabelRect = soundLabelGO.GetComponent<RectTransform>();
        soundLabelRect.sizeDelta = new Vector2(150, 50);

        var soundLabelText = soundLabelGO.AddComponent<TextMeshProUGUI>();
        soundLabelText.text = "Âm thanh";
        soundLabelText.fontSize = 32;
        soundLabelText.alignment = TextAlignmentOptions.MidlineRight;
        soundLabelText.color = Color.white;

        // 8.2) Sound Toggle
        var toggleGO = CreateToggle("SoundToggle", soundSettingGO.transform);
        var soundToggle = toggleGO.GetComponent<Toggle>();

        // 8.3) Sound Icons
        var soundOnIcon = CreateSoundIcon("SoundOnIcon", soundSettingGO.transform, true);
        var soundOffIcon = CreateSoundIcon("SoundOffIcon", soundSettingGO.transform, false);

        // 9) Tạo Buttons
        var saveBtn = CreateButton("SaveButton", panelGO.transform, new Vector2(0, -80), "LƯU", new Color(0.2f, 0.6f, 0.3f, 1f));
        var closeBtn = CreateButton("CloseButton", panelGO.transform, new Vector2(0, -160), "ĐÓNG", new Color(0.5f, 0.5f, 0.5f, 1f));

        // 10) Tạo Loading Indicator
        var loadingGO = CreateUIElement("LoadingIndicator", panelGO.transform);
        var loadingRect = loadingGO.GetComponent<RectTransform>();
        loadingRect.sizeDelta = new Vector2(50, 50);

        var loadingImage = loadingGO.AddComponent<Image>();
        loadingImage.color = Color.white;
        loadingGO.SetActive(false);

        // 11) Thêm SettingsPopup component
        var settingsPopup = popupGO.AddComponent<SettingsPopup>();

        // 12) Assign references qua SerializedObject
        var so = new SerializedObject(settingsPopup);
        so.FindProperty("popupPanel").objectReferenceValue = panelGO;
        so.FindProperty("overlayBackground").objectReferenceValue = overlayImage;
        so.FindProperty("soundToggle").objectReferenceValue = soundToggle;
        so.FindProperty("soundOnIcon").objectReferenceValue = soundOnIcon;
        so.FindProperty("soundOffIcon").objectReferenceValue = soundOffIcon;
        so.FindProperty("closeButton").objectReferenceValue = closeBtn;
        so.FindProperty("saveButton").objectReferenceValue = saveBtn;
        so.FindProperty("loadingIndicator").objectReferenceValue = loadingGO;
        so.ApplyModifiedProperties();

        // 13) Tạo Loading Indicator cho LevelSelection (load game data)
        var levelLoadingGO = CreateLoadingIndicator(canvas.transform);

        // 14) Link vào LevelSelectionManager
        var levelManager = Object.FindObjectOfType<LevelSelectionManager>();
        if (levelManager != null)
        {
            var lmSO = new SerializedObject(levelManager);
            lmSO.FindProperty("settingsPopup").objectReferenceValue = settingsPopup;
            lmSO.FindProperty("settingsButton").objectReferenceValue = settingsBtn;
            lmSO.FindProperty("loadingIndicator").objectReferenceValue = levelLoadingGO;
            lmSO.ApplyModifiedProperties();
            Debug.Log("[SetupSettingsPopup] Đã link SettingsPopup và LoadingIndicator vào LevelSelectionManager!");
        }
        else
        {
            Debug.LogWarning("[SetupSettingsPopup] Không tìm thấy LevelSelectionManager. Hãy assign thủ công.");
        }

        // 15) Đánh dấu scene dirty
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Selection.activeGameObject = popupGO;
        Debug.Log("[SetupSettingsPopup] Đã tạo Settings Popup thành công!");
    }

    private static GameObject CreateLoadingIndicator(Transform parent)
    {
        // Tạo container cho loading overlay
        var loadingGO = CreateUIElement("LoadingIndicator", parent);
        var loadingRect = loadingGO.GetComponent<RectTransform>();
        loadingRect.anchorMin = Vector2.zero;
        loadingRect.anchorMax = Vector2.one;
        loadingRect.offsetMin = Vector2.zero;
        loadingRect.offsetMax = Vector2.zero;

        // Background overlay
        var overlayImage = loadingGO.AddComponent<Image>();
        overlayImage.color = new Color(0, 0, 0, 0.5f);

        // Spinner container
        var spinnerGO = CreateUIElement("Spinner", loadingGO.transform);
        var spinnerRect = spinnerGO.GetComponent<RectTransform>();
        spinnerRect.sizeDelta = new Vector2(80, 80);

        var spinnerImage = spinnerGO.AddComponent<Image>();
        spinnerImage.color = Color.white;

        // Loading text
        var textGO = CreateUIElement("LoadingText", loadingGO.transform);
        var textRect = textGO.GetComponent<RectTransform>();
        textRect.anchoredPosition = new Vector2(0, -60);
        textRect.sizeDelta = new Vector2(300, 40);

        var loadingText = textGO.AddComponent<TextMeshProUGUI>();
        loadingText.text = "Đang tải...";
        loadingText.fontSize = 28;
        loadingText.alignment = TextAlignmentOptions.Center;
        loadingText.color = Color.white;

        loadingGO.SetActive(false);
        return loadingGO;
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

    private static Button CreateSettingsButton(Transform parent)
    {
        var go = CreateUIElement("SettingsButton", parent);
        var rect = go.GetComponent<RectTransform>();
        
        // Đặt góc phải trên
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(1, 1);
        rect.anchoredPosition = new Vector2(-30, -30);
        rect.sizeDelta = new Vector2(80, 80);

        var image = go.AddComponent<Image>();
        image.color = new Color(0.3f, 0.3f, 0.4f, 0.9f);

        var button = go.AddComponent<Button>();
        button.targetGraphic = image;

        // Icon text
        var textGO = CreateUIElement("Icon", go.transform);
        var textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        var text = textGO.AddComponent<TextMeshProUGUI>();
        text.text = "⚙";
        text.fontSize = 48;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;

        return button;
    }

    private static GameObject CreateToggle(string name, Transform parent)
    {
        var go = CreateUIElement(name, parent);
        var rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(80, 50);

        // Background
        var bgGO = CreateUIElement("Background", go.transform);
        var bgRect = bgGO.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        var bgImage = bgGO.AddComponent<Image>();
        bgImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);

        // Checkmark
        var checkGO = CreateUIElement("Checkmark", bgGO.transform);
        var checkRect = checkGO.GetComponent<RectTransform>();
        checkRect.anchorMin = new Vector2(0.1f, 0.1f);
        checkRect.anchorMax = new Vector2(0.9f, 0.9f);
        checkRect.offsetMin = Vector2.zero;
        checkRect.offsetMax = Vector2.zero;

        var checkImage = checkGO.AddComponent<Image>();
        checkImage.color = new Color(0.3f, 0.7f, 0.3f, 1f);

        // Toggle component
        var toggle = go.AddComponent<Toggle>();
        toggle.targetGraphic = bgImage;
        toggle.graphic = checkImage;
        toggle.isOn = true;

        return go;
    }

    private static Image CreateSoundIcon(string name, Transform parent, bool isOnIcon)
    {
        var go = CreateUIElement(name, parent);
        var rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(40, 40);

        var image = go.AddComponent<Image>();
        image.color = isOnIcon ? Color.green : Color.red;

        // Mặc định: On icon hiện, Off icon ẩn
        go.SetActive(isOnIcon);

        return image;
    }

    private static Button CreateButton(string name, Transform parent, Vector2 position, string label, Color bgColor)
    {
        var go = CreateUIElement(name, parent);
        var rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(200, 55);

        var image = go.AddComponent<Image>();
        image.color = bgColor;

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
        text.fontSize = 28;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.fontStyle = FontStyles.Bold;

        return button;
    }
}
