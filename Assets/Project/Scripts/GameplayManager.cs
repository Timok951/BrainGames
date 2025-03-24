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
            int currentLevelSize = Mathf.Min(GameManager.Instance.CurrentLevel + 1, 14);
            float totalCellSize = _cellSize + _cellSpacing;


            float boardSize = currentLevelSize * totalCellSize; 

            float boardOffsetX = -((currentLevelSize - 1) * totalCellSize) / 2f;
            float boardOffsetY = -((currentLevelSize - 1) * totalCellSize) / 2f;
            var board = Instantiate(_boardPrefab, Vector3.zero,Quaternion.identity);
            board.transform.localScale = new Vector2(currentLevelSize, currentLevelSize);
            Debug.Log(totalCellSize + " " + boardSize);

            for (int i = 0; i < currentLevelSize; i++)
            {
                for (int j = 0; j < currentLevelSize; j++)
                {
                    Instantiate(_bgcellPrefab,
                        new Vector3(i * totalCellSize + boardOffsetX, j * totalCellSize + boardOffsetY, 0f),
                        Quaternion.identity);
                }
            }


           Camera.main.orthographicSize = currentLevelSize + 2f;

            
            _clickHighlight.size = new Vector2(boardSize / 4, boardSize / 4);
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
