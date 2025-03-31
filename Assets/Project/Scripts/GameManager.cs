using Connect.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Connect.Core
{
    public class GameManager : MonoBehaviour
    {

        public static GameManager Instance;
        #region START_METHOD
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                Debug.Log("Gameobject was destroyed");
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }


        private void Init()
        {
            CurrentLevelConnect = PlayerPrefs.GetInt("ConnectLevels", 1);
            CurrentLevelColorsort = PlayerPrefs.GetInt("ColorsortLevels", 1);
            CurrentLevelPipes = PlayerPrefs.GetInt("PipesLevels", 1);

           
            DBManager.levelscore = PlayerPrefs.GetInt("LevelScore", 0);

            LevelsConnect = new Dictionary<string, LevelData>();
            foreach (var item in _allLevelsconnect.Levels)
            {
                LevelsConnect[item.LevelName] = item;
            }

            LevelsColorSort = new Dictionary<string, LevelColorSort>();
            foreach (var item in _allLevelscolorsort.Levels)
            {
                LevelsColorSort[item.LevelName] = item;
            }

            LevelsPipes = new Dictionary<string, LevelDataPipe>();
            foreach (var item in _allLevelspipes.LevelsPipes)
            {
                LevelsPipes[item.LevelName] = item;
            }

            if (DBManager.LoggedIn && !string.IsNullOrEmpty(PlayerPrefs.GetString("Nick", "")))
            {
                StartCoroutine(SyncWithServerOnStart());
            }
        }

        private IEnumerator SyncWithServerOnStart()
        {
            string nick = PlayerPrefs.GetString("Nick", "");
            string password = PlayerPrefs.GetString("Password", "");

            if (!string.IsNullOrEmpty(nick) && !string.IsNullOrEmpty(password))
            {
                yield return StartCoroutine(MainMenuManager.Instance.Login(nick, password));
            }
        }
        #endregion

        #region UnlockLevel
        private string levelNameConnect = "Connect";
        private string levelNameColosort = "Colorsort";
        private string levelNamePipes = "Pipes";
        [HideInInspector] public int CurrentLevelConnect;
        [HideInInspector] public int CurrentLevelColorsort;
        [HideInInspector] public int CurrentLevelPipes;


        public bool IsLevelUnlockedConnect(int level)
        {
            string levelName = "Level" + level.ToString();
            return PlayerPrefs.GetInt(levelName + levelNameConnect, level == 1 ? 1 : 0) == 1;
        }

        public bool IsLevelUnlockedColorsort(int level)
        {
            string levelName = "Level" + level.ToString();
            return PlayerPrefs.GetInt(levelName + levelNameColosort, level == 1 ? 1 : 0) == 1;
        }

        public bool IsLevelUnlockedPipes(int level)
        {
            string levelName = "Level" + level.ToString();
            return PlayerPrefs.GetInt(levelName + levelNamePipes, level == 1 ? 1 : 0) == 1;
        }

        #region FOR_DAILY_GAME
        public void SetCurrentLevelConnect(int levelIndex)
        {
            CurrentLevelConnect = levelIndex;
            Debug.Log($"Set CurrentLevelConnect to {CurrentLevelConnect}");
        }

        public void SetCurrentLevelColorSort(int levelIndex)
        {
            CurrentLevelColorsort = levelIndex;
            Debug.Log($"Set CurrentLevelColorsort to {CurrentLevelColorsort}");
        }

        public void SetCurrentLevelPipes(int levelIndex)
        {
            CurrentLevelPipes = levelIndex;
            Debug.Log($"Set CurrentLevelPipes to {CurrentLevelPipes}");
        }
        #endregion

        public void UnlockLevelConnect()
        {
            CurrentLevelConnect++;
            string levelName = "Level" + CurrentLevelConnect.ToString();
            PlayerPrefs.SetInt(levelName + levelNameConnect, 1);
            DBManager.connectLevels = CurrentLevelConnect;
            DBManager.levelscore += 100;

            PlayerPrefs.SetInt("ConnectLevels", DBManager.connectLevels);
            PlayerPrefs.SetInt("LevelScore", DBManager.levelscore);
            PlayerPrefs.Save();

            if (DBManager.LoggedIn)
            {
                StartCoroutine(UpdateServerData());
            }
        }

        public void UnlockLevelColorsort()
        {
            CurrentLevelColorsort++;
            string levelName = "Level" + CurrentLevelColorsort.ToString();
            PlayerPrefs.SetInt(levelName + levelNameColosort, 1);
            DBManager.colorsortLevels = CurrentLevelColorsort;
            DBManager.levelscore += 100;

            PlayerPrefs.SetInt("ColorsortLevels", DBManager.colorsortLevels);
            PlayerPrefs.SetInt("LevelScore", DBManager.levelscore);
            PlayerPrefs.Save();
            Debug.Log($"Unlocked level: {levelName + levelNameColosort}");

            if (DBManager.LoggedIn)
            {
                StartCoroutine(UpdateServerData());
            }
        }

        public void UnlockLevelPipes()
        {
            Debug.Log($"Before unlocking: CurrentLevelPipes = {CurrentLevelPipes}");
            CurrentLevelPipes++;
            string levelName = "Level" + CurrentLevelPipes.ToString();
            PlayerPrefs.SetInt(levelName + levelNamePipes, 1);
            DBManager.pipesLevels = CurrentLevelPipes;
            DBManager.levelscore += 100;

            PlayerPrefs.SetInt("PipesLevels", DBManager.pipesLevels);
            PlayerPrefs.SetInt("LevelScore", DBManager.levelscore);
            PlayerPrefs.Save();
            Debug.Log($"Level {CurrentLevelPipes} unlocked. LevelScore: {DBManager.levelscore}");

            if (DBManager.LoggedIn)
            {
                StartCoroutine(UpdateServerData());
            }
        }

        public IEnumerator UpdateServerData()
        {
string url = "http://93.81.252.217/unity/update_score_and_levels.php";
    WWWForm form = new WWWForm();
    form.AddField("nick", DBManager.nick);
    form.AddField("levelscore", DBManager.levelscore);
    form.AddField("colorsortLevels", DBManager.colorsortLevels);
    form.AddField("connectLevels", DBManager.connectLevels);
    form.AddField("pipesLevels", DBManager.pipesLevels);
    form.AddField("infinityscore", DBManager.infinityscore);

    Debug.Log($"Sending to server: nick={DBManager.nick}, levelscore={DBManager.levelscore}, " +
              $"colorsortLevels={DBManager.colorsortLevels}, connectLevels={DBManager.connectLevels}, " +
              $"pipesLevels={DBManager.pipesLevels}, infinityscore={DBManager.infinityscore}");

    using (UnityWebRequest www = UnityWebRequest.Post(url, form))
    {
        www.certificateHandler = new BypassCertificate();
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Server updated: " + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error updating server: " + www.error);
        }
    }
        }

        private class BypassCertificate : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData)
            {
                return true; 
            }
        }
        #endregion

        #region LEVEL_DATA
        [SerializeField]
        private LevelData DefaultLevel;

        [SerializeField]
        private LevelColorSort DefaultLevelColorSort;

        [SerializeField]
        private LevelDataPipe DefaultLevelPipe;

        [SerializeField]
        private LevelList _allLevelsconnect;

        [SerializeField]
        private AllLevelsPipes _allLevelspipes;

        [SerializeField]
        private AllLevelsColorSort _allLevelscolorsort;

        private Dictionary<string, LevelData> LevelsConnect;

        private Dictionary<string, LevelColorSort> LevelsColorSort;

        private Dictionary<string, LevelDataPipe> LevelsPipes;



        public LevelData GetLevelConnect()
        {
            string levelName = "Level" + CurrentLevelConnect.ToString();
            if(LevelsConnect.ContainsKey(levelName))
            {
                return LevelsConnect[levelName];
            }
            return DefaultLevel;
        }

        public LevelColorSort GetLevelColorSort()
        {
            string levelName = "Level" + CurrentLevelColorsort.ToString();
            if (LevelsColorSort.ContainsKey(levelName))
            {
                return LevelsColorSort[levelName];
            }
            return DefaultLevelColorSort;
        }

        public LevelDataPipe GetLevelPipes()
        {
            string levelName = "Level" + CurrentLevelPipes.ToString();
            if (LevelsPipes.ContainsKey(levelName))
            {
                return LevelsPipes[levelName];
            }
            return DefaultLevelPipe;
        }
        #endregion

        //Getting random levels
        public int GetRandomLevelIndexConnect()
        {
            int maxLevel = LevelsConnect.Count; 
            return Random.Range(1, maxLevel + 1);
        }

        public int GetRandomLevelIndexColorSort()
        {
            int maxLevel = LevelsColorSort.Count;
            return Random.Range(1, maxLevel + 1);
        }

        public int GetRandomLevelIndexPipes()
        {
            int maxLevel = LevelsPipes.Count;
            return Random.Range(1, maxLevel + 1);
        }

        #region SCENE_LOAD
        private const string MainMenu = "MainMenu";
        private const string Gameplay = "GameplayConnect";
        private const string GameplayColorsort = "GameplayColorsort";
        private const string GameplayPipes = "PipesGameplay";



        public void GoToMainMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(MainMenu);
        }

        public void GoToGameplayConnect()
        {

            UnityEngine.SceneManagement.SceneManager.LoadScene(Gameplay);

        }

        public void GoToGameplayColorSort()
        {

            UnityEngine.SceneManagement.SceneManager.LoadScene(GameplayColorsort);

        }

        public void GoToGameplayPipes()
        {

            UnityEngine.SceneManagement.SceneManager.LoadScene(GameplayPipes);

        }

        public void GoToDailyChallenge()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("DailyChallengeScene");
        }


        #endregion


        #region UPDATE_METHODS
        void ResetLevels()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("All levels reset");
        }
        #endregion
    }
}
