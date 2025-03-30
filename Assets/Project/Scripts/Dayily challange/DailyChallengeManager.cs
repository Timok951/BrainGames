using Connect.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Connect.Core
{
    public class DailyChallengeManager : MonoBehaviour
    {
        #region variables
        public static DailyChallengeManager Instance;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private TMP_Text _modeText;
        [SerializeField] private TMP_Text _winText;

        [SerializeField] private GameObject _challengeUI;

        private float _timer;
        private bool _isTimerRunning;
        private int _CurrentModeIndex;
        private bool _hasChallengeFinished;


        private int _pipesLevel;
        private int _colorSortLevel;
        private int _connectLevel;

        private const string DAILY_SCORE_KEY = "DailyChallengeScore";

        private List<string> _modSequence = new List<string>()
        {
            "PipesGameplay",
            "GameplayColorSort",
            "GameplayConnect"
        };

        private const int MAX_DAILY_SCORE = 5000; 
        private const float MAX_TIME = 180f;

        private const string LastDailyChallengeDateKey = "LastDailyChallengeDate";
        private const string CHALLENGE_AVAILABLE = "DailyChallengeAvailable";
        #endregion

        #region START_METHODS
        private void Awake()
        {
            Instance = this;
            _winText.gameObject.SetActive(false);
            _hasChallengeFinished = false;
            _CurrentModeIndex = 0;
            _timer = 0f;
            _isTimerRunning = false;
            _pipesLevel = GameManager.Instance.GetRandomLevelIndexPipes();
            _colorSortLevel = GameManager.Instance.GetRandomLevelIndexColorSort();
            _connectLevel = GameManager.Instance.GetRandomLevelIndexConnect();

            if (_challengeUI != null)
            {
                _challengeUI.SetActive(true);
            }
            else
            {
                Debug.LogError("_challengeUI is not assigned!");
            }

            if (_timerText == null) Debug.LogError("_timerText is not assigned!");
            if (_modeText == null) Debug.LogError("_modeText is not assigned!");
            if (_winText == null) Debug.LogError("_winText is not assigned!");

        }
        private void Start()
        {
            //ResetChallenge();
            StartChallenge();
        }

        private void StartChallenge()
        {
            _isTimerRunning = true;
            LoadCurrentMode();

        }
        private void LoadCurrentMode()
        {
            if (_CurrentModeIndex < _modSequence.Count)
            {
                switch (_modSequence[_CurrentModeIndex])
                {
                    case "PipesGameplay":
                        GameManager.Instance.SetCurrentLevelPipes(_pipesLevel);
                        break;
                    case "GameplayColorSort":
                        GameManager.Instance.SetCurrentLevelColorSort(_colorSortLevel);
                        break;
                    case "GameplayConnect":
                        GameManager.Instance.SetCurrentLevelConnect(_connectLevel);
                        break;
                }
                SceneManager.LoadScene(_modSequence[_CurrentModeIndex], LoadSceneMode.Additive);
                UpdateModeText();
            }
        }

        private void UpdateModeText()
        {
            _modeText.text = $"Mode {_CurrentModeIndex +1}/3: {_modSequence[_CurrentModeIndex].Replace("GameplayScene", "")}";

        }
        #endregion

        #region Update Methods
        private void Update()
        {
            if(_hasChallengeFinished || !_isTimerRunning) return;

            _timer += Time.deltaTime;
            _timerText.text = $"Time: {Mathf.Floor(_timer / 60):00}:{Mathf.Floor(_timer % 60):00}";
        }
        #endregion


        #region Completed_Methods
        //On evry mode that was completed
        public void OnModeCompleted()
        {
            SceneManager.UnloadSceneAsync(_modSequence[_CurrentModeIndex]);
            _CurrentModeIndex++;

            if (_CurrentModeIndex < _modSequence.Count)
            {
                LoadCurrentMode();
                UpdateModeText();
            }
            else
            {
                CompleteChallenge();
            }
        }

        private void CompleteChallenge()
        {
            _hasChallengeFinished = true;
            _isTimerRunning = false;
            int dailyScore = CalculateDailyScore(_timer);
            int currentDailyScore = PlayerPrefs.GetInt(DAILY_SCORE_KEY, 0);
            if (dailyScore > currentDailyScore)
            {
                PlayerPrefs.SetInt(DAILY_SCORE_KEY, dailyScore);
                DBManager.levelscore += dailyScore; 
                PlayerPrefs.SetInt("LevelScore", DBManager.levelscore);
                PlayerPrefs.Save();

                if (DBManager.LoggedIn && GameManager.Instance != null)
                {
                    StartCoroutine(GameManager.Instance.UpdateServerData()); 
                }
            }
            _winText.gameObject.SetActive(true); 
            _winText.text = $"Challenge Completed!\nTime: {Mathf.Floor(_timer / 60):00}:{Mathf.Floor(_timer % 60):00}\nScore: {dailyScore}";
            PlayerPrefs.SetString(LastDailyChallengeDateKey, DateTime.Now.ToString("yyyy-MM-dd"));
        }

        private int CalculateDailyScore(float timeTaken)
        {
            if (timeTaken >= MAX_TIME) return 0; 
            float scoreFactor = 1f - (timeTaken / MAX_TIME); 
            return Mathf.RoundToInt(MAX_DAILY_SCORE * scoreFactor); 
        }

        private void ReturnToMainMenu()
        {
            GameManager.Instance.GoToMainMenu();
        }
        #endregion

        #region DailyChallenge Availability
        private bool IschallengeAvailable()
        {
            string lastDateStr = PlayerPrefs.GetString(LastDailyChallengeDateKey, "");
            if(string.IsNullOrEmpty(lastDateStr))
            {
                return true;
            }
            DateTime lastDate = DateTime.ParseExact(lastDateStr, "yyy-MM-dd", null);
            DateTime today = DateTime.Now.Date;

            return today > lastDate;
        }

        #endregion

        #region Testing
        [ContextMenu("Reset Daily Challenge")]
        private void ResetChallenge()
        {
            PlayerPrefs.DeleteKey(LastDailyChallengeDateKey);
            PlayerPrefs.DeleteKey(CHALLENGE_AVAILABLE);
            PlayerPrefs.DeleteKey(DAILY_SCORE_KEY);
            Debug.Log("Daily Challenge reset!");
        }
        #endregion

        #region BUTTON_FUNCTIONS
        public void ClickedBack()
        {
            if (!_hasChallengeFinished)
            {
                _isTimerRunning = false;
                SceneManager.UnloadSceneAsync(_modSequence[_CurrentModeIndex]);
            }
            GameManager.Instance.GoToMainMenu();
        }
        #endregion
    }
}