using Connect.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace Connect.Core
{
    /// <summary> 
    /// Class for daily challenge and base logic
    /// </summary>
    public class DailyChallengeManager : MonoBehaviour
    {
        public static DailyChallengeManager Instance;

        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private TMP_Text _winText;
        [SerializeField] private GameObject _challengeUI;

        private Tween playStartTween;

        private float _timer;
        private bool _isTimerRunning;
        private int _CurrentModeIndex;
        private bool _hasChallengeFinished;

        private int _pipesLevel;
        private int _colorSortLevel;
        private int _connectLevel;
        private int _oneStrokeLevel;
        private int _paintLevel;
        private int _numberlinkLevel;

        private const string DAILY_SCORE_KEY = "LevelScore";
        private const string LastDailyChallengeDateKey = "LastDailyChallengeDate";
        private const float MAX_TIME = 180f;
        private const int MAX_DAILY_SCORE = 500;

        private List<string> _modSequence = new List<string>()
        {
            "PipesGameplay"

                      // "GameplayColorSort",
            //"OneStroke",
            //"Paint",
            //"NumberlinkGameplay",
            //"GameplayConnect",

        };

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
            _oneStrokeLevel = GameManager.Instance.GetRandomLevelIndexOneStroke();
            _paintLevel = GameManager.Instance.GetRandomLevelIndexPaint();
            _numberlinkLevel = GameManager.Instance.GetRandomLevelIndexNumberLinks();

            if (_challengeUI != null) _challengeUI.SetActive(true);

            UpdateTimerText();
        }

        private void Start()
        {
            if (IsChallengeAvailable())
                StartChallenge();
            else
            {
                ReturnToMainMenu();
            }
        }

        private void Update()
        {
            if (_hasChallengeFinished || !_isTimerRunning) return;

            _timer += Time.deltaTime;
            UpdateTimerText();
        }

        private void StartChallenge()
        {
            _isTimerRunning = true;
            LoadCurrentMode();
        }

        private void ClearPreviousLevelObjects()
        {
            foreach (var obj in GameObject.FindObjectsOfType<GameObject>())
            {
                if (obj.name.Contains("Edge(Clone)") || obj.name.Contains("Point(Clone)"))
                    Destroy(obj);
            }
        }

        private void LoadCurrentMode()
        {
            if (_CurrentModeIndex >= _modSequence.Count) return;

            if (_CurrentModeIndex > 0)
            {
                string prevSceneName = _modSequence[_CurrentModeIndex - 1];
                if (SceneManager.GetSceneByName(prevSceneName).isLoaded)
                    SceneManager.UnloadSceneAsync(prevSceneName);
            }

            ClearPreviousLevelObjects();

            switch (_modSequence[_CurrentModeIndex])
            {
                case "PipesGameplay":
                    GameManager.Instance.SetCurrentLevelPipes(_pipesLevel);
                    break;
                case "GameplayColorSort":
                    GameManager.Instance.SetCurrentLevelColorSort(_colorSortLevel);
                    break;
                case "OneStroke":
                    GameManager.Instance.SetCurrentLevelOneStroke(_oneStrokeLevel);
                    break;
                case "Paint":
                    GameManager.Instance.SetCurrentLevelPaint(_paintLevel);
                    break;
                case "NumberlinkGameplay":
                    GameManager.Instance.SetCurrentLevelNumberLinks(_numberlinkLevel);
                    break;
                case "GameplayConnect":
                    GameManager.Instance.SetCurrentLevelConnect(_connectLevel);
                    break;
            }

            SceneManager.LoadScene(_modSequence[_CurrentModeIndex], LoadSceneMode.Additive);
        }

        public void OnModeCompleted()
        {
            _CurrentModeIndex++;
            if (_CurrentModeIndex < _modSequence.Count)
                LoadCurrentMode();
            else
                CompleteChallenge();
        }

        private void CompleteChallenge()
        {
            if (_hasChallengeFinished) return;

            _hasChallengeFinished = true;
            _isTimerRunning = false;

            int score = CalculateDailyScore(_timer);

            DBManager.levelscore += score;

            PlayerPrefs.SetInt(DAILY_SCORE_KEY, DBManager.levelscore);
            PlayerPrefs.Save();

            Debug.Log($"[DailyChallenge] CompleteChallenge called. DailyScore: {score}, LevelScore: {DBManager.levelscore}");

            _winText.gameObject.SetActive(true);
            var winLocalized = new LocalizedString("Gameplay", "ChallengeCompleted")
            {
                Arguments = new object[] { $"{Mathf.Floor(_timer / 60):00}:{Mathf.Floor(_timer % 60):00}", score }
            };
            _winText.text = winLocalized.GetLocalizedString();

            if (DBManager.LoggedIn && !string.IsNullOrEmpty(DBManager.nick))
                StartCoroutine(UpdateServerAndReturn());
            else
                Invoke(nameof(ReturnToMainMenu), 5f);
        }

        private IEnumerator UpdateServerAndReturn()
        {
            if (!string.IsNullOrEmpty(DBManager.nick))
                yield return GameManager.Instance.UpdateServerData();

            Invoke(nameof(ReturnToMainMenu), 5f);
        }

        private int CalculateDailyScore(float timeTaken)
        {
            if (timeTaken >= MAX_TIME) return 0;
            float factor = 1f - (timeTaken / MAX_TIME);
            return Mathf.RoundToInt(MAX_DAILY_SCORE * factor);
        }

        private void UpdateTimerText()
        {
            var timerLocalized = new LocalizedString("Gameplay", "Timer")
            {
                Arguments = new object[] { $"{Mathf.Floor(_timer / 60):00}:{Mathf.Floor(_timer % 60):00}" }
            };
            _timerText.text = timerLocalized.GetLocalizedString();
        }

        private bool IsChallengeAvailable()
        {
            string lastDateStr = PlayerPrefs.GetString(LastDailyChallengeDateKey, "");
            if (string.IsNullOrEmpty(lastDateStr)) return true;

            try
            {
                DateTime lastDate = DateTime.ParseExact(lastDateStr, "yyyy-MM-dd", null);
                return DateTime.Now.Date > lastDate;
            }
            catch
            {
                PlayerPrefs.DeleteKey(LastDailyChallengeDateKey);
                PlayerPrefs.Save();
                return true;
            }
        }

        [ContextMenu("Reset Daily Challenge")]
        private void ResetChallenge()
        {
            PlayerPrefs.DeleteKey(LastDailyChallengeDateKey);
            PlayerPrefs.DeleteKey(DAILY_SCORE_KEY);
        }

        public void ClickedBack(UnityEngine.UI.Button button)
        {
            AnimateAndSwitch(button, () =>
            {
                if (!_hasChallengeFinished && _CurrentModeIndex < _modSequence.Count)
                    SceneManager.UnloadSceneAsync(_modSequence[_CurrentModeIndex]);

                PlayerPrefs.SetString(LastDailyChallengeDateKey, DateTime.Now.ToString("yyyy-MM-dd"));
                PlayerPrefs.Save();
                ReturnToMainMenu();
            });
        }

        private void ReturnToMainMenu() => GameManager.Instance.GoToMainMenu();

        public void Animate(UnityEngine.GameObject target, Action onComplete, float duration = 1f)
        {
            if (playStartTween != null && playStartTween.IsActive()) playStartTween.Kill();

            playStartTween = target.transform
                .DOScale(1.1f, 0.1f)
                .SetEase(Ease.Linear)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() => onComplete?.Invoke());

            playStartTween.Play();
        }

        private void AnimateAndSwitch(UnityEngine.UI.Button button, Action switchAction)
        {
            if (button != null)
                Animate(button.gameObject, switchAction);
            else
                switchAction?.Invoke();
        }
    }
}
