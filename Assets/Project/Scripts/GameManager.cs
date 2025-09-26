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
    ///<summary> 
    ///Class for working with levels, unlocking them
    ///</summary>
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
            CurrentLevelNumberLinks = PlayerPrefs.GetInt("NumberLinksLevels", 1);
            CurrentLevelOneStroke = PlayerPrefs.GetInt("OneStrokeLevels", 1); 
            CurrenLevelPaint = PlayerPrefs.GetInt("PaintLevels", 1);         

            LevelsConnect = new Dictionary<string, LevelData>();
            if (_allLevelsconnect != null && _allLevelsconnect.Levels != null)
            {
                foreach (var item in _allLevelsconnect.Levels)
                    LevelsConnect[item.LevelName] = item;
            }

            LevelsColorSort = new Dictionary<string, LevelColorSort>();
            if (_allLevelscolorsort != null && _allLevelscolorsort.Levels != null)
            {
                foreach (var item in _allLevelscolorsort.Levels)
                    LevelsColorSort[item.LevelName] = item;
            }

            LevelsPipes = new Dictionary<string, LevelDataPipe>();
            if (_allLevelspipes != null && _allLevelspipes.LevelsPipes != null)
            {
                foreach (var item in _allLevelspipes.LevelsPipes)
                    LevelsPipes[item.LevelName] = item;
            }

            LevelsNumberLinks = new Dictionary<string, NumberLinkLevel>();
            if (_allLevelsNumberLinks != null && _allLevelsNumberLinks.Levels != null)
            {
                foreach (var item in _allLevelsNumberLinks.Levels)
                    LevelsNumberLinks[item.LevelName] = item;
            }

            LevelsOneStroke = new Dictionary<string, LevelOneStroke>();
            if (_allLevelsOneStorke != null && _allLevelsOneStorke.Levels != null)
            {
                foreach (var item in _allLevelsOneStorke.Levels)
                    LevelsOneStroke[item.LevelName] = item;
            }

            LevelsPaint = new Dictionary<string, PaintLevel>();
            if (_allLevelsPaint != null && _allLevelsPaint.Levels != null)
            {
                foreach (var item in _allLevelsPaint.Levels)
                    LevelsPaint[item.LevelName] = item;
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
        private string levelNameNumberLinks = "NumberLinks";
        private string levelNameOneStroke = "OneStroke";
        private string levelNamePaint = "Paint";



        [HideInInspector] public int CurrentLevelConnect;
        [HideInInspector] public int CurrentLevelColorsort;
        [HideInInspector] public int CurrentLevelPipes;
        [HideInInspector] public int CurrentLevelNumberLinks;
        [HideInInspector] public int CurrentLevelOneStroke;
        [HideInInspector] public int CurrenLevelPaint;

        public bool IsLevelUnlockedConnect(int level) =>
            PlayerPrefs.GetInt("Level" + level + levelNameConnect, level == 1 ? 1 : 0) == 1;

        public bool IsLevelUnlockedColorsort(int level) =>
            PlayerPrefs.GetInt("Level" + level + levelNameColosort, level == 1 ? 1 : 0) == 1;

        public bool IsLevelUnlockedPipes(int level) =>
            PlayerPrefs.GetInt("Level" + level + levelNamePipes, level == 1 ? 1 : 0) == 1;

        public bool IsLevelUnlockedNumberLinks(int level) =>
            PlayerPrefs.GetInt("Level" + level + levelNameNumberLinks, level == 1 ? 1 : 0) == 1;
        public bool IsLevelUnlockedOneStroke(int level) =>
            PlayerPrefs.GetInt("Level" + level + levelNameOneStroke, level == 1 ? 1 : 0) == 1;
        public bool IsLevelUnlockedPaint(int level) =>
                PlayerPrefs.GetInt("Level" + level + levelNamePaint, level == 1 ? 1 : 0) == 1;

        #region FOR_DAILY_GAME
        public void SetCurrentLevelConnect(int levelIndex) => CurrentLevelConnect = levelIndex;
        public void SetCurrentLevelColorSort(int levelIndex) => CurrentLevelColorsort = levelIndex;
        public void SetCurrentLevelPipes(int levelIndex) => CurrentLevelPipes = levelIndex;
        public void SetCurrentLevelNumberLinks(int levelIndex) => CurrentLevelNumberLinks = levelIndex;
        public void SetCurrentLevelOneStroke(int levelIndex) => CurrentLevelOneStroke = levelIndex;

        public void SetCurrentLevelPaint(int levelIndex) => CurrenLevelPaint = levelIndex;



        #endregion

        public void UnlockLevelConnect()
        {
            if (CurrentLevelConnect >= LevelsConnect.Count) return;

            CurrentLevelConnect++;
            string levelName = "Level" + CurrentLevelConnect;
            PlayerPrefs.SetInt(levelName + levelNameConnect, 1);
            DBManager.connectLevels = CurrentLevelConnect;
            DBManager.levelscore += 100;

            PlayerPrefs.SetInt("ConnectLevels", DBManager.connectLevels);
            PlayerPrefs.SetInt("LevelScore", DBManager.levelscore);
            PlayerPrefs.Save();

            if (DBManager.LoggedIn) StartCoroutine(UpdateServerData());
        }

        public void UnlockLevelColorsort()
        {
            if (CurrentLevelColorsort >= LevelsColorSort.Count) return;

            CurrentLevelColorsort++;
            string levelName = "Level" + CurrentLevelColorsort;
            PlayerPrefs.SetInt(levelName + levelNameColosort, 1);
            DBManager.colorsortLevels = CurrentLevelColorsort;
            DBManager.levelscore += 100;

            PlayerPrefs.SetInt("ColorsortLevels", DBManager.colorsortLevels);
            PlayerPrefs.SetInt("LevelScore", DBManager.levelscore);
            PlayerPrefs.Save();

            if (DBManager.LoggedIn) StartCoroutine(UpdateServerData());
        }

        public void UnlockLevelPipes()
        {
            if (CurrentLevelPipes >= LevelsPipes.Count) return;

            CurrentLevelPipes++;
            string levelName = "Level" + CurrentLevelPipes;
            PlayerPrefs.SetInt(levelName + levelNamePipes, 1);
            DBManager.pipesLevels = CurrentLevelPipes;
            DBManager.levelscore += 100;

            PlayerPrefs.SetInt("PipesLevels", DBManager.pipesLevels);
            PlayerPrefs.SetInt("LevelScore", DBManager.levelscore);
            PlayerPrefs.Save();

            if (DBManager.LoggedIn) StartCoroutine(UpdateServerData());
        }

        public void UnlockLevelNumberLinks()
        {
            if (CurrentLevelNumberLinks >= LevelsNumberLinks.Count) return;

            CurrentLevelNumberLinks++;
            string levelName = "Level" + CurrentLevelNumberLinks;
            PlayerPrefs.SetInt(levelName + levelNameNumberLinks, 1);
            DBManager.numberLinksLevels = CurrentLevelNumberLinks;
            DBManager.levelscore += 100;

            PlayerPrefs.SetInt("NumberLinksLevels", DBManager.numberLinksLevels);
            PlayerPrefs.SetInt("LevelScore", DBManager.levelscore);
            PlayerPrefs.Save();

            if (DBManager.LoggedIn) StartCoroutine(UpdateServerData());
        }
        public void UnlockLevelOneStroke()
        {
            if (CurrentLevelOneStroke >= LevelsOneStroke.Count) return;

            CurrentLevelOneStroke++;
            string levelName = "Level" + CurrentLevelOneStroke;
            PlayerPrefs.SetInt(levelName + levelNameOneStroke, 1);
            DBManager.oneStrokeLevels = CurrentLevelOneStroke;
            DBManager.levelscore += 100;

            PlayerPrefs.SetInt("OneStrokeLevels", DBManager.oneStrokeLevels);
            PlayerPrefs.SetInt("LevelScore", DBManager.levelscore);
            PlayerPrefs.Save();

            if (DBManager.LoggedIn) StartCoroutine(UpdateServerData());
        }

        public void UnlockLevelPaint()
        {
            if (CurrenLevelPaint >= LevelsPaint.Count) return;

            CurrenLevelPaint++;
            string levelName = "Level" + CurrenLevelPaint;
            PlayerPrefs.SetInt(levelName + levelNamePaint, 1);
            DBManager.paintLevels = CurrenLevelPaint;
            DBManager.levelscore += 100;

            PlayerPrefs.SetInt("PaintLevels", DBManager.paintLevels);
            PlayerPrefs.SetInt("LevelScore", DBManager.levelscore);
            PlayerPrefs.Save();

            if (DBManager.LoggedIn) StartCoroutine(UpdateServerData());
        }

        public IEnumerator UpdateServerData()
        {
            string nickToSend = DBManager.nick;
            if (string.IsNullOrEmpty(nickToSend))
            {
                nickToSend = PlayerPrefs.GetString("Nick", "");
                if (string.IsNullOrEmpty(nickToSend))
                {
                    Debug.LogWarning("[UpdateServerData] Nick is empty both in DBManager and PlayerPrefs! Server update aborted.");
                    yield break;
                }
            }

            Debug.Log("Starting server update...");
            Debug.Log($"[UpdateServerData] Sending nick: {nickToSend}, infinityscore={DBManager.infinityscore}");

            string url = "http://93.81.252.217/unity/update_score_and_levels.php";
            WWWForm form = new WWWForm();
            form.AddField("nick", nickToSend);
            form.AddField("levelscore", DBManager.levelscore);
            form.AddField("colorsortLevels", DBManager.colorsortLevels);
            form.AddField("connectLevels", DBManager.connectLevels);
            form.AddField("pipesLevels", DBManager.pipesLevels);
            form.AddField("numberLinksLevels", DBManager.numberLinksLevels);
            form.AddField("oneStrokeLevels", DBManager.oneStrokeLevels);
            form.AddField("paintLevels", DBManager.paintLevels);
            form.AddField("infinityscore", DBManager.infinityscore);

            using (UnityWebRequest www = UnityWebRequest.Post(url, form))
            {
                www.certificateHandler = new BypassCertificate();
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string response = www.downloadHandler.text;
                    Debug.Log($"[UpdateServerData] Server response: {response}");
                    try
                    {
                        UpdateResponse updateResponse = JsonConvert.DeserializeObject<UpdateResponse>(response);
                        if (updateResponse != null && updateResponse.Status == "0")
                            Debug.Log("UpdateServerData: Server data updated successfully");
                        else
                            Debug.LogWarning($"UpdateServerData: Failed - {updateResponse?.Message}");
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
            protected override bool ValidateCertificate(byte[] certificateData) => true;
        }
        #endregion

        #region LEVEL_DATA
        [SerializeField] private LevelData DefaultLevel;
        [SerializeField] private LevelColorSort DefaultLevelColorSort;
        [SerializeField] private LevelDataPipe DefaultLevelPipe;
        [SerializeField] private NumberLinkLevel DefaultLevelNumberLink;
        [SerializeField] private LevelOneStroke DefaultLevelOneStroke;
        [SerializeField] private PaintLevel DefaultLevelPaint;



        [SerializeField] private LevelList _allLevelsconnect;
        [SerializeField] private AllLevelsPipes _allLevelspipes;
        [SerializeField] private AllLevelsColorSort _allLevelscolorsort;
        [SerializeField] private AllLevelsNumberLink _allLevelsNumberLinks;
        [SerializeField] private AllLevelsOneStorke _allLevelsOneStorke;
        [SerializeField] private AllLevelsPaint _allLevelsPaint;



        private Dictionary<string, LevelData> LevelsConnect;
        private Dictionary<string, LevelColorSort> LevelsColorSort;
        private Dictionary<string, LevelDataPipe> LevelsPipes;
        private Dictionary<string, NumberLinkLevel> LevelsNumberLinks;
        private Dictionary<string, LevelOneStroke> LevelsOneStroke;
        private Dictionary<string, PaintLevel> LevelsPaint;



        public LevelData GetLevelConnect()
        {
            string levelName = "Level" + CurrentLevelConnect;
            if (LevelsConnect.ContainsKey(levelName)) return LevelsConnect[levelName];
            return DefaultLevel;
        }

        public LevelColorSort GetLevelColorSort()
        {
            string levelName = "Level" + CurrentLevelColorsort;
            if (LevelsColorSort.ContainsKey(levelName)) return LevelsColorSort[levelName];
            return DefaultLevelColorSort;
        }

        public LevelDataPipe GetLevelPipes()
        {
            string levelName = "Level" + CurrentLevelPipes;
            if (LevelsPipes.ContainsKey(levelName)) return LevelsPipes[levelName];
            return DefaultLevelPipe;
        }

        public NumberLinkLevel GetLevelNumberLinks()
        {
            string levelName = "Level" + CurrentLevelNumberLinks;
            if (LevelsNumberLinks.ContainsKey(levelName)) return LevelsNumberLinks[levelName];
            return DefaultLevelNumberLink;
        }

        public LevelOneStroke GetLevelOneStroke()
        {
            string levelName = "Level" + CurrentLevelOneStroke;
            if (LevelsOneStroke.ContainsKey(levelName)) return LevelsOneStroke[levelName];
            return DefaultLevelOneStroke;
        }

        public PaintLevel GetLevelPaint()
        {
            string levelName = "Level" + CurrenLevelPaint;
            if (LevelsPaint.ContainsKey(levelName)) return LevelsPaint[levelName];
            return DefaultLevelPaint;
        }
        #endregion

        public int GetRandomLevelIndexConnect() => Random.Range(1, LevelsConnect.Count + 1);
        public int GetRandomLevelIndexColorSort() => Random.Range(1, LevelsColorSort.Count + 1);
        public int GetRandomLevelIndexPipes() => Random.Range(1, LevelsPipes.Count + 1);
        public int GetRandomLevelIndexNumberLinks() => Random.Range(1, LevelsNumberLinks.Count + 1);
        public int GetRandomLevelIndexOneStroke() => Random.Range(1, LevelsOneStroke.Count + 1);
        public int GetRandomLevelIndexPaint() => Random.Range(1, LevelsPaint.Count + 1);



        #region SCENE_LOAD
        private const string MainMenu = "MainMenu";
        private const string Gameplay = "GameplayConnect";
        private const string GameplayColorsort = "GameplayColorsort";
        private const string GameplayPipes = "PipesGameplay";
        private const string GameplayNumberLinks = "NumberlinkGameplay";
        private const string OneStroke = "OneStroke";
        private const string Paint = "Paint";



        public void GoToMainMenu() => SceneManager.LoadScene(MainMenu);
        public void GoToGameplayConnect() => SceneManager.LoadScene(Gameplay);
        public void GoToGameplayColorSort() => SceneManager.LoadScene(GameplayColorsort);
        public void GoToGameplayPipes() => SceneManager.LoadScene(GameplayPipes);
        public void GoToGameplayNumberLinks() => SceneManager.LoadScene(GameplayNumberLinks);
        public void GoToGameplayOneStroke() => SceneManager.LoadScene(OneStroke);
        public void GoToGameplayPaint() => SceneManager.LoadScene(Paint);
        public void GoToDailyChallenge() => SceneManager.LoadScene("DailyChallengeScene");
        #endregion
    }
}
