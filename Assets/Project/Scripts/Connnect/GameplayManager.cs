using Connect.Common;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace Connect.Core
{
    /// <summary>
    /// Code for spawning board and connecting dots to add logic for winning
    /// </summary>

    public class GameplayManager : MonoBehaviour
    {
        #region StartVariables
        public static GameplayManager Instance;

        public int levelmaxsize = 11;
        [HideInInspector] public bool hasGameFinished;

        private Tween playStartTween;
        private Tween playNextTween;
        public float BoardSize { get; private set; }
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] GameObject _winText;
        [SerializeField] private SpriteRenderer _clickHighlight;
        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private float _cellSpacing = 0.1f;

        [SerializeField] private GameObject _nextLevelButton;
        [SerializeField] private GameObject _restartButton;
        [SerializeField] private GameObject _backButton;

        private bool _isDailyChallengeMode;

        // Initializing
        private void Awake()
        {
            Instance = this;



            if (_titleText == null)
            {
                Debug.LogError("_titleText is not assigned!");
            }
            else
            {
                _titleText.text = "Level " + GameManager.Instance.CurrentLevelConnect.ToString();
            }

            if (_titleText != null)
            {
                int currentLevelIndex = GameManager.Instance.CurrentLevelNumberLinks ; 
                _titleText.text = $"Level {currentLevelIndex}";
            }


            hasGameFinished = false;
            _winText.SetActive(false);
            _titleText.gameObject.SetActive(true);
            _titleText.text = "Level " + GameManager.Instance.CurrentLevelConnect.ToString();

            CurrentLevelData = GameManager.Instance.GetLevelConnect();
            SpawndBoard();
            SpawnNodes();

            _isDailyChallengeMode = DailyChallengeManager.Instance != null;

            if (_isDailyChallengeMode)
            {
                if (_nextLevelButton != null) _nextLevelButton.SetActive(false);
                if (_restartButton != null) _restartButton.gameObject.SetActive(false);
                if (_backButton != null) _backButton.gameObject.SetActive(false);

                if (_winText.gameObject != null) _winText.gameObject.SetActive(false);
                if (_titleText.gameObject != null) _titleText.gameObject.SetActive(false);
            }

            var levelLocalized = new LocalizedString("Gameplay", "Level");
            levelLocalized.Arguments = new object[] { GameManager.Instance.CurrentLevelColorsort };
            levelLocalized.StringChanged += (value) => _titleText.text = value;


            levelLocalized.RefreshString();
        }
        #endregion

        #region BOARD_SPAWN
        [SerializeField] private SpriteRenderer _boardPrefab, _bgcellPrefab;
        private void SpawndBoard()
        {
            int currentLevelSize = Mathf.Min(GameManager.Instance.CurrentLevelConnect + 4, levelmaxsize);
            float totalCellSize = _cellSize + _cellSpacing;
            BoardSize = currentLevelSize * totalCellSize;

            float boardScaleFactor = 1.1f;

            if (_boardPrefab == null)
            {
                Debug.LogError("_boardPrefab is null!");
                return;
            }

            var board = Instantiate(_boardPrefab, Vector3.zero, Quaternion.identity);
            board.size = new Vector2(BoardSize, BoardSize);
            board.transform.localScale = Vector3.zero;

            if (board != null && board.gameObject != null)
            {
                playStartTween = board.transform.DOScale(Vector3.one, 0.5f)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        if (_bgcellPrefab == null)
                        {
                            Debug.LogError("_bgcellPrefab is null!");
                            return;
                        }
                        float startX = -BoardSize / 2f + totalCellSize / 2f;
                        float startY = -BoardSize / 2f + totalCellSize / 2f;

                        for (int i = 0; i < currentLevelSize; i++)
                        {
                            for (int j = 0; j < currentLevelSize; j++)
                            {
                                Instantiate(_bgcellPrefab,
                                    new Vector3(startX + i * totalCellSize, startY + j * totalCellSize, 0f),
                                    Quaternion.identity);
                            }
                        }
                    });
                playStartTween.Play();
            }

            Camera.main.orthographicSize = (BoardSize * boardScaleFactor) / 2f + 1f;
            Camera.main.transform.position = new Vector3(BoardSize / 2f, BoardSize / 2f, -10f);

            if (_clickHighlight != null)
            {
                _clickHighlight.size = new Vector2(totalCellSize, totalCellSize);
                _clickHighlight.transform.position = Vector3.zero;
                _clickHighlight.gameObject.SetActive(false);
            }

            AdjustCameraToFitBoard(currentLevelSize);
        }

        private void AdjustCameraToFitBoard(int currentLevelSize)
        {
            float totalCellSize = _cellSize + _cellSpacing;
            float boardWidth = currentLevelSize * totalCellSize;
            float boardHeight = currentLevelSize * totalCellSize;

            float screenAspect = (float)Screen.width / Screen.height;
            float boardAspect = boardWidth / boardHeight;

            float orthoSize;
            float padding = 1f;
            if (screenAspect > boardAspect)
            {
                orthoSize = (boardHeight / 2f) + padding;
            }
            else
            {
                orthoSize = (boardWidth / (2f * screenAspect)) + padding / screenAspect;
            }

            Camera.main.orthographicSize = orthoSize;

            Camera.main.transform.position = new Vector3(0f, 0f, -10f);
        }
        #endregion

        #region NODE_SPAWN
        private LevelData CurrentLevelData;
        [SerializeField] private Node _nodePrefab;
        private List<Node> _nodes;
        public Dictionary<Vector2Int, Node> _nodeGrid;

        public Dictionary<Vector2Int, Node> Nodes;
        // Spawning nodes
        private void SpawnNodes()
        {
            _nodes = new List<Node>();
            _nodeGrid = new Dictionary<Vector2Int, Node>();
            int currentLevelSize = Mathf.Min(GameManager.Instance.CurrentLevelConnect + 4, levelmaxsize);
            float totalCellSize = _cellSize + _cellSpacing;
            Node spawnedNode;
            Vector3 spawnPos;

            float startX = -BoardSize / 2f + totalCellSize / 2f;
            float startY = -BoardSize / 2f + totalCellSize / 2f;
            for (int i = 0; i < currentLevelSize; i++)
            {
                for (int j = 0; j < currentLevelSize; j++)
                {
                    spawnPos = new Vector3(startX + i * totalCellSize, startY + j * totalCellSize, 0f);
                    spawnedNode = Instantiate(_nodePrefab, spawnPos, Quaternion.identity);
                    spawnedNode.transform.localScale = Vector3.one * _cellSize;

                    spawnedNode.Init();
                    int colorIdForSpawnNode = GetColorId(i, j);

                    if (colorIdForSpawnNode != -1)
                    {
                        spawnedNode.SetColorForPoint(colorIdForSpawnNode);
                    }
                    Vector2Int pos = new Vector2Int(i, j);
                    if (_nodeGrid.ContainsKey(pos))
                    {
                        Debug.LogError($"Duplicate Node at {pos}");
                    }
                    _nodes.Add(spawnedNode);
                    _nodeGrid.Add(new Vector2Int(i, j), spawnedNode);
                    spawnedNode.gameObject.name = i.ToString() + j.ToString();
                    spawnedNode.Pos2D = new Vector2Int(i, j);
                }
            }
            List<Vector2Int> offsetPos = new List<Vector2Int>()
            { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            foreach (var item in _nodeGrid)
            {
                foreach (var offset in offsetPos)
                {
                    var checkPos = item.Key + offset;
                    if (_nodeGrid.ContainsKey(checkPos))
                    {
                        item.Value.SetEdge(offset, _nodeGrid[checkPos]);
                    }
                }
            }
        }

        // Getting color for highlight
        public Color GetHighLightColor(int colorId)
        {
            Color result = NodeColors[colorId];
            result.a = 0.4f;
            return result;
        }

        public List<Color> NodeColors;

        // Getting color ID
        public int GetColorId(int i, int j)
        {
            List<Edge> edges = CurrentLevelData.Edges;
            Vector2Int point = new Vector2Int(i, j);
            for (int colorId = 0; colorId < edges.Count; colorId++)
            {
                if (edges[colorId].StartPoint == point || edges[colorId].EndPoint == point)
                {
                    return colorId;
                }
            }
            Debug.Log($"Checking color for Node ({i},{j}), Total Edges: {edges.Count}");
            return -1;
        }
        #endregion

        #region Update_Methods
        // Drawing edges
        private Node startNode;

        private void Update()
        {
            var levelLocalized = new LocalizedString("Gameplay", "Level");
            levelLocalized.Arguments = new object[] { GameManager.Instance.CurrentLevelConnect };
            levelLocalized.StringChanged += (value) => _titleText.text = value;


            levelLocalized.RefreshString();

            if (hasGameFinished) return;

            if (Input.GetMouseButtonDown(0))
            {
                startNode = null;
                return;
            }
            if (Input.GetMouseButton(0))
            {
                // Drawing connect with color
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (startNode == null)
                {
                    if (hit && hit.collider.gameObject.TryGetComponent(out Node tNode) && tNode.IsClickable)
                    {
                        startNode = tNode;
                        _clickHighlight.gameObject.SetActive(true);
                        _clickHighlight.gameObject.transform.position = (Vector3)mousePos2D;
                        _clickHighlight.color = GetHighLightColor(tNode.colorId);
                    }

                    return;
                }

                _clickHighlight.gameObject.transform.position = (Vector3)mousePos2D;

                if (hit && hit.collider.gameObject.TryGetComponent(out Node tmpNode) && startNode != tmpNode)
                {
                    Debug.Log($"Trying to connect {startNode.name} to {tmpNode.name}");

                    if (!IsWithinBoardBounds(tmpNode.Pos2D))
                    {
                        Debug.Log($"{tmpNode.name} at {tmpNode.Pos2D} is outside board bounds ({BoardSize}x{BoardSize}), line not drawn");
                        return;
                    }
                    if (startNode.colorId != tmpNode.colorId && tmpNode.IsEndNode)
                    {
                        return;
                    }
                    Debug.Log($"Connected {startNode.name} to {tmpNode.name}. {startNode.name} now has {startNode.ConnectedNodes.Count} connections.");
                    startNode.UpdateInput(tmpNode);
                    CheckWin();
                    startNode = null;
                }
                return;
            }

            if (Input.GetMouseButtonUp(0))
            {
                // Highlight
                startNode = null;
                _clickHighlight.gameObject.SetActive(false);
            }
        }

        private bool IsWithinBoardBounds(Vector2Int position)
        {
            int currentLevelSize = Mathf.Min(GameManager.Instance.CurrentLevelConnect + 4, levelmaxsize);
            return position.x >= 0 && position.x < currentLevelSize && position.y >= 0 && position.y < currentLevelSize;
        }
        #endregion

        #region WIN_CONDITION
        // Win condition
        private void CheckWin()
        {
            if (CurrentLevelData == null || CurrentLevelData.Edges == null || CurrentLevelData.Edges.Count == 0)
            {
                Debug.LogError("CurrentLevelData or Edges is invalid!");
                return;
            }

            bool IsWinning = true;
            foreach (var edge in CurrentLevelData.Edges)
            {
                Vector2Int startPos = edge.StartPoint;
                Vector2Int endPos = edge.EndPoint;
                if (!_nodeGrid.ContainsKey(startPos) || !_nodeGrid.ContainsKey(endPos))
                {
                    Debug.LogError($"Edge nodes not found: Start {startPos}, End {endPos}");
                    return;
                }

                Node startNode = _nodeGrid[startPos];
                Node endNode = _nodeGrid[endPos];

                bool startConnected = startNode.IsEndNode && startNode.ConnectedNodes.Count == 1;
                bool endConnected = endNode.IsEndNode && endNode.ConnectedNodes.Count == 1;
                bool areConnected = AreNodesConnected(startNode, endNode);

                Debug.Log($"Edge {startPos} -> {endPos}: StartConnected = {startConnected}, EndConnected = {endConnected}, AreConnected = {areConnected}");
                IsWinning &= startConnected && endConnected && areConnected;
                if (!IsWinning)
                {
                    Debug.Log($"Win condition failed for edge {startPos} -> {endPos}");
                    return;
                }
            }

            Debug.Log("All edges connected successfully! Level won!");
            _winText.SetActive(true);
            _clickHighlight.gameObject.SetActive(false);
            hasGameFinished = true;

            if (!_isDailyChallengeMode)
            {
                GameManager.Instance.UnlockLevelConnect();
            }
            else if (DailyChallengeManager.Instance != null)
            {
                DailyChallengeManager.Instance.OnModeCompleted();
            }
        }

        // Recursively checks if a path exists between two nodes via their ConnectedNodes
        private bool AreNodesConnected(Node start, Node end, HashSet<Node> visited = null)
        {
            // It keeps track of already visited nodes to avoid infinite loops in case of looped connections.
            if (visited == null) visited = new HashSet<Node>();
            if (start == end) return true;
            if (visited.Contains(start)) return false;

            visited.Add(start);
            // Checking neighbour nodes
            foreach (var node in start.ConnectedNodes)
            {
                if (AreNodesConnected(node, end, visited))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region BUTTON_FUNCTIONS
        public void ClickedBack(Button button)
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
                GameManager.Instance.GoToGameplayConnect();
            });
        }

        public void ClickedNextLevel(Button button)
        {
            if (!hasGameFinished) return;

            AnimateAndSwitch(button, () =>
            {
                GameManager.Instance.GoToGameplayConnect();
            });
        }

        #region ANIMATIONS
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
        #endregion
        #endregion
    }
}