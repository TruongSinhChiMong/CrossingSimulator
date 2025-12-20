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
        [SerializeField] InputField displayNameField;
        [SerializeField] Button loginButton;
        [SerializeField] Button registerButton;
        [SerializeField] Button switchModeButton;
        [SerializeField] Text feedbackLabel;
        [SerializeField] GameObject pressAnyKeyPrompt;
        [SerializeField] Text titleLabel;
        [SerializeField] GameObject displayNameContainer;

        [Header("Scene Flow")]
        [SerializeField] string nextSceneName = "LevelSelection";
        [SerializeField] string title = "Crossing Simulator";

        [Header("Debug Options")]
        [SerializeField] bool enableFakeLogin = true;
        [SerializeField] string fakeLoginUsername = "test";
        [SerializeField] string fakeLoginPassword = "test";

        Font runtimeFont;
        bool loginSucceeded;
        bool isSubmitting;
        bool isRegisterMode;
        Coroutine loginCoroutine;
        LoginResponse lastLoginResponse;

        void Awake()
        {
            runtimeFont = LoadRuntimeFont();

            if (!usernameField || !passwordField || !loginButton || !feedbackLabel || !pressAnyKeyPrompt)
                BuildRuntimeUI();

            if (loginButton != null)
                loginButton.onClick.AddListener(HandleLoginClicked);
            
            if (registerButton != null)
                registerButton.onClick.AddListener(HandleRegisterClicked);
            
            if (switchModeButton != null)
                switchModeButton.onClick.AddListener(HandleSwitchModeClicked);

            if (feedbackLabel != null)
                feedbackLabel.text = string.Empty;

            if (pressAnyKeyPrompt != null)
                pressAnyKeyPrompt.SetActive(false);
            
            // Bắt đầu ở chế độ Login
            isRegisterMode = false;
            UpdateUIMode();
        }

        void OnDestroy()
        {
            if (loginButton != null)
                loginButton.onClick.RemoveListener(HandleLoginClicked);
            
            if (registerButton != null)
                registerButton.onClick.RemoveListener(HandleRegisterClicked);
            
            if (switchModeButton != null)
                switchModeButton.onClick.RemoveListener(HandleSwitchModeClicked);

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
                PopupService.Instance.Show("Nhập email và password trước nhé!");
                return;
            }

            BeginLogin(email, password);
        }
        
        void HandleRegisterClicked()
        {
            if (isSubmitting)
                return;

            var email = usernameField != null ? usernameField.text.Trim() : string.Empty;
            var password = passwordField != null ? passwordField.text.Trim() : string.Empty;
            var displayName = displayNameField != null ? displayNameField.text.Trim() : string.Empty;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                PopupService.Instance.Show("Nhập email và password trước nhé!");
                return;
            }
            
            if (string.IsNullOrEmpty(displayName))
            {
                PopupService.Instance.Show("Nhập tên hiển thị trước nhé!");
                return;
            }
            
            if (password.Length < 6)
            {
                PopupService.Instance.Show("Password phải có ít nhất 6 ký tự!");
                return;
            }

            BeginRegister(email, password, displayName);
        }
        
        void HandleSwitchModeClicked()
        {
            if (isSubmitting)
                return;
            
            isRegisterMode = !isRegisterMode;
            UpdateUIMode();
            SetFeedback(string.Empty);
        }
        
        void UpdateUIMode()
        {
            // Cập nhật title
            if (titleLabel != null)
                titleLabel.text = isRegisterMode ? "ĐĂNG KÝ" : title;
            
            // Hiện/ẩn display name field
            if (displayNameContainer != null)
                displayNameContainer.SetActive(isRegisterMode);
            else if (displayNameField != null)
                displayNameField.gameObject.SetActive(isRegisterMode);
            
            // Hiện/ẩn buttons
            if (loginButton != null)
                loginButton.gameObject.SetActive(!isRegisterMode);
            
            if (registerButton != null)
                registerButton.gameObject.SetActive(isRegisterMode);
            
            // Cập nhật vị trí switch button dựa trên mode
            if (switchModeButton != null)
            {
                var switchRect = switchModeButton.GetComponent<RectTransform>();
                if (switchRect != null)
                {
                    // Login mode: switch button cách login button 16px (login Y=-84, height=60 → bottom=-114, switch=-114-16-30=-160)
                    // Register mode: switch button cách register button 16px (register Y=-186, height=60 → bottom=-216, switch=-216-16-30=-262)
                    float switchY = isRegisterMode ? -262f : -160f;
                    switchRect.anchoredPosition = new Vector2(0, switchY);
                }
                
                var btnText = switchModeButton.GetComponentInChildren<Text>();
                if (btnText != null)
                    btnText.text = isRegisterMode ? "Đã có tài khoản?" : "Chưa có tài khoản?";
            }
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

            // Check for fake login
            if (enableFakeLogin && email.Equals(fakeLoginUsername, System.StringComparison.OrdinalIgnoreCase) 
                && password == fakeLoginPassword)
            {
                Debug.Log("[LoginUIScreen] Using fake login for testing");
                OnFakeLoginSuccess();
                return;
            }

            var payload = new LoginRequest
            {
                email = email,
                password = password
            };

            if (loginCoroutine != null)
                StopCoroutine(loginCoroutine);

            loginCoroutine = ApiService.Instance.PostJson(ApiPaths.Login, payload, OnLoginCompleted);
        }

        void BeginRegister(string email, string password, string displayName)
        {
            loginSucceeded = false;
            isSubmitting = true;
            lastLoginResponse = null;

            if (pressAnyKeyPrompt != null)
                pressAnyKeyPrompt.SetActive(false);

            SetControlsInteractable(false);
            SetFeedback("Đang đăng ký...");

            var payload = new RegisterRequest
            {
                email = email,
                password = password
            };

            if (loginCoroutine != null)
                StopCoroutine(loginCoroutine);

            loginCoroutine = ApiService.Instance.PostJson(ApiPaths.Register, payload, OnRegisterCompleted);
        }
        
        void OnRegisterCompleted(ApiResponse response)
        {
            isSubmitting = false;
            loginCoroutine = null;

            var envelope = response.GetEnvelopeOrDefault<RegisterResponse>();
            var success = response.Success && envelope.IsSuccessStatus && envelope.data != null;

            if (!success)
            {
                var message = !string.IsNullOrEmpty(envelope.message)
                    ? envelope.message
                    : (!string.IsNullOrEmpty(response.Error) ? response.Error : "Đăng ký thất bại, vui lòng thử lại.");

                SetControlsInteractable(true);
                PopupService.Instance.Show(message);
                return;
            }

            // Đăng ký thành công - tự động đăng nhập
            var registerData = envelope.data;
            lastLoginResponse = new LoginResponse
            {
                idToken = registerData.idToken,
                uid = registerData.uid,
                email = registerData.email,
                displayName = null,
                refreshToken = registerData.refreshToken
            };
            
            loginSucceeded = true;
            AuthTokenStore.Instance.ApplyLoginResponse(lastLoginResponse);

            SetFeedback("Đăng ký thành công! Nhấn phím bất kỳ để vào game.");
            SetControlsInteractable(false);

            if (pressAnyKeyPrompt != null)
                pressAnyKeyPrompt.SetActive(true);
        }

        void OnFakeLoginSuccess()
        {
            isSubmitting = false;
            loginSucceeded = true;

            // Create fake login response
            lastLoginResponse = new LoginResponse
            {
                idToken = "fake_token_for_testing",
                uid = "test_user_id",
                email = fakeLoginUsername + "@test.com",
                displayName = "Test User",
                refreshToken = "fake_refresh_token"
            };

            // Apply fake token if AuthTokenStore exists
            if (AuthTokenStore.Instance != null)
            {
                AuthTokenStore.Instance.ApplyLoginResponse(lastLoginResponse);
            }

            SetFeedback("Login successful! (Test Mode) Press any key to enter the game.");
            SetControlsInteractable(false);

            if (pressAnyKeyPrompt != null)
                pressAnyKeyPrompt.SetActive(true);
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
                    : (!string.IsNullOrEmpty(response.Error) ? response.Error : "Login failed, please try again.");

                SetControlsInteractable(true);
                PopupService.Instance.Show(message);
                return;
            }

            lastLoginResponse = envelope.data;
            loginSucceeded = true;
            AuthTokenStore.Instance.ApplyLoginResponse(lastLoginResponse);

            SetFeedback("Login successful! Press any key to enter the game.");
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

            var panel = CreateRect("Panel", canvasGO.transform, new Vector2(700, 620), Vector2.zero);
            var panelImage = panel.gameObject.AddComponent<Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.65f);

            titleLabel = CreateText("Title", panel, title, 42, new Vector2(0, 240));
            titleLabel.fontStyle = FontStyle.Bold;
            titleLabel.color = Color.white;

            var usernameLabel = CreateLabel("UsernameLabel", panel, "Email", 24, new Vector2(0, 160));
            usernameField = CreateInputField("UsernameInput", panel, new Vector2(0, 110), "Nhập email");

            var passwordLabel = CreateLabel("PasswordLabel", panel, "Password", 24, new Vector2(0, 40));
            passwordField = CreateInputField("PasswordInput", panel, new Vector2(0, -10), "Nhập password");
            passwordField.contentType = InputField.ContentType.Password;
            
            // Display Name field (chỉ hiện khi đăng ký)
            var displayNameContainerRect = CreateRect("DisplayNameContainer", panel, new Vector2(520, 100), new Vector2(0, -90));
            displayNameContainer = displayNameContainerRect.gameObject;
            
            var displayNameLabel = CreateLabel("DisplayNameLabel", displayNameContainerRect, "Tên hiển thị", 24, new Vector2(0, 30));
            displayNameField = CreateInputField("DisplayNameInput", displayNameContainerRect, new Vector2(0, -20), "Nhập tên hiển thị");

            // Login button: cách password input (Y=-10, height=55) khoảng 16px
            // Password bottom = -10 - 27.5 = -37.5, button top = -37.5 - 16 = -53.5
            // Button height = 60, center = -53.5 - 30 = -83.5 ≈ -84
            loginButton = CreateButton("LoginButton", panel, new Vector2(0, -84), "Đăng nhập");
            
            // Register button: cách displayName input (trong container Y=-90, input Y=-20) khoảng 16px
            // DisplayName container bottom ≈ -90 - 50 = -140, + 16 + 30 = -186
            registerButton = CreateButton("RegisterButton", panel, new Vector2(0, -186), "Đăng ký");
            
            // Switch mode button cách button chính 16px
            switchModeButton = CreateButton("SwitchModeButton", panel, new Vector2(0, -160), "Chưa có tài khoản? Đăng ký");
            var switchBtnImage = switchModeButton.GetComponent<Image>();
            if (switchBtnImage != null)
                switchBtnImage.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);

            feedbackLabel = CreateText("FeedbackLabel", panel, string.Empty, 20, new Vector2(0, -230));
            feedbackLabel.color = Color.white;

            var pressLabel = CreateText("PressAnyKey", panel, "Nhấn phím bất kỳ", 24, new Vector2(0, -270));
            pressLabel.fontStyle = FontStyle.Bold;
            pressLabel.color = Color.white;
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
            // Dùng width = 520 để căn chỉnh với input field
            var rect = CreateRect(name, parent, new Vector2(520, 40), anchoredPosition);
            var text = rect.gameObject.AddComponent<Text>();
            text.font = runtimeFont;
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = fontSize;
            text.text = content;
            text.color = Color.white;
            return text;
        }
        
        Text CreateLabel(string name, RectTransform parent, string content, int fontSize, Vector2 anchoredPosition)
        {
            // Label căn trái, cùng width với input field
            var rect = CreateRect(name, parent, new Vector2(520, 35), anchoredPosition);
            var text = rect.gameObject.AddComponent<Text>();
            text.font = runtimeFont;
            text.alignment = TextAnchor.MiddleLeft;
            text.fontSize = fontSize;
            text.text = content;
            text.color = Color.white;
            return text;
        }

        Button CreateButton(string name, RectTransform parent, Vector2 anchoredPosition, string label)
        {
            var rect = CreateRect(name, parent, new Vector2(300, 60), anchoredPosition);
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
            text.fontSize = 26;
            text.text = label;
            text.color = Color.white;

            return button;
        }

        InputField CreateInputField(string name, RectTransform parent, Vector2 anchoredPosition, string placeholderText)
        {
            var rect = CreateRect(name, parent, new Vector2(520, 55), anchoredPosition);
            var image = rect.gameObject.AddComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.25f, 1f); // Màu nền đậm hơn, không trong suốt

            var input = rect.gameObject.AddComponent<InputField>();

            var placeholderRect = CreateChildRect(rect, "Placeholder");
            placeholderRect.offsetMin = new Vector2(16, 8);
            placeholderRect.offsetMax = new Vector2(-16, -8);
            var placeholder = placeholderRect.gameObject.AddComponent<Text>();
            placeholder.font = runtimeFont;
            placeholder.fontStyle = FontStyle.Italic;
            placeholder.fontSize = 22;
            placeholder.alignment = TextAnchor.MiddleLeft;
            placeholder.text = placeholderText;
            placeholder.color = new Color(0.7f, 0.7f, 0.7f, 1f); // Placeholder rõ hơn

            var textRect = CreateChildRect(rect, "Text");
            textRect.offsetMin = new Vector2(16, 8);
            textRect.offsetMax = new Vector2(-16, -8);
            var text = textRect.gameObject.AddComponent<Text>();
            text.font = runtimeFont;
            text.fontSize = 22;
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
            if (registerButton != null)
                registerButton.interactable = interactable;
            if (switchModeButton != null)
                switchModeButton.interactable = interactable;
            if (usernameField != null)
                usernameField.interactable = interactable;
            if (passwordField != null)
                passwordField.interactable = interactable;
            if (displayNameField != null)
                displayNameField.interactable = interactable;
        }

        void SetFeedback(string message)
        {
            if (feedbackLabel != null)
                feedbackLabel.text = message ?? string.Empty;
        }
    }
}
