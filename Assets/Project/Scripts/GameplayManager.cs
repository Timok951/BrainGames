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
        [SerializeField] private float _boardPadding = 0.08f; 
        private void Awake()
        {
            Instance = this;

            hasGameFinished = false;
            _winText.SetActive(false);
            _titleText.gameObject.SetActive(true);
            _titleText.text = "Level " + GameManager.Instance.CurrentLevel.ToString();

            SpawndBoard();
        }
        #endregion

        #endregion

        #region BOARD_SPAWN
        [SerializeField] private SpriteRenderer _boardPrefab, _bgcellPrefab;
        private void SpawndBoard()
        {
            int currentLevelSize = Mathf.Min(GameManager.Instance.CurrentLevel + 2, 14);
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
        public Dictionary<Vector2Int, Node> _nodeGridsS;


        public Dictionary<Vector2Int, Node> Nodes;

        private void SpawnNodes()
        {

        }

        public List<Color> NodeColors;

        #endregion
        #region Update_Methods
        #endregion

        #region WIN_CONDITION 

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
