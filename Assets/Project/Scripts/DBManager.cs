using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;


/* <summary>
 A static class for managing user data and authentication state in the game.
Provides access to user information such as nickname, email, scores, and level progress.
 Handles login status and logout functionality, as well as initialization of data from PlayerPrefs.
 </summary>*/

public static class DBManager 
{
    public static string nick;
    public static string email;

    public static int levelscore;
    public static int infinityscore;

    public static int colorsortLevels;
    public static int connectLevels;
    public static int pipesLevels;


    public static bool LoggedIn
    {
        get
        {
            return nick != null;
        }

    }

    public static void LoggedOut()
    {

     nick = null;
     PlayerPrefs.DeleteKey("Nick");
     PlayerPrefs.DeleteKey("Password");
     PlayerPrefs.DeleteKey("Email");
     PlayerPrefs.Save();

     Debug.Log("User logged out successfully");

    }

    public static void InitializeFromPrefs()
    {
        levelscore = PlayerPrefs.GetInt("LevelScore", 0);
        colorsortLevels = PlayerPrefs.GetInt("ColorsortLevels", 0);
        connectLevels = PlayerPrefs.GetInt("ConnectLevels", 0);
        pipesLevels = PlayerPrefs.GetInt("PipesLevels", 0);
    }

}
