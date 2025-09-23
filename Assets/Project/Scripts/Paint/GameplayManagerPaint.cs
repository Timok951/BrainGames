using Connect.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Connect.Core
{
    /// <summary>
    /// Class for main logig from paint mode
    /// </summary>
    public class GameplayManagerPaint : MonoBehaviour
    {
        #region startvariables
        public static GameplayManagerPaint Instance;

        [HideInInspector] public bool CanClick;

        [SerializeField] private PaintLevel _level;
        [SerializeField] private BlockPaint _blockPrefab;
        [SerializeField] PlayerPaint _player;

        private BlockPaint[,] blocks;
        private Vector2 start, end;

        private bool hasGameFinishide;
        #endregion

        private void Awake()
        {
            Instance = this;
            hasGameFinishide = false;
            CanClick = true;
            SpawnLevel();
        }

        private void SpawnLevel()
        {
            Vector3 camPos = Camera.main.transform.position;
            camPos.x = _level.Col * 0.5f - 0.5f;
            camPos.y = _level.Row * 0.5f - 0.5f;
            Camera.main.transform.position = camPos;
            Camera.main.orthographicSize = Mathf.Max(_level.Row, _level.Col) +2f;

            _player.Init(_level.Start);

            blocks = new BlockPaint[_level.Row, _level.Col];

            for (int i =0; i<_level.Row; i++)
            {
                for (int j = 0; j < _level.Col; j++)
                {
                    blocks[i, j] = Instantiate(_blockPrefab, new Vector3(j, i, 0), Quaternion.identity, transform);
                    blocks[i, j].Init(_level.Data[i * _level.Col + j]);
                }
            }

        }
        #region Updateregion

        private void Update()
        {
            if (hasGameFinishide || !CanClick) return;

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                start = new Vector2(mousePos.x, mousePos.y);
            }
            else if (Input.GetMouseButton(0))
            {
                Vector3 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                end = new Vector2(mousePos.x, mousePos.y);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Vector2Int  direction = GetDirection();
                Vector2Int offset = GetOffsetEndPos(direction);
                if (offset == Vector2.zero) return;
                StartCoroutine(_player.Move(offset, offset.x == 0 ? offset.y : offset.x));
                CanClick = false;
            }

        }



        private Vector2Int GetDirection()
        {
            Vector2 offset = end - start;
            if(Mathf.Abs(offset.x) > Mathf.Abs(offset.y))
            {
                return offset.x > 0 ? Vector2Int.up : Vector2Int.down;
            }
            return offset.y > 0 ? Vector2Int.right : Vector2Int.left;

        }

        private Vector2Int GetOffsetEndPos(Vector2Int direction)
        {
            Vector2Int result = direction;
            Vector2Int checkPos = _player.Pos + result;

            while (IsValid(checkPos) && !blocks[checkPos.x, checkPos.y].Blocked)
            {
                result += direction;
                checkPos += direction;
            }
            return result - direction;

        }

        private bool IsValid(Vector2Int pos)
        {
            return pos.x >= 0 && pos.y >= 0 && pos.x < _level.Row && pos.y < _level.Col; 
        }

        public void HighLightBlock(int x, int y)
        {
            blocks[y, x].Add();
        }

        public void CheckWin()
        {
            for (int i = 0; i < _level.Row; i++)
            {
                for (int j = 0; j < _level.Row; j++)
                {
                    if (!blocks[i, j].Filled) return;
                }
            }
            hasGameFinishide = true;
        }
        #endregion




    }
}

