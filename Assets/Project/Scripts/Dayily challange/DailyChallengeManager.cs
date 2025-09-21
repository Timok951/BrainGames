using Connect.Common;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Connect.Core
{
    /// <summary> 
    /// Class for daily challenge and base logic
    /// </summary>
    public class DailyChallengeManager : MonoBehaviour
    {
        #region variables
        public static DailyChallengeManager Instance;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private TMP_Text _winText;

        [SerializeField] private GameObject _challengeUI;

        private Tween playStartTween;
        private Tween playNextTween;

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

        private const int MAX_DAILY_SCORE = 500;
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
            if (_winText == null) Debug.LogError("_winText is not assigned!");
            Debug.Log("Daily challenge awake");

            UpdateTimerText();
        }

        private void Start()
        {
            if (IschallengeAvailable())
            {
                StartChallenge();
            }
            else
            {
                Debug.Log("Daily Challenge is not available today. Returning to Main Menu.");
                GameManager.Instance.GoToMainMenu();
            }
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
            }
        }
        #endregion

        #region Update Methods
        private void Update()
        {
            if (_hasChallengeFinished || !_isTimerRunning) return;

            _timer += Time.deltaTime;
            UpdateTimerText();
        }

        private void UpdateTimerText()
        {
            var timerLocalized = new LocalizedString("Gameplay", "Timer")
            {
                Arguments = new object[] { $"{Mathf.Floor(_timer / 60):00}:{Mathf.Floor(_timer % 60):00}" }
            };
            _timerText.text = timerLocalized.GetLocalizedString();
        }
        #endregion

        #region Completed_Methods
        public void OnModeCompleted()
        {
            SceneManager.UnloadSceneAsync(_modSequence[_CurrentModeIndex]);
            _CurrentModeIndex++;

            if (_CurrentModeIndex < _modSequence.Count)
            {
                LoadCurrentMode();
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

            var winLocalized = new LocalizedString("Gameplay", "ChallengeCompleted")
            {
                Arguments = new object[] { $"{Mathf.Floor(_timer / 60):00}:{Mathf.Floor(_timer % 60):00}", dailyScore }
            };
            _winText.text = winLocalized.GetLocalizedString();

            PlayerPrefs.SetString(LastDailyChallengeDateKey, DateTime.Now.ToString("yyyy-MM-dd"));
            PlayerPrefs.Save();
            Invoke(nameof(ReturnToMainMenu), 5f);
            Debug.Log("game has finished");
        }

        private int CalculateDailyScore(float timeTaken)
        {
            if (timeTaken >= MAX_TIME) return 0;
            float scoreFactor = 1f - (timeTaken / MAX_TIME);
            return Mathf.RoundToInt(MAX_DAILY_SCORE * scoreFactor);
        }

        private void ReturnToMainMenu()
        {
            Debug.Log(LastDailyChallengeDateKey + DateTime.Now.ToString("yyyy-MM-dd"));
            GameManager.Instance.GoToMainMenu();
        }
        #endregion

        #region DailyChallenge Availability
        private bool IschallengeAvailable()
        {
            string lastDateStr = PlayerPrefs.GetString(LastDailyChallengeDateKey, "");

            if (string.IsNullOrEmpty(lastDateStr))
            {
                Debug.Log("No previous challenge date found. Challenge is available.");
                return true;
            }

            try
            {
                DateTime lastDate = DateTime.ParseExact(lastDateStr, "yyyy-MM-dd", null);
                DateTime today = DateTime.Now.Date;
                Debug.Log($"Last challenge date: {lastDate}, Today: {today}");
                return today > lastDate;
            }
            catch (FormatException e)
            {
                Debug.LogError($"Failed to parse date '{lastDateStr}': {e.Message}. Resetting date.");
                PlayerPrefs.DeleteKey(LastDailyChallengeDateKey);
                PlayerPrefs.Save();
                return true;
            }
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
        public void ClickedBack(UnityEngine.UI.Button button)
        {
            AnimateAndSwitch(button, () =>
            {
                if (!_hasChallengeFinished)
                {
                    _isTimerRunning = false;
                    SceneManager.UnloadSceneAsync(_modSequence[_CurrentModeIndex]);
                }
                PlayerPrefs.SetString(LastDailyChallengeDateKey, DateTime.Now.ToString("yyyy-MM-dd"));
                PlayerPrefs.Save();
                Invoke(nameof(ReturnToMainMenu), 0.2f);
                Debug.Log("game has finished");
            });
        }
        #endregion

        public void Animate(GameObject target, System.Action onComplete, float duration = 1f)
        {
            if (playStartTween != null && playStartTween.IsActive())
            {
                playStartTween.Kill();
            }

            playStartTween = target.transform
                .DOScale(1.1f, 0.1f)
                .SetEase(Ease.Linear)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() => onComplete?.Invoke());

            playStartTween.Play();
        }

        private void AnimateAndSwitch(UnityEngine.UI.Button button, System.Action switchAction)
        {
            if (button != null)
            {
                Animate(button.gameObject, switchAction);
            }
            else
            {
                Debug.LogError("Button is null");
                switchAction?.Invoke();
            }
        }
    }
}