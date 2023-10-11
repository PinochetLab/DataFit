using UnityEngine;

public static class ProgressStorage
{
    private const string prefix = "level_";
    private const string nextLevel = "next_level";

    public static bool IsLevelOpened(int levelIndex)
    {
        if (levelIndex == 0) return true;
        return PlayerPrefs.GetInt(prefix + levelIndex, 0) > 0;
    }

    public static void PassLevel(int levelIndex)
    {
        PlayerPrefs.SetInt(prefix + levelIndex, 1);
        PlayerPrefs.SetInt(prefix + (levelIndex + 1), 1);
        if (levelIndex + 1 > NextLevel())
        {
            PlayerPrefs.SetInt(nextLevel, 0);
        }
    }

    public static int NextLevel()
    {
        var n = PlayerPrefs.GetInt(nextLevel, 0);
        if (n >= Level.Levels.Count) n = 0;
        return n;
    }
}