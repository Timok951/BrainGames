using Connect.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Newtonsoft.Json; 
using Assets.Project.Scripts.Database; 

namespace Connect.Core
{

    ///<summarry> 
     ///Class for working with levels, unlocking them
     ///<sumarry>
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



            LevelsConnect = new Dictionary<string, LevelData>();
            if (_allLevelsconnect != null && _allLevelsconnect.Levels != null)
            {
                foreach (var item in _allLevelsconnect.Levels)
                {
                    LevelsConnect[item.LevelName] = item;
                }
            }
            else
            {
                Debug.LogError("_allLevelsconnect or its Levels is null");
            }

            LevelsColorSort = new Dictionary<string, LevelColorSort>();
            if (_allLevelscolorsort != null && _allLevelscolorsort.Levels != null)
            {
                foreach (var item in _allLevelscolorsort.Levels)
                {
                    LevelsColorSort[item.LevelName] = item;
                }
            }
            else
            {
                Debug.LogError("_allLevelscolorsort or its Levels is null");
            }

            LevelsPipes = new Dictionary<string, LevelDataPipe>();
            if (_allLevelspipes != null && _allLevelspipes.LevelsPipes != null)
            {
                foreach (var item in _allLevelspipes.LevelsPipes)
                {
                    LevelsPipes[item.LevelName] = item;
                }
            }
            else
            {
                Debug.LogError("_allLevelspipes or its LevelsPipes is null");
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
                Debug.Log($"SyncWithServerOnStart: Attempting login with nick={nick}");
                yield return StartCoroutine(MainMenuManager.Instance.Login(nick, password));
                if (DBManager.LoggedIn)
                {
                    Debug.Log("SyncWithServerOnStart: Login successful, data synced");
                }
                else
                {
                    Debug.LogWarning("SyncWithServerOnStart: Login failed");
                }
            }
            else
            {
                Debug.LogWarning("SyncWithServerOnStart: Nick or password is empty");
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
            if (CurrentLevelConnect >= LevelsConnect.Count)
            {
                Debug.LogWarning("UnlockLevelConnect: Max level reached");
                return;
            }

            CurrentLevelConnect++;
            string levelName = "Level" + CurrentLevelConnect.ToString();
            PlayerPrefs.SetInt(levelName + levelNameConnect, 1);
            DBManager.connectLevels = CurrentLevelConnect;
            DBManager.levelscore += 100;

            PlayerPrefs.SetInt("ConnectLevels", DBManager.connectLevels);
            PlayerPrefs.SetInt("LevelScore", DBManager.levelscore);
            PlayerPrefs.Save();

            Debug.Log($"Unlocked level: {levelName + levelNameConnect}, LevelScore: {DBManager.levelscore}");

            if (DBManager.LoggedIn)
            {
                StartCoroutine(UpdateServerData());
            }
        }

        public void UnlockLevelColorsort()
        {
            if (CurrentLevelColorsort >= LevelsColorSort.Count)
            {
                Debug.LogWarning("UnlockLevelColorsort: Max level reached");
                return;
            }

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
            if (CurrentLevelPipes >= LevelsPipes.Count)
            {
                Debug.LogWarning("UnlockLevelPipes: Max level reached");
                return;
            }

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
            form.AddField("nick", DBManager.nick ?? "");
            form.AddField("levelscore", DBManager.levelscore);
            form.AddField("colorsortLevels", DBManager.colorsortLevels);
            form.AddField("connectLevels", DBManager.connectLevels);
            form.AddField("pipesLevels", DBManager.pipesLevels);
            form.AddField("infinityscore", DBManager.infinityscore);

            Debug.Log($"Sending to server: nick={DBManager.nick ?? "null"}, levelscore={DBManager.levelscore}, " +
                      $"colorsortLevels={DBManager.colorsortLevels}, connectLevels={DBManager.connectLevels}, " +
                      $"pipesLevels={DBManager.pipesLevels}, infinityscore={DBManager.infinityscore}");

            using (UnityWebRequest www = UnityWebRequest.Post(url, form))
            {
                www.certificateHandler = new BypassCertificate();
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string response = www.downloadHandler.text;
                    Debug.Log($"UpdateServerData: Response received: {response}");

                    if (string.IsNullOrEmpty(response))
                    {
                        Debug.LogError("UpdateServerData: Server returned empty response");
                        yield break;
                    }

                    try
                    {
                        UpdateResponse updateResponse = JsonConvert.DeserializeObject<UpdateResponse>(response);
                        if (updateResponse == null)
                        {
                            Debug.LogError("UpdateServerData: Deserialization returned null");
                            yield break;
                        }

                        Debug.Log($"UpdateServerData: Status: {updateResponse.Status}, Message: {updateResponse.Message}");

                        if (updateResponse.Status == "0")
                        {
                            Debug.Log("UpdateServerData: Server data updated successfully");
                        }
                        else
                        {
                            Debug.LogWarning($"UpdateServerData: Server reported failure - {updateResponse.Message}");
                        }
                    }
                    catch (JsonException e)
                    {
                        Debug.LogError($"UpdateServerData: JSON parse error: {e.Message}. Raw response: {response}");
                    }
                }
                else
                {
                    Debug.LogError($"UpdateServerData: Connection error: {www.error}");
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
        [SerializeField] private LevelData DefaultLevel;
        [SerializeField] private LevelColorSort DefaultLevelColorSort;
        [SerializeField] private LevelDataPipe DefaultLevelPipe;

        [SerializeField] private LevelList _allLevelsconnect;
        [SerializeField] private AllLevelsPipes _allLevelspipes;
        [SerializeField] private AllLevelsColorSort _allLevelscolorsort;

        private Dictionary<string, LevelData> LevelsConnect;
        private Dictionary<string, LevelColorSort> LevelsColorSort;
        private Dictionary<string, LevelDataPipe> LevelsPipes;

        public LevelData GetLevelConnect()
        {
            string levelName = "Level" + CurrentLevelConnect.ToString();
            if (LevelsConnect.ContainsKey(levelName))
            {
                return LevelsConnect[levelName];
            }
            if (DefaultLevel != null)
            {
                Debug.LogWarning($"Level {levelName} not found, returning DefaultLevel");
                return DefaultLevel;
            }
            Debug.LogError($"Level {levelName} not found and DefaultLevel is null");
            return null;
        }

        public LevelColorSort GetLevelColorSort()
        {
            string levelName = "Level" + CurrentLevelColorsort.ToString();
            if (LevelsColorSort.ContainsKey(levelName))
            {
                return LevelsColorSort[levelName];
            }
            if (DefaultLevelColorSort != null)
            {
                Debug.LogWarning($"Level {levelName} not found, returning DefaultLevelColorSort");
                return DefaultLevelColorSort;
            }
            Debug.LogError($"Level {levelName} not found and DefaultLevelColorSort is null");
            return null;
        }

        public LevelDataPipe GetLevelPipes()
        {
            string levelName = "Level" + CurrentLevelPipes.ToString();
            if (LevelsPipes.ContainsKey(levelName))
            {
                return LevelsPipes[levelName];
            }
            if (DefaultLevelPipe != null)
            {
                Debug.LogWarning($"Level {levelName} not found, returning DefaultLevelPipe");
                return DefaultLevelPipe;
            }
            Debug.LogError($"Level {levelName} not found and DefaultLevelPipe is null");
            return null;
        }
        #endregion

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
    }
}