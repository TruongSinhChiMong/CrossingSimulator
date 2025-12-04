using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CrossingSimulator.Networking;

namespace Runtime.UI
{
    public class SettingsManager : MonoBehaviour
    {
        [Header("Sound")]
        [SerializeField] private Toggle soundToggle;
        [SerializeField] private Image soundOnIcon;
        [SerializeField] private Image soundOffIcon;

        [Header("Buttons")]
        [SerializeField] private Button backButton;
        [SerializeField] private Button saveButton;

        [Header("Loading")]
        [SerializeField] private GameObject loadingIndicator;

        private GameSettings currentSettings;

        void Start()
        {
            SetupListeners();
            LoadSettings();
        }

        void SetupListeners()
        {
            if (soundToggle != null)
                soundToggle.onValueChanged.AddListener(OnSoundToggleChanged);

            if (backButton != null)
                backButton.onClick.AddListener(OnBackClicked);

            if (saveButton != null)
                saveButton.onClick.AddListener(OnSaveClicked);
        }

        void LoadSettings()
        {
            ShowLoading(true);

            GameDataService.Instance.GetGameSettings((success, settings) =>
            {
                ShowLoading(false);
                currentSettings = settings ?? new GameSettings { sound = true };
                UpdateUI();
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
                {
                    Debug.Log("[Settings] Saved successfully");
                    GoBack();
                }
                else
                {
                    Debug.LogError($"[Settings] Save failed: {message}");
                    // Vẫn quay lại, settings đã lưu local
                    GoBack();
                }
            });
        }

        void OnBackClicked()
        {
            // Lưu settings trước khi quay lại
            GameDataService.Instance.SaveGameSettings(currentSettings, (success, message) =>
            {
                GoBack();
            });
        }

        void GoBack()
        {
            SceneManager.LoadScene("LevelSelection");
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

            if (backButton != null)
                backButton.onClick.RemoveListener(OnBackClicked);

            if (saveButton != null)
                saveButton.onClick.RemoveListener(OnSaveClicked);
        }
    }
}
