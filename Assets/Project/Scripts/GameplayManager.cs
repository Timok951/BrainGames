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

        [HideInInspector] public bool hasGameFinished;

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

            CurrentLevelData = GameManager.Instance.GetLevel();
            SpawndBoard();
            SpawnNodes();
        }
        #endregion

        #endregion

        #region BOARD_SPAWN
        [SerializeField] private SpriteRenderer _boardPrefab, _bgcellPrefab;
        private void SpawndBoard()
        {
            int currentLevelSize = Mathf.Min(GameManager.Instance.CurrentLevel + 1, 14);
            float totalCellSize = _cellSize + _cellSpacing;
            float boardSize = currentLevelSize * totalCellSize;
            float boardScaleFactor = 1.1f;

            var board = Instantiate(_boardPrefab,
                new Vector3(boardSize / 2f, boardSize / 2f, 0f),
                Quaternion.identity);

            board.size = new Vector2(boardSize, boardSize);
            //boardoffset
            float startX = (boardSize - totalCellSize * currentLevelSize) / 2f + totalCellSize / 2f;
            float startY = (boardSize - totalCellSize * currentLevelSize) / 2f + totalCellSize / 2f;

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
            Camera.main.orthographicSize = (boardSize * boardScaleFactor) / 2f + 1f;
            Camera.main.transform.position = new Vector3(boardSize / 2f, boardSize / 2f, -10f);

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
            int currentLevelSize = Mathf.Min(GameManager.Instance.CurrentLevel + 2, 14);
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
                    if (startNode.colorId != tmpNode.colorId && tmpNode.IsEndNode)
                    {
                        return;
                    }
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
        #endregion

        #region WIN_CONDITION 
        //Win condition
        private void CheckWin()
        {
            bool IsWinning = true;

            foreach (var item in _nodes)
            {
                item.SolveHighlight();
            }
            //if evry node in win position
            foreach (var item in _nodes)
            { //IsWinning = IsWinning && item.Iswin;
                IsWinning &= item.Iswin;
                if (!IsWinning)
                {
                    return;
                }

            }
            //Unlock new level and hilight the text 
            GameManager.Instance.UnlockLevel();
            _winText.gameObject.SetActive(true);
            _clickHighlight.gameObject.SetActive(false);

            hasGameFinished = true;
        }
        #endregion

        #region BUTTON_FUNCTIONS
        public void ClickedBack()
        {
            GameManager.Instance.GoToMainMenu();
        }

        public void ClickedRestart()
        {
            GameManager.Instance.GoToGameplay();
        }

        public void ClickedNextLevel()
        {
            if (!hasGameFinished) return;
           
            GameManager.Instance.GoToGameplay();
        }
        #endregion

    }
}
