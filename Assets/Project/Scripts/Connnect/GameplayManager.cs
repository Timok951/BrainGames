using Connect.Common;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Connect.Core {
    public class GameplayManager : MonoBehaviour
    {
        #region startMethods



        #region StartVariables
        public static GameplayManager Instance;

        public int levelmaxsize = 11;
        [HideInInspector] public bool hasGameFinished;
        public float BoardSize { get; private set; }
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] GameObject _winText;
        [SerializeField] private SpriteRenderer _clickHighlight;
        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private float _cellSpacing = 0.1f;
        //Initializing
        private void Awake()
        {
            Instance = this;

            hasGameFinished = false;
            _winText.SetActive(false);
            _titleText.gameObject.SetActive(true);
            _titleText.text = "Level " + GameManager.Instance.CurrentLevel.ToString();

            CurrentLevelData = GameManager.Instance.GetLevelConnect();
            SpawndBoard();
            SpawnNodes();
        }
        #endregion

        #endregion

        #region BOARD_SPAWN
        [SerializeField] private SpriteRenderer _boardPrefab, _bgcellPrefab;
        private void SpawndBoard()
        {
            int currentLevelSize = Mathf.Min(GameManager.Instance.CurrentLevel + 4, levelmaxsize);
            float totalCellSize = _cellSize + _cellSpacing;
            BoardSize = currentLevelSize * totalCellSize;

            float boardScaleFactor = 1.1f;

            var board = Instantiate(_boardPrefab,
                new Vector3(BoardSize / 2f, BoardSize / 2f, 0f),
                Quaternion.identity);

            board.size = new Vector2(BoardSize, BoardSize);
            //boardoffset
            float startX = (BoardSize - totalCellSize * currentLevelSize) / 2f + totalCellSize / 2f;
            float startY = (BoardSize - totalCellSize * currentLevelSize) / 2f + totalCellSize / 2f;

            //spawning cells
            for (int i = 0; i < currentLevelSize; i++)
            {
                for (int j = 0; j < currentLevelSize; j++)
                {
                    Instantiate(_bgcellPrefab,
                        new Vector3(startX + i * totalCellSize, startY + j * totalCellSize, 0f),
                        Quaternion.identity);
                }
            }

            //moving camera to fully show the level
            Camera.main.orthographicSize = (BoardSize * boardScaleFactor) / 2f + 1f;
            Camera.main.transform.position = new Vector3(BoardSize / 2f, BoardSize / 2f, -10f);

            _clickHighlight.size = new Vector2(currentLevelSize / 4f, currentLevelSize / 4f);
            _clickHighlight.transform.position = Vector3.zero;
            _clickHighlight.gameObject.SetActive(false);

        }


        #endregion

        #region NODE_SPAWN
        private LevelData CurrentLevelData;
        [SerializeField] private Node _nodePrefab;
        private List<Node> _nodes;
        public Dictionary<Vector2Int, Node> _nodeGrid;


        public Dictionary<Vector2Int, Node> Nodes;
        //Spawning nodes
        private void SpawnNodes()
        {
            _nodes = new List<Node>();
            _nodeGrid = new Dictionary<Vector2Int, Node>();
            int currentLevelSize = Mathf.Min(GameManager.Instance.CurrentLevel + 4, levelmaxsize);
            float totalCellSize = _cellSize + _cellSpacing;
            Node spawnedNode;
            Vector3 spawnPos;

            //Spawning nodes for evry cell
            for (int i = 0; i < currentLevelSize; i++)
            {
                for (int j = 0; j < currentLevelSize; j++)
                {
                    spawnPos = new Vector3(i * totalCellSize + totalCellSize / 2f, j * totalCellSize + totalCellSize / 2f, 0f);
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
        //Getting color for hilghlight
        public Color GetHighLightColor(int colorId)
        {
            Color result = NodeColors[colorId];
            result.a = 0.4f;
            return result;
        }



        public List<Color> NodeColors;

        //Getting color ID
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
        //Drawingedges
        private Node startNode;

        private void Update()
        {
            if (hasGameFinished) return;

            if (Input.GetMouseButtonDown(0))
            {
                startNode = null;
                return;
            }
            if (Input.GetMouseButton(0))
            {   //Drawing connect with color
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
                //Highlight 

                startNode = null;
                _clickHighlight.gameObject.SetActive(false);


            }

        }

        private bool IsWithinBoardBounds(Vector2Int position)
        {
            return position.x >= 0 && position.x < BoardSize && position.y >= 0 && position.y < BoardSize;
        }
        #endregion

        #region WIN_CONDITION 
        //Win condition
        private void CheckWin()
        {
            if (CurrentLevelData == null || CurrentLevelData.Edges == null || CurrentLevelData.Edges.Count == 0)
            {
                Debug.LogError("CurrentLevelData or Edges is invalid!");
                return;
            }

            bool IsWinning = true;
            //Specifies pairs of nodes that must be connected to win
            foreach (var edge in CurrentLevelData.Edges)
            {
                Vector2Int startPos = edge.StartPoint;
                Vector2Int endPos = edge.EndPoint;
                //if Dictionary has these nodes
                if (!_nodeGrid.ContainsKey(startPos) || !_nodeGrid.ContainsKey(endPos))
                {
                    Debug.LogError($"Edge nodes not found: Start {startPos}, End {endPos}");
                    return;
                }
                //Getting end node and start node
                Node startNode = _nodeGrid[startPos];
                Node endNode = _nodeGrid[endPos];

                //Checking conditions for startnode
                bool startConnected = startNode.IsEndNode && startNode.ConnectedNodes.Count == 1;
                //Checking conditions for endNode
                bool endConnected = endNode.IsEndNode && endNode.ConnectedNodes.Count == 1;
                //Checking connections between two nodes
                bool areConnected = AreNodesConnected(startNode, endNode);

                Debug.Log($"Edge {startPos} -> {endPos}: StartConnected = {startConnected}, EndConnected = {endConnected}, AreConnected = {areConnected}");
                //Updating winnig flag
                IsWinning &= startConnected && endConnected && areConnected;
                if (!IsWinning)
                {
                    Debug.Log($"Win condition failed for edge {startPos} -> {endPos}");
                    return;
                }
            }

            Debug.Log("All edges connected successfully! Level won!");
            GameManager.Instance.UnlockLevelConnect();
            _winText.SetActive(true);
            _clickHighlight.gameObject.SetActive(false);
            hasGameFinished = true;
        }
        //Recursively checks if a path exists between two nodes via their ConnectedNodes
        private bool AreNodesConnected(Node start, Node end, HashSet<Node> visited = null)
        {
            //It keeps track of already visited nodes to avoid infinite loops in case of looped connections.
            if (visited == null) visited = new HashSet<Node>();
            if (start == end) return true;
            if (visited.Contains(start)) return false;

            visited.Add(start);
            //checking neighbour nodes
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
        public void ClickedBack()
        {
            GameManager.Instance.GoToMainMenu();
        }

        public void ClickedRestart()
        {
            GameManager.Instance.GoToGameplayConnect();
        }

        public void ClickedNextLevel()
        {
            if (!hasGameFinished) return;
           
            GameManager.Instance.GoToGameplayConnect();
        }
        #endregion

    }
}
