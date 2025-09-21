using Connect.Common;
using Connect.Core;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Connect.Core
{
    /// <summary>
    /// Gameplay manager for Numberlink level that connects core logic for the game
    /// </summary>
    public class GameplayNumberlink : MonoBehaviour
    {
        public static GameplayNumberlink Instance;

        public bool hasGameFinished;

        private LocalizedString _levelLocalized;
        public float Edgesize => _cellGap + _cellSize;
        private Tween playStartTween;

        [Header("Prefabs & Sprites")]
        [SerializeField] private CellNumberlink _cellprefab;
        [SerializeField] private SpriteRenderer _bgSprite;
        [SerializeField] private SpriteRenderer _highlightSprite;
        [SerializeField] private Vector2 _highlightSize;

        [Header("Level Data")]
        [SerializeField] private LevelDataNumberLinks _levelData;
        [SerializeField] private float _cellGap;
        [SerializeField] private float _cellSize;
        [SerializeField] private float _levelGap;

        [Header("UI")]
        [SerializeField] private GameObject _winText;
        [SerializeField] private GameObject _nextLevelButton;
        [SerializeField] private GameObject _restartButton;
        [SerializeField] private GameObject _backButton;
        [SerializeField] private TMP_Text _titleText;
        private bool _isDailyChallengeMode;


        private int[,] leveGrid;
        private CellNumberlink[,] cellGrid;
        private CellNumberlink startCell;
        private Vector2 startPos;

        private List<Vector2Int> Directions = new List<Vector2Int>() {
            Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
        };

        private void Awake()
        {



            Instance = this;
            hasGameFinished = false;

            _highlightSprite.gameObject.SetActive(false);
            Connect.Common.NumberLinkLevel currentLevel = GameManager.Instance.GetLevelNumberLinks();

            int currentLevelIndex = GameManager.Instance.CurrentLevelNumberLinks;

            Debug.Log(currentLevelIndex);
            if (_titleText == null)
            {
                Debug.LogError("_titleText is not assigned!");
            }

            _levelLocalized = new LocalizedString("Gameplay", "Level");
            _levelLocalized.Arguments = new object[] { GameManager.Instance.CurrentLevelNumberLinks };
            _levelLocalized.StringChanged += (value) => _titleText.text = value;
            _levelLocalized.RefreshString();

            var tableName = "Gameplay";
            var keyName = "Level";

            var table = LocalizationSettings.StringDatabase.GetTable(tableName);
            if (table == null)
            {
                Debug.LogError($"Localization table '{tableName}' not found!");
            }
            else
            {
                var entry = table.GetEntry(keyName);
                if (entry == null)
                {
                    Debug.LogError($"Localization key '{keyName}' not found in table '{tableName}'!");
                }
                else
                {
                    Debug.Log($"Localization key '{keyName}' found! Value: {entry.GetLocalizedString()}");
                }
            }



            if (_titleText != null)
                _titleText.gameObject.SetActive(true);

            _levelLocalized.RefreshString();

            _levelData = new LevelDataNumberLinks
            {
                row = currentLevel.Rows,
                Col = currentLevel.Columns,
                data = new List<int>(currentLevel.Data)
            };

            leveGrid = new int[_levelData.row, _levelData.Col];
            cellGrid = new CellNumberlink[_levelData.row, _levelData.Col];

            for (int i = 0; i < _levelData.row; i++)
            {
                for (int j = 0; j < _levelData.Col; j++)
                {
                    leveGrid[i, j] = _levelData.data[i * _levelData.Col + j];
                }
            }
            _isDailyChallengeMode = DailyChallengeManager.Instance != null;


            if (_isDailyChallengeMode)
            {
                if (_nextLevelButton != null) _nextLevelButton.SetActive(false);
                if (_restartButton != null) _restartButton.gameObject.SetActive(false);
                if (_backButton != null) _backButton.gameObject.SetActive(false);

                if (_winText.gameObject != null) _winText.gameObject.SetActive(false);
                if (_titleText.gameObject != null) _titleText.gameObject.SetActive(false);
            }

            SpawnLevel();

        }


        private void SpawnLevel()
        {

            _winText.SetActive(false);

            float width = (_cellSize + _cellGap) * _levelData.Col - _cellGap + _levelGap;
            float height = (_cellSize + _cellGap) * _levelData.row - _cellGap + _levelGap;
            _bgSprite.size = new Vector2(width, height);

            Vector3 bgPos = new Vector3(
                width / 2f - _cellSize / 2f - _levelGap / 2f,
                height / 2f - _cellSize / 2f - _levelGap / 2f,
                0f);
            _bgSprite.transform.position = bgPos;

            Camera.main.orthographicSize = width * 1.2f;
            Camera.main.transform.position = new Vector3(bgPos.x, bgPos.y, -10f);

            Vector3 startPos = Vector3.zero;
            Vector3 rightOffset = Vector3.right * (_cellSize + _cellGap);
            Vector3 topOffset = Vector3.up * (_cellSize + _cellGap);

            for (int i = 0; i < _levelData.row; i++)
            {
                for (int j = 0; j < _levelData.Col; j++)
                {
                    Vector3 spawnPos = startPos + j * rightOffset + i * topOffset;
                    CellNumberlink tempCell = Instantiate(_cellprefab, spawnPos, Quaternion.identity);
                    tempCell.Init(i, j, leveGrid[i, j]);
                    cellGrid[i, j] = tempCell;

                    if (tempCell.Number == 0)
                    {
                        Destroy(tempCell.gameObject);
                        cellGrid[i, j] = null;
                    }
                }
            }

            for (int i = 0; i < _levelData.row; i++)
            {
                for (int j = 0; j < _levelData.Col; j++)
                {
                    if (cellGrid[i, j] != null)
                        cellGrid[i, j].Init();
                }
            }
        }

        private void Update()
        {

            if (_titleText.text != _levelLocalized.GetLocalizedString())
            {
                Debug.Log($"Title text mismatch! Current: {_titleText.text}, Expected: {_levelLocalized.GetLocalizedString()}");
                _titleText.text = _levelLocalized.GetLocalizedString();
            }

            if (hasGameFinished) return;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            if (Input.GetMouseButtonDown(0))
            {
                startCell = null;
                startPos = mousePos2D;

                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                if (hit && hit.collider.TryGetComponent(out startCell))
                {
                    _highlightSprite.gameObject.SetActive(true);
                    _highlightSprite.size = _highlightSize;
                    _highlightSprite.transform.position = startCell.transform.position;
                }
                else
                {
                    TryRemoveEdgeAtMouse(mousePos);
                }
            }
            else if (Input.GetMouseButton(0))
            {
                if (startCell == null) return;

                Vector2 offset = mousePos2D - startPos;
                Vector2Int offsetDirection = GetDirection(offset);
                float offsetValue = GetOffset(offset, offsetDirection);
                int directionIndex = GetDirectionIndex(offsetDirection);

                Vector3 angle = new Vector3(0, 0, 90f * (directionIndex - 1));
                _highlightSprite.transform.eulerAngles = angle;

                _highlightSprite.size = new Vector2(_highlightSize.x, offsetValue);

                Vector3 newPos = startCell.transform.position;
                if (offsetDirection == Vector2Int.up)
                    newPos.y += offsetValue / 2f;
                else if (offsetDirection == Vector2Int.down)
                    newPos.y -= offsetValue / 2f;
                else if (offsetDirection == Vector2Int.right)
                    newPos.x += offsetValue / 2f;
                else if (offsetDirection == Vector2Int.left)
                    newPos.x -= offsetValue / 2f;

                _highlightSprite.transform.position = newPos;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (startCell == null) return;

                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                if (hit && hit.collider.TryGetComponent(out CellNumberlink endCell))
                {
                    if (endCell == startCell)
                    {
                        startCell.RemoveAllEdges();
                        RemoveEdgesAround(startCell);
                    }
                    else
                    {
                        Vector2 offset = mousePos2D - startPos;
                        Vector2Int offsetDirection = GetDirection(offset);
                        int directionIndex = GetDirectionIndex(offsetDirection);

                        if (startCell.IsValidCell(endCell, directionIndex))
                        {
                            startCell.AddEdge(directionIndex);
                            endCell.AddEdge((directionIndex + 2) % 4);
                        }
                    }
                }

                startCell = null;
                _highlightSprite.gameObject.SetActive(false);
                Checkwin();
            }
        }


        private void TryRemoveEdgeAtMouse(Vector3 mousePos)
        {
            CellNumberlink cell = null;
            RaycastHit2D hit;

            hit = Physics2D.Raycast(mousePos, Vector2.left);
            if (hit && hit.collider.TryGetComponent(out cell)) cell.RemoveEdge(0);

            hit = Physics2D.Raycast(mousePos, Vector2.down);
            if (hit && hit.collider.TryGetComponent(out cell)) cell.RemoveEdge(1);

            hit = Physics2D.Raycast(mousePos, Vector2.right);
            if (hit && hit.collider.TryGetComponent(out cell)) cell.RemoveEdge(2);

            hit = Physics2D.Raycast(mousePos, Vector2.up);
            if (hit && hit.collider.TryGetComponent(out cell)) cell.RemoveEdge(3);
        }

        private void RemoveEdgesAround(CellNumberlink cell)
        {
            for (int i = 0; i < 4; i++)
            {
                var adjacentCell = GetAdjacentCell(cell.Row, cell.Column, i);
                if (adjacentCell != null)
                {
                    int adjacentDirection = (i + 2) % 4;
                    adjacentCell.RemoveEdge(adjacentDirection);
                }
            }
        }

        private void Checkwin()
        {
            for (int i = 0; i < _levelData.row; i++)
            {
                for (int j = 0; j < _levelData.Col; j++)
                {
                    if (cellGrid[i, j] != null && cellGrid[i, j].Number != 0) return;
                }
            }

            hasGameFinished = true;

            _winText.SetActive(true);

            if (!_isDailyChallengeMode)
            {
                GameManager.Instance.UnlockLevelNumberLinks();

            }
            else if (DailyChallengeManager.Instance != null)
            {
                DailyChallengeManager.Instance.OnModeCompleted();
            }
        }

        public void ClickedRestart(Button button)
        {
            AnimateAndSwitch(button, () =>
            {
                GameManager.Instance.GoToGameplayNumberLinks();
            });
        }

        public void ClickedNextLevel(Button button)
        {
            if (!hasGameFinished) return;

            AnimateAndSwitch(button, () =>
            {
                GameManager.Instance.GoToGameplayNumberLinks();
            });
        }


        public void ClickedBack(Button button)
        {
            AnimateAndSwitch(button, () =>
            {
                GameManager.Instance.GoToMainMenu();
            });
        }


        private int GetDirectionIndex(Vector2Int offsetDirection)
        {
            if (offsetDirection == Vector2Int.right) return 0;
            if (offsetDirection == Vector2Int.left) return 2;
            if (offsetDirection == Vector2Int.up) return 1;
            if (offsetDirection == Vector2Int.down) return 3;
            return -1;
        }

        private float GetOffset(Vector2 offset, Vector2Int offsetDirection)
        {
            if (offsetDirection == Vector2Int.left || offsetDirection == Vector2Int.right)
                return Mathf.Abs(offset.x);
            else
                return Mathf.Abs(offset.y);
        }

        private Vector2Int GetDirection(Vector2 offset)
        {
            if (Mathf.Abs(offset.y) > Mathf.Abs(offset.x))
                return offset.y > 0 ? Vector2Int.up : Vector2Int.down;
            else
                return offset.x > 0 ? Vector2Int.right : Vector2Int.left;
        }

        public CellNumberlink GetAdjacentCell(int row, int col, int direction)
        {
            Vector2Int currentDirection = Directions[direction];
            Vector2Int startPos = new Vector2Int(row, col);
            Vector2Int checkPos = startPos + currentDirection;
            while (IsValid(checkPos) && cellGrid[checkPos.x, checkPos.y] == null)
            {
                checkPos += currentDirection;
            }
            return IsValid(checkPos) ? cellGrid[checkPos.x, checkPos.y] : null;
        }

        public bool IsValid(Vector2Int pos)
        {
            return pos.x >= 0 && pos.y >= 0 && pos.x < _levelData.row && pos.y < _levelData.Col;
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
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() => onComplete?.Invoke());

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


    }


    [Serializable]
    public struct LevelDataNumberLinks
    {
        public int row, Col;
        public List<int> data;
    }
}
