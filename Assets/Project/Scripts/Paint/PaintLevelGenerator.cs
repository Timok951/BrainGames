using Connect.Core;
using System.Collections.Generic;

using UnityEngine;

namespace Connect.Common
{
    /// <summary>
    /// Class for level generation in Paint mode
    /// </summary>
    public class PaintLevelGenerator : MonoBehaviour
    {
        [SerializeField] private int _row, _col;
        [SerializeField] private PaintLevel _level;
        [SerializeField] private BlockPaint _blockPrefab;
        [SerializeField] private PlayerPaint _player;

        private BlockPaint[,] blocks;

        private void Awake()
        {
            CreateLevel();
            SpawnLevel();
        }

        private void CreateLevel()
        {
            if (_level.Row == _row && _level.Col == _col) return;

            _level.Row = _row;
            _level.Col = _col;
            _level.Start = Vector2Int.zero;

            _level.Data = new List<int>(_row * _col);
            for (int i = 0; i < _row * _col; i++)
            {
                _level.Data.Add(0); // пустой блок
            }

        }

        private void SpawnLevel()
        {
            Vector3 camPos = Camera.main.transform.position;
            camPos.x = _level.Col * 0.5f - 0.5f;
            camPos.y = _level.Row * 0.5f - 0.5f;
            Camera.main.transform.position = camPos;
            Camera.main.orthographicSize = Mathf.Max(_level.Row, _level.Col) + 2f;

            _player.Init(_level.Start, _level.Row, _level.Col);

            blocks = new BlockPaint[_level.Row, _level.Col];

            for (int row = 0; row < _level.Row; row++)
            {
                for (int col = 0; col < _level.Col; col++)
                {
                    blocks[row, col] = Instantiate(_blockPrefab, new Vector3(col, row, 0), Quaternion.identity, transform);
                    blocks[row, col].Init(_level.Data[row * _level.Col + col]);
                }
            }
        }

        private void Update()
        {
            HandleLeftClick();
            HandleRightClick();
        }

        private void HandleLeftClick()
        {
            if (!Input.GetMouseButtonDown(0)) return;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int gridPos = new Vector2Int(
                Mathf.FloorToInt(mousePos.x + 0.5f),
                Mathf.FloorToInt(mousePos.y + 0.5f)
            );

            if (!IsValid(gridPos)) return;

            int currentFill = _level.Data[gridPos.y * _col + gridPos.x];
            currentFill = currentFill == 1 ? 0 : 1;
            _level.Data[gridPos.y * _col + gridPos.x] = currentFill;

            blocks[gridPos.y, gridPos.x].Init(_level.Data[gridPos.y * _col + gridPos.x]);

        }

        private void HandleRightClick()
        {
            if (!Input.GetMouseButtonDown(1)) return;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int gridPos = new Vector2Int(
                Mathf.FloorToInt(mousePos.x + 0.5f),
                Mathf.FloorToInt(mousePos.y + 0.5f)
            );

            if (!IsValid(gridPos)) return;

            _level.Start = gridPos;
            _player.Init(_level.Start, _level.Row, _level.Col);
        }

        private bool IsValid(Vector2Int pos)
        {
            return pos.x >= 0 && pos.y >= 0 && pos.x < _col && pos.y < _row;
        }
    }
}
