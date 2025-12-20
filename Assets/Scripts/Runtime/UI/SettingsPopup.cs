using UnityEngine;
using UnityEngine.UI;
using CrossingSimulator.Networking;

namespace Runtime.UI
{
    /// <summary>
    /// Settings popup - hiển thị overlay với các tùy chọn cài đặt game.
    /// Được gọi từ LevelSelectionManager.
    /// </summary>
    public class SettingsPopup : MonoBehaviour
    {
        [Header("Popup Container")]
        [SerializeField] private GameObject popupPanel;
        [SerializeField] private Image overlayBackground;

        [Header("Sound")]
        [SerializeField] private Toggle soundToggle;
        [SerializeField] private Image soundOnIcon;
        [SerializeField] private Image soundOffIcon;

        [Header("Buttons")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button saveButton;

        [Header("Loading")]
        [SerializeField] private GameObject loadingIndicator;

        private GameSettings currentSettings;
        private bool isInitialized;

        void Awake()
        {
            SetupListeners();
            Hide();
        }

        void SetupListeners()
        {
            if (soundToggle != null)
                soundToggle.onValueChanged.AddListener(OnSoundToggleChanged);

            if (closeButton != null)
                closeButton.onClick.AddListener(OnCloseClicked);

            if (saveButton != null)
                saveButton.onClick.AddListener(OnSaveClicked);

            // Click overlay để đóng popup
            if (overlayBackground != null)
            {
                var overlayButton = overlayBackground.gameObject.GetComponent<Button>();
                if (overlayButton == null)
                    overlayButton = overlayBackground.gameObject.AddComponent<Button>();
                overlayButton.onClick.AddListener(OnCloseClicked);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            if (popupPanel != null)
                popupPanel.SetActive(true);

            LoadSettings();
        }

        public void Hide()
        {
            if (popupPanel != null)
                popupPanel.SetActive(false);
            gameObject.SetActive(false);
        }

        void LoadSettings()
        {
            ShowLoading(true);

            GameDataService.Instance.GetGameSettings((success, settings) =>
            {
                ShowLoading(false);
                currentSettings = settings ?? new GameSettings { sound = true };
                UpdateUI();
                isInitialized = true;
            });
        }

        void UpdateUI()
        {
            if (soundToggle != null)
                soundToggle.isOn = currentSettings.sound;

            UpdateSoundIcons(currentSettings.sound);
        }

        void OnSoundToggleChanged(bool isOn)
        {
            if (!isInitialized) return;

            currentSettings.sound = isOn;
            UpdateSoundIcons(isOn);

            // Apply sound setting immediately
            AudioListener.volume = isOn ? 1f : 0f;
        }

        void UpdateSoundIcons(bool soundOn)
        {
            if (soundOnIcon != null)
                soundOnIcon.gameObject.SetActive(soundOn);

            if (soundOffIcon != null)
                soundOffIcon.gameObject.SetActive(!soundOn);
        }

        void OnSaveClicked()
        {
            ShowLoading(true);

            GameDataService.Instance.SaveGameSettings(currentSettings, (success, message) =>
            {
                ShowLoading(false);

                if (success)
                    Debug.Log("[SettingsPopup] Saved successfully");
                else
                    Debug.LogError($"[SettingsPopup] Save failed: {message}");

                Hide();
            });
        }

        void OnCloseClicked()
        {
            // Auto save khi đóng
            if (isInitialized && currentSettings != null)
            {
                GameDataService.Instance.SaveGameSettings(currentSettings, (success, message) =>
                {
                    if (!success)
                        Debug.LogWarning($"[SettingsPopup] Auto-save failed: {message}");
                });
            }

            Hide();
        }

        void ShowLoading(bool show)
        {
            if (loadingIndicator != null)
                loadingIndicator.SetActive(show);
        }

        void OnDestroy()
        {
            if (soundToggle != null)
                soundToggle.onValueChanged.RemoveListener(OnSoundToggleChanged);

            if (closeButton != null)
                closeButton.onClick.RemoveListener(OnCloseClicked);

            if (saveButton != null)
                saveButton.onClick.RemoveListener(OnSaveClicked);
        }
    }
}
