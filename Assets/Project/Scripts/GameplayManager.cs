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
            int currentLevelSize = GameManager.Instance.CurrentLevel + 4;

            var board = Instantiate(_boardPrefab, 
                new Vector3(currentLevelSize / 2f, currentLevelSize / 2f, 0f),
                Quaternion.identity);
            board.size = new Vector2(currentLevelSize + 0.08f, currentLevelSize + 0.08f);

            for (int i = 0; i < currentLevelSize; i++) { 
                for(int j = 0; i < currentLevelSize; i++)
                {
                    Instantiate(_bgcellPrefab, new Vector3(i + 0.5f, j + 0.5f, 0f), Quaternion.identity);
                }
            }
            Camera.main.orthographicSize = currentLevelSize + 2f;
            Camera.main.transform.position = new Vector3(currentLevelSize/2, currentLevelSize/2, -10f);

            _clickHighlight.size = new Vector2(currentLevelSize / 4, currentLevelSize / 4);
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
