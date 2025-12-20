using System;
using System.Collections.Generic;

namespace CrossingSimulator.Networking
{
    // ============ GAME SETTINGS ============

    [Serializable]
    public class GameSettings
    {
        public bool sound = true;
    }

    [Serializable]
    public class GameSettingsRequest
    {
        public GameSettings data;

        public GameSettingsRequest(GameSettings settings)
        {
            data = settings;
        }
    }

    /// <summary>
    /// Response từ API /user/game/settings
    /// Format: { status, data: { uid, settings, metadata } }
    /// </summary>
    [Serializable]
    public class GameSettingsResponse
    {
        public string uid;
        public GameSettings settings;
        public SettingsMetadata metadata;
    }

    [Serializable]
    public class SettingsMetadata
    {
        public long updatedAt;
        public string updatedAtIso;
    }

    // ============ GAME DATA (Level Progress) ============

    [Serializable]
    public class LevelProgress
    {
        public string map;
        public string score;
        public int star;
        public bool unlock;

        public LevelProgress() { }

        public LevelProgress(string mapName, int scoreValue, int starCount, bool isUnlocked)
        {
            map = mapName;
            score = scoreValue.ToString();
            star = starCount;
            unlock = isUnlocked;
        }
    }

    [Serializable]
    public class GameData
    {
        public int unlockLevel = 1; // Default: level 1 mở
        public List<LevelProgress> levels;

        public GameData()
        {
            unlockLevel = 1;
            levels = new List<LevelProgress>();
        }
    }

    [Serializable]
    public class GameDataRequest
    {
        public GameData data;

        public GameDataRequest(GameData gameData)
        {
            data = gameData;
        }
    }

    /// <summary>
    /// Response từ API /user/game/data
    /// Format: { status, data: { uid, gameData, metadata } }
    /// </summary>
    [Serializable]
    public class GameDataResponse
    {
        public string uid;
        public GameData gameData;
        public GameDataMetadata metadata;
    }

    [Serializable]
    public class GameDataMetadata
    {
        public long updatedAt;
        public string updatedAtIso;
    }
}
