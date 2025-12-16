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

    [Serializable]
    public class GameSettingsResponse
    {
        public GameSettings data;
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
        public int unlockLevel;
        public List<LevelProgress> levels;

        public GameData()
        {
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

    [Serializable]
    public class GameDataResponse
    {
        public GameData data;
    }
}
