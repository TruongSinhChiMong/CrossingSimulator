using System;
using UnityEngine;
using UnityEngine.UI;

namespace CrossingSimulator.UI
{
    /// <summary>
    /// Global popup overlay for lightweight alert dialogs so any script can call PopupService.Instance.Show(...).
    /// </summary>
    public class PopupService : MonoBehaviour
    {
        static PopupService instance;

        [Header("Popup Elements")]
        [SerializeField] Canvas popupCanvas;
        [SerializeField] Image overlayImage;
        [SerializeField] Text messageText;
        [SerializeField] Button closeButton;

        Action currentDismiss;

        public static PopupService Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                instance = FindObjectOfType<PopupService>();
                if (instance != null)
                    return instance;

                var go = new GameObject("PopupService");
                instance = go.AddComponent<PopupService>();
                DontDestroyOnLoad(go);
                instance.CreateRuntimePopup();
                return instance;
            }
        }

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            if (popupCanvas == null || overlayImage == null || messageText == null || closeButton == null)
                CreateRuntimePopup();

            closeButton.onClick.AddListener(Hide);
            Hide();
        }

        void OnDestroy()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveListener(Hide);

            if (instance == this)
                instance = null;
        }

        public void Show(string message, Action onDismiss = null)
        {
            if (messageText != null)
                messageText.text = string.IsNullOrEmpty(message) ? "Có lỗi xảy ra." : message;

            if (popupCanvas != null)
                popupCanvas.enabled = true;

            currentDismiss = onDismiss;
        }

        public void Hide()
        {
            if (popupCanvas != null)
                popupCanvas.enabled = false;

            currentDismiss?.Invoke();
            currentDismiss = null;
        }

        void CreateRuntimePopup()
        {
            popupCanvas = new GameObject("PopupCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)).GetComponent<Canvas>();
            popupCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            popupCanvas.sortingOrder = 999;
            popupCanvas.gameObject.layer = LayerMask.NameToLayer("UI");
            popupCanvas.transform.SetParent(transform, false);

            var scaler = popupCanvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            overlayImage = new GameObject("Overlay", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            overlayImage.rectTransform.SetParent(popupCanvas.transform, false);
            overlayImage.rectTransform.anchorMin = Vector2.zero;
            overlayImage.rectTransform.anchorMax = Vector2.one;
            overlayImage.rectTransform.offsetMin = Vector2.zero;
            overlayImage.rectTransform.offsetMax = Vector2.zero;
            overlayImage.color = new Color(0f, 0f, 0f, 0.65f);

            var panel = new GameObject("PopupPanel", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            panel.SetParent(overlayImage.transform, false);
            panel.sizeDelta = new Vector2(420, 220);
            panel.anchoredPosition = Vector2.zero;

            var panelImage = panel.GetComponent<Image>();
            panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);

            var messageGO = new GameObject("Message", typeof(RectTransform), typeof(Text));
            messageGO.transform.SetParent(panel, false);
            var messageRect = messageGO.GetComponent<RectTransform>();
            messageRect.anchorMin = new Vector2(0.1f, 0.35f);
            messageRect.anchorMax = new Vector2(0.9f, 0.85f);
            messageRect.offsetMin = Vector2.zero;
            messageRect.offsetMax = Vector2.zero;

            messageText = messageGO.GetComponent<Text>();
            messageText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            messageText.fontSize = 18;
            messageText.alignment = TextAnchor.MiddleCenter;
            messageText.color = Color.white;
            messageText.supportRichText = false;

            var buttonGO = new GameObject("CloseButton", typeof(RectTransform), typeof(Image), typeof(Button));
            buttonGO.transform.SetParent(panel, false);
            var buttonRect = buttonGO.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(200, 50);
            buttonRect.anchoredPosition = new Vector2(0, -70);

            closeButton = buttonGO.GetComponent<Button>();
            var buttonImage = buttonGO.GetComponent<Image>();
            buttonImage.color = new Color(0.19f, 0.43f, 0.84f, 1f);

            var buttonTextGO = new GameObject("Label", typeof(RectTransform), typeof(Text));
            buttonTextGO.transform.SetParent(buttonRect, false);
            var buttonText = buttonTextGO.GetComponent<Text>();
            var buttonTextRect = buttonTextGO.GetComponent<RectTransform>();
            buttonTextRect.anchorMin = Vector2.zero;
            buttonTextRect.anchorMax = Vector2.one;
            buttonTextRect.offsetMin = Vector2.zero;
            buttonTextRect.offsetMax = Vector2.zero;
            buttonText.font = messageText.font;
            buttonText.fontSize = 20;
            buttonText.text = "Đã hiểu";
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.color = Color.white;
        }
    }
}
