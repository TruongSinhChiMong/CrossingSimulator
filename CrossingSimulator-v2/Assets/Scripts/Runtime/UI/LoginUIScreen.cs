using CrossingSimulator.Networking;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CrossingSimulator.UI
{
    public class LoginUIScreen : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] InputField usernameField;
        [SerializeField] InputField passwordField;
        [SerializeField] Button loginButton;
        [SerializeField] Text feedbackLabel;
        [SerializeField] GameObject pressAnyKeyPrompt;

        [Header("Scene Flow")]
        [SerializeField] string nextSceneName = "Map1";
        [SerializeField] string title = "Crossing Simulator";

        Font runtimeFont;
        bool loginSucceeded;
        bool isSubmitting;
        Coroutine loginCoroutine;
        LoginResponse lastLoginResponse;

        void Awake()
        {
            runtimeFont = LoadRuntimeFont();

            if (!usernameField || !passwordField || !loginButton || !feedbackLabel || !pressAnyKeyPrompt)
                BuildRuntimeUI();

            if (loginButton != null)
                loginButton.onClick.AddListener(HandleLoginClicked);

            if (feedbackLabel != null)
                feedbackLabel.text = string.Empty;

            if (pressAnyKeyPrompt != null)
                pressAnyKeyPrompt.SetActive(false);
        }

        void OnDestroy()
        {
            if (loginButton != null)
                loginButton.onClick.RemoveListener(HandleLoginClicked);

            if (loginCoroutine != null)
                StopCoroutine(loginCoroutine);
        }

        void Update()
        {
            if (loginSucceeded && WasAnyInputPressedThisFrame())
                SceneManager.LoadScene(nextSceneName);
        }

        void HandleLoginClicked()
        {
            if (isSubmitting)
                return;

            var email = usernameField != null ? usernameField.text.Trim() : string.Empty;
            var password = passwordField != null ? passwordField.text.Trim() : string.Empty;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                PopupService.Instance.Show("Nhập username và password trước nhé!");
                return;
            }

            BeginLogin(email, password);
        }

        void BeginLogin(string email, string password)
        {
            loginSucceeded = false;
            isSubmitting = true;
            lastLoginResponse = null;

            if (pressAnyKeyPrompt != null)
                pressAnyKeyPrompt.SetActive(false);

            SetControlsInteractable(false);
            SetFeedback("Đang đăng nhập...");

            var payload = new LoginRequest
            {
                email = email,
                password = password
            };

            if (loginCoroutine != null)
                StopCoroutine(loginCoroutine);

            loginCoroutine = ApiService.Instance.PostJson(ApiPaths.Login, payload, OnLoginCompleted);
        }

        void OnLoginCompleted(ApiResponse response)
        {
            isSubmitting = false;
            loginCoroutine = null;

            var envelope = response.GetEnvelopeOrDefault<LoginResponse>();
            var success = response.Success && envelope.IsSuccessStatus && envelope.data != null;

            if (!success)
            {
                var message = !string.IsNullOrEmpty(envelope.message)
                    ? envelope.message
                    : (!string.IsNullOrEmpty(response.Error) ? response.Error : "Đăng nhập thất bại, thử lại nhé.");

                SetControlsInteractable(true);
                PopupService.Instance.Show(message);
                return;
            }

            lastLoginResponse = envelope.data;
            loginSucceeded = true;
            AuthTokenStore.Instance.ApplyLoginResponse(lastLoginResponse);

            SetFeedback("Đăng nhập thành công! Nhấn phím bất kỳ để vào game.");
            SetControlsInteractable(false);

            if (pressAnyKeyPrompt != null)
                pressAnyKeyPrompt.SetActive(true);
        }

        Font LoadRuntimeFont()
        {
            try
            {
                return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
            catch
            {
                Debug.LogWarning("LegacyRuntime.ttf missing, falling back to Arial.");
                return Resources.GetBuiltinResource<Font>("Arial.ttf");
            }
        }

        void BuildRuntimeUI()
        {
            EnsureEventSystem();

            var canvasGO = new GameObject("LoginCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGO.transform.SetParent(transform, false);

            var canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var uiLayer = LayerMask.NameToLayer("UI");
            if (uiLayer >= 0)
                SetLayerRecursively(canvasGO.transform, uiLayer);

            var scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            var panel = CreateRect("Panel", canvasGO.transform, new Vector2(520, 360), Vector2.zero);
            var panelImage = panel.gameObject.AddComponent<Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.65f);

            var titleLabel = CreateText("Title", panel, title, 28, new Vector2(0, 120));
            titleLabel.fontStyle = FontStyle.Bold;
            titleLabel.color = Color.white;

            var usernameLabel = CreateText("UsernameLabel", panel, "Username", 18, new Vector2(0, 60));
            usernameLabel.alignment = TextAnchor.MiddleLeft;

            usernameField = CreateInputField("UsernameInput", panel, new Vector2(0, 20), "Enter username");

            var passwordLabel = CreateText("PasswordLabel", panel, "Password", 18, new Vector2(0, -30));
            passwordLabel.alignment = TextAnchor.MiddleLeft;

            passwordField = CreateInputField("PasswordInput", panel, new Vector2(0, -70), "Enter password");
            passwordField.contentType = InputField.ContentType.Password;

            loginButton = CreateButton("LoginButton", panel, new Vector2(0, -130), "Login");

            feedbackLabel = CreateText("FeedbackLabel", panel, string.Empty, 16, new Vector2(0, -180));
            feedbackLabel.color = Color.white;

            var pressLabel = CreateText("PressAnyKey", panel, "Press any key", 18, new Vector2(0, -220));
            pressLabel.fontStyle = FontStyle.Bold;
            pressLabel.color = new Color(1f, 1f, 1f, 0.9f);
            pressLabel.gameObject.SetActive(false);
            pressAnyKeyPrompt = pressLabel.gameObject;
        }

        void EnsureEventSystem()
        {
            var existing = FindObjectOfType<EventSystem>();
            if (existing != null)
            {
                var standalone = existing.GetComponent<StandaloneInputModule>();
                if (standalone != null)
                    Destroy(standalone);

                if (!existing.TryGetComponent<InputSystemUIInputModule>(out _))
                    existing.gameObject.AddComponent<InputSystemUIInputModule>();
                return;
            }

            var eventSystemGO = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            eventSystemGO.transform.SetParent(transform, false);
        }

        Text CreateText(string name, RectTransform parent, string content, int fontSize, Vector2 anchoredPosition)
        {
            var rect = CreateRect(name, parent, new Vector2(400, 40), anchoredPosition);
            var text = rect.gameObject.AddComponent<Text>();
            text.font = runtimeFont;
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = fontSize;
            text.text = content;
            text.color = new Color(0.9f, 0.9f, 0.9f, 1f);
            return text;
        }

        Button CreateButton(string name, RectTransform parent, Vector2 anchoredPosition, string label)
        {
            var rect = CreateRect(name, parent, new Vector2(220, 48), anchoredPosition);
            var image = rect.gameObject.AddComponent<Image>();
            image.color = new Color(0.19f, 0.43f, 0.84f, 1f);

            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            var textRect = CreateChildRect(rect, "Text");
            textRect.offsetMin = new Vector2(10, 6);
            textRect.offsetMax = new Vector2(-10, -6);
            var text = textRect.gameObject.AddComponent<Text>();
            text.font = runtimeFont;
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = 20;
            text.text = label;
            text.color = Color.white;

            return button;
        }

        InputField CreateInputField(string name, RectTransform parent, Vector2 anchoredPosition, string placeholderText)
        {
            var rect = CreateRect(name, parent, new Vector2(380, 40), anchoredPosition);
            var image = rect.gameObject.AddComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.1f);

            var input = rect.gameObject.AddComponent<InputField>();

            var placeholderRect = CreateChildRect(rect, "Placeholder");
            placeholderRect.offsetMin = new Vector2(12, 6);
            placeholderRect.offsetMax = new Vector2(-12, -6);
            var placeholder = placeholderRect.gameObject.AddComponent<Text>();
            placeholder.font = runtimeFont;
            placeholder.fontStyle = FontStyle.Italic;
            placeholder.alignment = TextAnchor.MiddleLeft;
            placeholder.text = placeholderText;
            placeholder.color = new Color(1f, 1f, 1f, 0.4f);

            var textRect = CreateChildRect(rect, "Text");
            textRect.offsetMin = new Vector2(12, 6);
            textRect.offsetMax = new Vector2(-12, -6);
            var text = textRect.gameObject.AddComponent<Text>();
            text.font = runtimeFont;
            text.alignment = TextAnchor.MiddleLeft;
            text.color = Color.white;
            text.supportRichText = false;

            input.textComponent = text;
            input.placeholder = placeholder;
            input.lineType = InputField.LineType.SingleLine;

            return input;
        }

        RectTransform CreateRect(string name, Transform parent, Vector2 size, Vector2 anchoredPosition)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;
            return rect;
        }

        RectTransform CreateChildRect(RectTransform parent, string name)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return rect;
        }

        void SetLayerRecursively(Transform root, int layer)
        {
            root.gameObject.layer = layer;
            for (int i = 0; i < root.childCount; i++)
                SetLayerRecursively(root.GetChild(i), layer);
        }

        bool WasAnyInputPressedThisFrame()
        {
            var devices = InputSystem.devices;
            for (int i = 0; i < devices.Count; i++)
            {
                var device = devices[i];
                var controls = device.allControls;
                for (int j = 0; j < controls.Count; j++)
                {
                    if (controls[j] is ButtonControl button && button.wasPressedThisFrame)
                        return true;
                }
            }

            return false;
        }

        void SetControlsInteractable(bool interactable)
        {
            if (loginButton != null)
                loginButton.interactable = interactable;
            if (usernameField != null)
                usernameField.interactable = interactable;
            if (passwordField != null)
                passwordField.interactable = interactable;
        }

        void SetFeedback(string message)
        {
            if (feedbackLabel != null)
                feedbackLabel.text = message ?? string.Empty;
        }
    }
}
