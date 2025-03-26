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

            CurrentLevel = 1;

            LevelsConnect = new Dictionary<string, LevelData>();

            foreach (var item in _allLevelsconnect.Levels)
            {
                LevelsConnect[item.LevelName] = item;
            }

            LevelsColorSort = new Dictionary<string, LevelData>();


            foreach (var item in _allLevelscolorsort.Levels)
            {
                LevelsConnect[item.LevelName] = item;
            }


            LevelsPipes = new Dictionary<string, LevelData>();


            foreach (var item in _allLevelspipes.Levels)
            {
                LevelsConnect[item.LevelName] = item;
            }

        }
        #endregion

        #region UnlockLevel
        [HideInInspector]
        public int CurrentLevel;
        private string levelNameConnect = "Connect";
        private string levelNameColosort = "ColorSort";
        private string levelNamePipes = "Pipes";



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
            CurrentLevel++;

            
            string levelName = "Level"  + CurrentLevel.ToString();
            PlayerPrefs.SetInt(levelName + levelNameConnect, 1);
        }

        public void UnlockLevelConnectColorsort()
        {
            CurrentLevel++;


            string levelName = "Level" + CurrentLevel.ToString();
            PlayerPrefs.SetInt(levelName + levelNameColosort, 1);
        }

        public void UnlockLevelPipes()
        {
            CurrentLevel++;


            string levelName = "Level" + CurrentLevel.ToString();
            PlayerPrefs.SetInt(levelName + levelNameColosort, 1);
        }
        #endregion

        #region LEVEL_DATA
        [SerializeField]
        private LevelData DefaultLevel;

        [SerializeField]
        private LevelList _allLevelsconnect;

        [SerializeField]
        private LevelList _allLevelspipes;

        [SerializeField]
        private LevelList _allLevelscolorsort;

        private Dictionary<string, LevelData> LevelsConnect;

        private Dictionary<string, LevelData> LevelsColorSort;

        private Dictionary<string, LevelData> LevelsPipes;



        public LevelData GetLevelConnect()
        {
            string levelName = "Level" + CurrentLevel.ToString();
            if(LevelsConnect.ContainsKey(levelName))
            {
                return LevelsConnect[levelName];
            }
            return DefaultLevel;
        }

        public LevelData GetLevelColorSort()
        {
            string levelName = "Level" + CurrentLevel.ToString();
            if (LevelsConnect.ContainsKey(levelName))
            {
                return LevelsColorSort[levelName];
            }
            return DefaultLevel;
        }
        #endregion


        void ResetLevels()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("All levels reset");
        }

        #region SCENE_LOAD
        private const string MainMenu = "MainMenu";
        private const string Gameplay = "GameplayConnect";
        private const string GameplayColorsort = "GameplayColorsort";
        private const string GameplayPipes = "GameplayPipes";



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


    }
}
