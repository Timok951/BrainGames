using UnityEngine;

/// <summary>
/// A static class for managing user data and authentication state in the game.
/// Provides access to user information such as nickname, email, scores, and level progress.
/// Handles login status and logout functionality, as well as initialization of data from PlayerPrefs.
/// </summary>
public static class DBManager
{
    public static string nick;
    public static string email;

    public static int levelscore;
    public static int infinityscore;

    public static int colorsortLevels;
    public static int connectLevels;
    public static int pipesLevels;
    public static int numberLinksLevels; 

    public static bool LoggedIn => !string.IsNullOrEmpty(nick);

    public static void LoggedOut()
    {
        nick = null;
        PlayerPrefs.DeleteKey("Nick");
        PlayerPrefs.DeleteKey("Password");
        PlayerPrefs.DeleteKey("Email");

        PlayerPrefs.Save();
        Debug.Log("User logged out successfully");
    }

    /// <summary>
    /// Load progress & scores from PlayerPrefs
    /// </summary>
    public static void InitializeFromPrefs()
    {
        levelscore = PlayerPrefs.GetInt("LevelScore", 0);
        infinityscore = PlayerPrefs.GetInt("InfinityScore", 0);

        colorsortLevels = PlayerPrefs.GetInt("ColorsortLevels", 0);
        connectLevels = PlayerPrefs.GetInt("ConnectLevels", 0);
        pipesLevels = PlayerPrefs.GetInt("PipesLevels", 0);
        numberLinksLevels = PlayerPrefs.GetInt("NumberlinkLevels", 0);
    }

    /// <summary>
    /// Save progress for a specific game mode
    /// </summary>
    public static void SaveProgress(string mode, int level)
    {
        switch (mode)
        {
            case "Colorsort":
                colorsortLevels = Mathf.Max(colorsortLevels, level);
                PlayerPrefs.SetInt("ColorsortLevels", colorsortLevels);
                break;
            case "Connect":
                connectLevels = Mathf.Max(connectLevels, level);
                PlayerPrefs.SetInt("ConnectLevels", connectLevels);
                break;
            case "Pipes":
                pipesLevels = Mathf.Max(pipesLevels, level);
                PlayerPrefs.SetInt("PipesLevels", pipesLevels);
                break;
            case "Numberlink":
                numberLinksLevels = Mathf.Max(numberLinksLevels, level);
                PlayerPrefs.SetInt("NumberlinkLevels", numberLinksLevels);
                break;
        }

        PlayerPrefs.Save();
    }
}
