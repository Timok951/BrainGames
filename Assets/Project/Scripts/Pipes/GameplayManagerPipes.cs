using Connect.Common;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace Connect.Core
{
    /// <summary>
    /// Gameplaymanager for connect pipes
    /// </summary>
    public class GameplayManagerPipes : MonoBehaviour
    {

        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _winText;
        [SerializeField] private Pipe _cellPrefab;
        [SerializeField] private GameObject _nextLevelButton; 
        [SerializeField] private GameObject _restartButton;   
        [SerializeField] private GameObject _backButton;


        private Tween playStartTween;
        private Tween playNextTween;

        private bool hasGameFinished;
        private Pipe[,] pipes;
        private List<Pipe> startPipes;

        private bool _isDailyChallengeMode;

        public static GameplayManagerPipes Instance;

        private LevelDataPipe _currentLevelData;
        #region START_METHODS
        private void Awake()
        {

            _winText.gameObject.SetActive(false);
            Instance = this;
            _currentLevelData = GameManager.Instance.GetLevelPipes();
            hasGameFinished = false;
            SpawnLevel();
            _isDailyChallengeMode = DailyChallengeManager.Instance != null;

            if (_isDailyChallengeMode)
            {
                if (_nextLevelButton != null) _nextLevelButton.SetActive(false);
                if (_restartButton != null) _restartButton.SetActive(false);
                if (_backButton != null) _backButton.SetActive(false);

                if (_winText.gameObject != null) _winText.gameObject.SetActive(false);
                if (_titleText.gameObject != null) _titleText.gameObject.SetActive(false);
            }
        }
        private void SpawnLevel()
        {
            _titleText.text = "Level " + GameManager.Instance.CurrentLevelPipes.ToString();
            pipes = new Pipe[_currentLevelData.Row, _currentLevelData.Col];
            startPipes = new List<Pipe>();

            for (int i = 0; i < _currentLevelData.Row; i++)
            {
                for (int j = 0; j < _currentLevelData.Col; j++)
                {
                    Vector2 targetPos = new Vector2(j + 0.5f, i + 0.5f);
                    Pipe tempPipe = Instantiate(_cellPrefab);
                    tempPipe.transform.position = targetPos + Vector2.down * 2f; 
                    tempPipe.transform.localScale = Vector3.zero;

           
                    tempPipe.transform
                        .DOMove(targetPos, 0.3f)
                        .SetEase(Ease.OutQuad)
                        .SetDelay((i + j) * 0.02f); 

                    tempPipe.transform
                        .DOScale(Vector3.one, 0.3f)
                        .SetEase(Ease.OutBack)
                        .SetDelay((i + j) * 0.02f);

                    tempPipe.Init(_currentLevelData.Data[i * _currentLevelData.Col + j]);
                    pipes[i, j] = tempPipe;

                    if (tempPipe.PipeType == 1)
                    {
                        startPipes.Add(tempPipe);
                    }
                }
            }

            Camera.main.orthographicSize = Mathf.Max(_currentLevelData.Row, _currentLevelData.Col);
            Vector3 cameraPos = Camera.main.transform.position;
            cameraPos.x = _currentLevelData.Col * 0.5f;
            cameraPos.y = _currentLevelData.Row * 0.5f;
            Camera.main.transform.position = cameraPos;

            Debug.Log($"Start pipes count: {startPipes.Count}");

            CheckFill();
            CheckWin();
        }

        #endregion

        #region UPDATE_METHODS
        private void Update()
        {
            if (hasGameFinished) return;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int row = Mathf.FloorToInt(mousePos.y);
            int col = Mathf.FloorToInt(mousePos.x);
            if (row < 0 || col < 0) return;

            if (row >= _currentLevelData.Row) return;

            if(col >= _currentLevelData.Col) return;


            if (Input.GetMouseButtonDown(0))
            {
                pipes[row, col].UpdateInput();
                StartCoroutine(ShowHint());

            }
            CheckWin();
            CheckFill();
        }

        private IEnumerator ShowHint()
        {
            yield return new WaitForSeconds(0.1f);
            CheckWin();
            CheckFill();
        }

        /// <summary>
        /// Fills a network of pipes starting from the specified initial pipes using a breadth-first search approach.
        /// Marks all reachable pipes as filled and updates their state in the grid.
        /// </summary>
        /// <param name="startPipes">The collection of initial pipes where the filling process begins.</param>
        /// <param name="pipes">A 2D array representing the grid of pipes to be updated.</param>
        /// <param name="levelData">The level data containing the dimensions of the pipe grid (rows and columns).</param

        private void CheckFill()
        {
            for (int i = 0; i < _currentLevelData.Row; i++)
            {
                for (int j = 0; j < _currentLevelData.Col; j++)
                {
                    Pipe tempPipe = pipes[i, j];
                    if (tempPipe.PipeType != 0 && tempPipe.PipeType != 1)
                    {
                        tempPipe.IsFilled = false;
                    }
                }
            }

            Queue<Pipe> check = new Queue<Pipe>();
            HashSet<Pipe> finished = new HashSet<Pipe>();
            //we have queue and Hashshets multiplicity
            foreach (var pipe in startPipes)
            {
                check.Enqueue(pipe);
                finished.Add(pipe);
                //Enque startpipes
            }

            //if pipe connected
            while(check.Count > 0)
            {
                Pipe pipe = check.Dequeue();
                List<Pipe> connected = pipe.ConnectedPipes();

                finished.Add(pipe);
                
                foreach (var connectedPipe in connected)
                {
                    if (!finished.Contains(connectedPipe))
                    {
                        check.Enqueue(connectedPipe);
                        finished.Add(connectedPipe);
                    }
                }
            }
            foreach (var filled in finished)
            {
                filled.IsFilled = true; 

            }

            for (int i = 0; i < _currentLevelData.Row; i++)
            {
                for (int j = 0; j < _currentLevelData.Col; j++)
                {
                    Pipe tempPipe = pipes[i, j];
                    tempPipe.UpdateFilled();
                }
            }
        }

        private void CheckWin()
        {
            if (hasGameFinished) return;

            for (int i = 0; i < _currentLevelData.Row; i++)
            {
                for (int j = 0; j < _currentLevelData.Col; j++)
                {
                    if (!pipes[i, j].IsFilled) return;
                }
            }
            hasGameFinished = true;
            _winText.gameObject.SetActive(true);
            hasGameFinished = true;
            Debug.Log("Game has been finished");

            if (!_isDailyChallengeMode)
            {
                GameManager.Instance.UnlockLevelPipes();
            }
            else if (DailyChallengeManager.Instance != null)
            {
                DailyChallengeManager.Instance.OnModeCompleted();
            }
        }
        #endregion


        #region BUTTON_FUNCTIONS
        public void ClickedBack(UnityEngine.UI.Button button)
        {
            AnimateAndSwitch(button, () =>
            {
                GameManager.Instance.GoToMainMenu();
            });
        }

        public void ClickedRestart(Button button)
        {
            AnimateAndSwitch(button, () =>
            {
                GameManager.Instance.GoToGameplayPipes();
            });
        }

        public void ClickedNextLevel(UnityEngine.UI.Button button)
        {
            AnimateAndSwitch(button, () =>
            {
                if (!hasGameFinished) return;

            GameManager.Instance.GoToGameplayPipes();
        });
        }

        public void Animate(GameObject target, System.Action onComplete, float duration = 1f)
        {

            if (playStartTween != null && playStartTween.IsActive())
            {
                playStartTween.Kill();
            }

            playStartTween = target.transform
            .DOScale(1.1f, 0.1f)
            .SetEase(Ease.Linear)
            .SetLoops(2, LoopType.Yoyo).OnComplete(() => onComplete?.Invoke());

            playStartTween.Play();
        }

        private void AnimateAndSwitch(Button button, System.Action switchAction)
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
        #endregion
    }
}

