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

            CurrentLevel = 1; 

            Levels = new Dictionary<string, LevelData>();

            foreach (var item in _allLevels.Levels)
            {
                Levels[item.LevelName] = item;
            }

        }
        #endregion

        #region UnlockLevel
        [HideInInspector]
        public int CurrentLevel;


        public bool IsLevelUnlocked(int level)
        {
            string levelName = "Level" + level.ToString();

            if (level == 1)
            {
                PlayerPrefs.SetInt(levelName, 1);
                return true;
            }
            if (PlayerPrefs.HasKey(levelName))
                {
                return PlayerPrefs.GetInt(levelName) == 1;
            }
            PlayerPrefs.SetInt(levelName, 0);
            return false;

        }

        public void UnlockLevel()
        {
            CurrentLevel++;

            
            string levelName = "Level"  + CurrentLevel.ToString();
            PlayerPrefs.SetInt(levelName, 1);
        }
        #endregion

        #region LEVEL_DATA
        [SerializeField]
        private LevelData DefaultLevel;

        [SerializeField]
        private LevelList _allLevels;

        private Dictionary<string, LevelData> Levels;

        public LevelData GetLevel()
        {
            string levelName = "Level" + CurrentLevel.ToString();
            if(Levels.ContainsKey(levelName))
            {
                return Levels[levelName];
            }
            return DefaultLevel;
        }
        #endregion

        #region SCENE_LOAD
        private const string MainMenu = "MainMenu";
        private const string Gameplay = "Gameplay";

        public void GoToMainMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(MainMenu);
        }

        public void GoToGameplay()
        {

            UnityEngine.SceneManagement.SceneManager.LoadScene(Gameplay);

        }
        #endregion


    }
}
