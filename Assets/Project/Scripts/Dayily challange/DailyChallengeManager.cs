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

        private List<string> _modSequence = new List<string>()
        {
            "PipesGameplayScene",
            "ColorSortGameplayScene",
            "ConnectGameplayScene"
        };

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

        }
        private void StartChallenge()
        {
            _isTimerRunning = true;

        }
        private void LoadCurrentMode()
        {
            if (_CurrentModeIndex < _modSequence.Count)
            {
                SceneManager.LoadScene(_modSequence[_CurrentModeIndex], LoadSceneMode.Additive);
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
            _isTimerRunning=false;
            _winText.text = $"Challenge Completed!\nTime: {Mathf.Floor(_timer/ 60):00}:{Mathf.Floor(_timer % 60)}:00";

            //Savind data
            PlayerPrefs.SetString(LastDailyChallengeDateKey, DateTime.Now.ToString("yyyy-MM-dd"));
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