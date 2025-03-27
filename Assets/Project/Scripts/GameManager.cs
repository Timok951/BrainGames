using Connect.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connect.Core
{
    public class GameManager : MonoBehaviour
    {

        public static GameManager Instance;
        #region START_METHOD
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Init();
                DontDestroyOnLoad(gameObject);
                return;
            }
            else
            {
                Destroy(gameObject);
            }
        }


        private void Init()
        {
            //ResetLevels();

            CurrentLevelConnect = 1;
            CurrentLevelColorsort = 1;
            CurrentLevelPipes = 1;

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

            if (level == 1)
            {
                PlayerPrefs.SetInt(levelName + levelNameConnect, 1);
                return true;
            }
            if (PlayerPrefs.HasKey(levelName + levelNameConnect))
                {
                return PlayerPrefs.GetInt(levelName + levelNameConnect) == 1;
            }
            PlayerPrefs.SetInt(levelName, 0);
            return false;

        }


        public bool IsLevelUnlockedColorsort(int level)
        {
            string levelName = "Level" + level.ToString();

            if (level == 1)
            {
                PlayerPrefs.SetInt(levelName + levelNameColosort, 1);
                return true;
            }
            if (PlayerPrefs.HasKey(levelName + levelNameColosort))
            {
                return PlayerPrefs.GetInt(levelName + levelNameColosort) == 1;
            }
            PlayerPrefs.SetInt(levelName, 0);
            return false;

        }


        public bool IsLevelUnlockedPipes(int level)
        {
            string levelName = "Level" + level.ToString();

            if (level == 1)
            {
                PlayerPrefs.SetInt(levelName + levelNamePipes, 1);
                return true;
            }
            if (PlayerPrefs.HasKey(levelName + levelNamePipes))
            {
                return PlayerPrefs.GetInt(levelName + levelNamePipes) == 1;
            }
            PlayerPrefs.SetInt(levelName, 0);
            return false;

        }

        public void UnlockLevelConnect()
        {
            CurrentLevelConnect++;

            
            string levelName = "Level"  + CurrentLevelConnect.ToString();
            PlayerPrefs.SetInt(levelName + levelNameConnect, 1);
        }

        public void UnlockLevelConnectColorsort()
        {
            CurrentLevelColorsort++;
            string levelName = "Level" + CurrentLevelColorsort.ToString(); 
            PlayerPrefs.SetInt(levelName + levelNameColosort, 1);
            Debug.Log($"Unlocked level: {levelName + levelNameColosort}"); 
        }

        public void UnlockLevelPipes()
        {
            CurrentLevelPipes++;


            string levelName = "Level" + CurrentLevelPipes.ToString();
            PlayerPrefs.SetInt(levelName + levelNamePipes, 1);
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
