using System;
using UnityEngine;

namespace CrossingSimulator.Networking
{
    /// <summary>
    /// Service để gọi API Game Settings và Game Data
    /// </summary>
    public class GameDataService : MonoBehaviour
    {
        static GameDataService instance;

        public static GameDataService Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                var existing = FindObjectOfType<GameDataService>();
                if (existing != null)
                {
                    instance = existing;
                    return instance;
                }

                var go = new GameObject("GameDataService");
                instance = go.AddComponent<GameDataService>();
                DontDestroyOnLoad(go);
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
        }

        // ============ GAME SETTINGS ============

        /// <summary>
        /// POST /user/game/settings - Lưu game settings
        /// </summary>
        public void SaveGameSettings(GameSettings settings, Action<bool, string> onComplete)
        {
            var request = new GameSettingsRequest(settings);

            ApiService.Instance.PostJson(ApiPaths.GameSettings, request, response =>
            {
                if (response.Success)
                {
                    // Lưu local
                    SaveSettingsLocal(settings);
                    Debug.Log("[GameDataService] Settings saved successfully");
                    onComplete?.Invoke(true, "Settings saved");
                }
                else
                {
                    Debug.LogError($"[GameDataService] Failed to save settings: {response.Error}");
                    onComplete?.Invoke(false, response.Error ?? "Failed to save settings");
                }
            });
        }

        /// <summary>
        /// GET /user/game/settings - Lấy game settings
        /// </summary>
        public void GetGameSettings(Action<bool, GameSettings> onComplete)
        {
            ApiService.Instance.Get(ApiPaths.GameSettings, response =>
            {
                if (response.Success)
                {
                    if (response.TryGetEnvelope<GameSettingsResponse>(out var envelope) && envelope.data != null)
                    {
                        var settings = envelope.data.data;
                        SaveSettingsLocal(settings);
                        Debug.Log("[GameDataService] Settings loaded successfully");
                        onComplete?.Invoke(true, settings);
                    }
                    else
                    {
                        // Trả về default settings nếu parse fail
                        var defaultSettings = LoadSettingsLocal();
                        onComplete?.Invoke(true, defaultSettings);
                    }
                }
                else
                {
                    Debug.LogError($"[GameDataService] Failed to get settings: {response.Error}");
                    // Trả về local settings nếu API fail
                    var localSettings = LoadSettingsLocal();
                    onComplete?.Invoke(false, localSettings);
                }
            });
        }

        // ============ GAME DATA ============

        /// <summary>
        /// POST /user/game/data - Lưu game data (level progress)
        /// </summary>
        public void SaveGameData(GameData gameData, Action<bool, string> onComplete)
        {
            var request = new GameDataRequest(gameData);

            ApiService.Instance.PostJson(ApiPaths.GameData, request, response =>
            {
                if (response.Success)
                {
                    // Lưu local
                    SaveGameDataLocal(gameData);
                    Debug.Log("[GameDataService] Game data saved successfully");
                    onComplete?.Invoke(true, "Game data saved");
                }
                else
                {
                    Debug.LogError($"[GameDataService] Failed to save game data: {response.Error}");
                    onComplete?.Invoke(false, response.Error ?? "Failed to save game data");
                }
            });
        }

        /// <summary>
        /// GET /user/game/data - Lấy game data
        /// </summary>
        public void GetGameData(Action<bool, GameData> onComplete)
        {
            ApiService.Instance.Get(ApiPaths.GameData, response =>
            {
                if (response.Success)
                {
                    if (response.TryGetEnvelope<GameDataResponse>(out var envelope) && envelope.data != null)
                    {
                        var gameData = envelope.data.data;
                        SaveGameDataLocal(gameData);
                        Debug.Log("[GameDataService] Game data loaded successfully");
                        onComplete?.Invoke(true, gameData);
                    }
                    else
                    {
                        var defaultData = LoadGameDataLocal();
                        onComplete?.Invoke(true, defaultData);
                    }
                }
                else
                {
                    Debug.LogError($"[GameDataService] Failed to get game data: {response.Error}");
                    var localData = LoadGameDataLocal();
                    onComplete?.Invoke(false, localData);
                }
            });
        }

        // ============ LOCAL STORAGE ============

        const string SETTINGS_KEY = "GameSettings";
        const string GAME_DATA_KEY = "GameData";

        void SaveSettingsLocal(GameSettings settings)
        {
            var json = JsonUtility.ToJson(settings);
            PlayerPrefs.SetString(SETTINGS_KEY, json);
            PlayerPrefs.Save();
        }

        public GameSettings LoadSettingsLocal()
        {
            var json = PlayerPrefs.GetString(SETTINGS_KEY, "");
            if (string.IsNullOrEmpty(json))
            {
                return new GameSettings { sound = true };
            }
            try
            {
                return JsonUtility.FromJson<GameSettings>(json);
            }
            catch
            {
                return new GameSettings { sound = true };
            }
        }

        void SaveGameDataLocal(GameData data)
        {
            var json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(GAME_DATA_KEY, json);
            PlayerPrefs.Save();
        }

        public GameData LoadGameDataLocal()
        {
            var json = PlayerPrefs.GetString(GAME_DATA_KEY, "");
            if (string.IsNullOrEmpty(json))
            {
                return new GameData();
            }
            try
            {
                return JsonUtility.FromJson<GameData>(json);
            }
            catch
            {
                return new GameData();
            }
        }
    }
}
