using Connect.Core;
using DG.Tweening;
using log4net.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Connect.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace Connect.Core
{
    public class GameplayManagerColorSort : MonoBehaviour
    {
        #region StartVariables
        public static GameplayManagerColorSort Instance;
        public static int Rows;
        public static int Cols;

        private LevelColorSort _currentLevelData;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _winText;


        [SerializeField] private TMP_Text _movesText;
        [SerializeField] private TMP_Text _bestText;
        [SerializeField] private Transform _gridParent;
        [SerializeField] private Transform _nextButton;
        [SerializeField] private Transform _restartButton;
        [SerializeField] private Transform _backButton;

        [SerializeField] private Cell _cellPrefab;



        private const int Front = 1;
        private const int Back = 0;


        private string leveldate;
        private int moveNum;
        private int bestNum;

        private bool hasGameFinished;
        private bool canMove;
        private bool canStartClicking;

        private Cell[,] cells;
        private Color[,] correctColors;

        private Cell selectedCell;
        private Cell movedCell;
        private Vector2 startPos;

        //Tweens for animations 
        private Tween palystartTween;
        private Tween palyendTween;
        #endregion
        public float offsetX;
        public float offsetY;
        private bool _isDailyChallengeMode;

        #region START_METHODS
        private void Awake()
        {
            Debug.Log("Awake started");
            Instance = this;
            hasGameFinished = false;
            _winText.gameObject.SetActive(false);

            _currentLevelData = GameManager.Instance.GetLevelColorSort();

            if (_currentLevelData == null)
            {
                Debug.LogError("_currentLevelData is null!", this);
                return;
            }

            Rows = _currentLevelData.Row;
            Cols = _currentLevelData.Col;

            _levelText.text = "Level"+ " " + GameManager.Instance.CurrentLevelColorsort.ToString();
            _movesText.text = "Moves " + moveNum.ToString();
            _bestText.text = bestNum.ToString();
            moveNum = 0;
            bestNum = PlayerPrefs.GetInt("Best" + _currentLevelData.ToString(), 0);
            DOTween.defaultAutoPlay = AutoPlay.None;
            SpawnCells();
            StartCoroutine(WaitForInitialAnimation());
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    Debug.Log($"Correct color at ({i}, {j}): {correctColors[i, j]}");
                }
            }

            _isDailyChallengeMode = DailyChallengeManager.Instance != null;

            if (_isDailyChallengeMode)
            {
                if (_nextButton != null) _nextButton.gameObject.SetActive(false);
                if (_restartButton != null) _restartButton.gameObject.SetActive(false);
                if (_backButton != null) _backButton.gameObject.SetActive(false);

                if (_winText.gameObject != null) _winText.gameObject.SetActive(false);
                if (_levelText.gameObject != null) _levelText.gameObject.SetActive(false);
                if (_movesText.gameObject != null) _movesText.gameObject.SetActive(false);

            }
        }

        private IEnumerator WaitForInitialAnimation()
        {

            yield return new WaitForSeconds(0.5f + (Rows + Cols) * _cellPrefab._startScalelDelay + _cellPrefab._startScaleTime);
            foreach (var cell in cells)
            {
                if (cell.startAnimation != null && cell.startAnimation.IsActive())
                    cell.startAnimation.Complete();
                cell.transform.localScale = Vector3.one * 0.4f;
            }

            PerformSwaps();

            yield return new WaitForSeconds(_cellPrefab._startMoveAnimationTime);
        }

        private void SpawnCells()
        {
            cells = new Cell[_currentLevelData.Row, _currentLevelData.Col];
            correctColors = new Color[_currentLevelData.Row, _currentLevelData.Col];

            offsetX = (Cols - 1) / 2f;
            offsetY = (Rows - 1) / 2f;

            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Cols; y++)
                {
                    float xLerp = (float)y / (Cols - 1);
                    float yLerp = (float)x / (Rows - 1);
                    Color leftColor = Color.Lerp(_currentLevelData.BottomLeftColor, _currentLevelData.TopLeftColor, yLerp);
                    Color rightColor = Color.Lerp(_currentLevelData.BottomRightColor, _currentLevelData.TopRightColor, yLerp);
                    Color currentColor = Color.Lerp(leftColor, rightColor, xLerp);
                    correctColors[x, y] = currentColor;
                    bool isLocked = _currentLevelData.LockedCells.Contains(new Vector2Int(x, y));

                    cells[x, y] = Instantiate(_cellPrefab, _gridParent);
                    cells[x, y].Init(currentColor, y, x, offsetX, offsetY, isLocked); 
                    Debug.Log($"Cell at ({x}, {y}) - Locked: {isLocked}");
                }
            }
            Camera.main.orthographicSize = Mathf.Max(Rows, Cols) * 1.5f / 2f + 1f;
        }

        private void PerformSwaps()
        {
            {
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Cols; j++)
                    {
                        if (_currentLevelData.LockedCells.Contains(new Vector2Int(i, j)))
                        {
                            continue;
                        }

                        int swapX, swapY;
                        do
                        {
                            swapX = Random.Range(0, Rows);
                            swapY = Random.Range(0, Cols);
                        } while (_currentLevelData.LockedCells.Contains(new Vector2Int(swapX, swapY)));

                        Cell temp = cells[i, j];
                        cells[i, j] = cells[swapX, swapY];
                        Vector2Int swappedPosition = cells[swapX, swapY].Position;
                        cells[i, j].Position = temp.Position;
                        cells[swapX, swapY] = temp;
                        temp.Position = swappedPosition;

                        cells[i, j].AnimateStartPosition(offsetX, offsetY);
                        cells[swapX, swapY].AnimateStartPosition(offsetX, offsetY);
                    }
                }
            }
        }

        private void AnimateCells()
        {
            for (int i = 0; i < Rows; ++i)
            {
                for (int j = 0; j < Cols; ++j)
                {
                    cells[i, j].AnimateStartPosition(offsetX, offsetY);
                }
            }
        }

        #endregion

        #region UPDATE_METHODS
        private void Update()
        {
            if (hasGameFinished) return;

            if (!canStartClicking)
            {
                canStartClicking = true;
                for (int i = 0; i < Rows; ++i)
                {
                    for (int j = 0; j < Cols; ++j)
                    {
                        if (cells[i, j].IsStarMovePlaying)
                        {
                            canStartClicking = false;
                            return;
                        }
                    }
                }
                canMove = true;
            }

            if (canMove && selectedCell != null && movedCell != null)
            {
                if (selectedCell.hasSelectedMoveFinished && movedCell.hasMoveFinished)
                {
                    selectedCell = null;
                    movedCell = null;
                    canMove = true;
                    CheckWin();
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                Debug.Log($"Mouse pos: {mousePos2D}, Hit: {(hit.collider != null ? hit.collider.name : "None")}");
                if (hit.collider != null && hit.collider.TryGetComponent(out Cell clickedCell))
                {
                    if (_currentLevelData.LockedCells.Contains(new Vector2Int(clickedCell.Position.y, clickedCell.Position.x)))
                    {
                        if (selectedCell != null)
                        {
                            selectedCell.SelectedMoveEnd();
                            selectedCell = null;
                        }
                        return;
                    }

                    if (selectedCell != null && selectedCell != clickedCell)
                    {
                        selectedCell.SelectedMoveEnd();
                    }
                    selectedCell = clickedCell;
                    startPos = mousePos2D;
                    selectedCell.SelectedMoveStart();
                }
            }

            if (Input.GetMouseButton(0) && selectedCell != null)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                Vector2 offset = mousePos2D - startPos;
                selectedCell.SelectedMove(offset);
            }

            if (Input.GetMouseButtonUp(0) && selectedCell != null)
            {
                if (selectedCell == null) return;

                canMove = false;
                Vector2 pos = (Vector2)selectedCell.transform.localPosition;
                int row = Mathf.Clamp(Mathf.RoundToInt(pos.y + offsetY), 0, Rows - 1);
                int col = Mathf.Clamp(Mathf.RoundToInt(pos.x + offsetX), 0, Cols - 1);
                movedCell = cells[row, col];

                Debug.Log($"SelectedCell pos: {pos}, Target row: {row}, col: {col}");

                if (_currentLevelData.LockedCells.Contains(new Vector2Int(row, col)) || movedCell == selectedCell)
                {
                    selectedCell.SelectedMoveEnd();
                    selectedCell = null;
                    canMove = true;
                    return;
                }

                Vector2Int tempPos = selectedCell.Position;
                selectedCell.Position = movedCell.Position;
                movedCell.Position = tempPos;

                cells[selectedCell.Position.y, selectedCell.Position.x] = selectedCell;
                cells[movedCell.Position.y, movedCell.Position.x] = movedCell;

                selectedCell.SelectedMoveEnd();
                movedCell.MoveEnd();
                moveNum++;
                _movesText.text = "Moves " + moveNum.ToString();

                selectedCell = null;
                CheckWin();
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Cols; j++)
                    {
                        Debug.Log($"Current color at ({i}, {j}): {cells[i, j].Color}");
                    }
                }
            }
        }


        private void CheckWin()
        {
            for(int i = 0; i < Rows; i++)
            {
                for(int j =0; j < Cols; j++){
                    if (cells[i,j].Color != correctColors[i,j])
                    {
                        return;
                    }
                }
            }
            hasGameFinished = true;
            _winText.gameObject.SetActive(true);
            if(bestNum == 0 || bestNum > moveNum)
            {

            }

            PlayerPrefs.SetInt("Best" + _currentLevelData, bestNum);

            if (!_isDailyChallengeMode)
            {
                GameManager.Instance.UnlockLevelColorsort();
            }
            else if (DailyChallengeManager.Instance != null)
            {
                DailyChallengeManager.Instance.OnModeCompleted();
            }

        }
        #endregion


        #region BUTTON_FUNCTIONS
        public void ClickedBack()
        {
            GameManager.Instance.GoToMainMenu();
        }

        public void ClickedRestart()
        {
            GameManager.Instance.GoToGameplayColorSort();
        }

        public void ClickedNextLevel()
        {
            if (!hasGameFinished) return;

            GameManager.Instance.GoToGameplayColorSort();
        }
        #endregion
    }

}
