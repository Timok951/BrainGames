using Connect.Common;
using Connect.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;

namespace Connect.Core
{

    /// <summary>
    /// Gameplay manager for Numberlink level that connect basic logic for game. Core logic
    /// </summary>
    #region StartVariables
    public class GameplayNumberlink : MonoBehaviour
    {
        public static GameplayNumberlink Instance;

        public bool hasGameFinished;

        public float Edgesize => _cellGap + _cellSize;

        [SerializeField] private CellNumberlink _cellprefab;
        [SerializeField] private SpriteRenderer _bgSprite;
        [SerializeField] private SpriteRenderer _highlightSprite;
        [SerializeField] private Vector2 _highlightSize;
        [SerializeField] private LevelDataNumberLinks _levelData;
        [SerializeField] private float _cellGap;
        [SerializeField] private float _cellSize;
        [SerializeField] private float _levelGap;

        private int[,] leveGrid;
        private CellNumberlink[,] cellGrid;
        private CellNumberlink startCell;
        private Vector2 startPos;

        private List<Vector2Int> Directions = new List<Vector2Int>() {

            // Vector2Int representation of 2D vectors and points using
            // integers This structure is used in some places to
            // represent 2D positions and vectors that don't
            // require the precision of floating-point.
            Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left

        };
        #endregion

        private void SpawnLevel()
        {
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
                    tempCell.Init(i,j,leveGrid[i, j]);
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
                    {
                        cellGrid[i, j].Init();
                    }
                }
            }

        }
        //Core Logic Method
        private void Update()
        {
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
                    // Try to add edge
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

                // Set rotation
                Vector3 angle = new Vector3(0, 0, 90f * (directionIndex - 1));
                _highlightSprite.transform.eulerAngles = angle;

                // Edge size change
                _highlightSprite.size = new Vector2(_highlightSize.x, offsetValue);

                // Move position
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
        }
        private int GetDirectionIndex(Vector2Int offsetDirection)
        {
            int result = 0;
            if (offsetDirection == Vector2Int.right) {

                result =  0;
            
            }
            if (offsetDirection == Vector2Int.left)
            {
                result = 2;
            }
            if(offsetDirection == Vector2Int.up)
            {
                result = 1;
            }
            if(offsetDirection == Vector2Int.down)
            {
                result = 3;
            }

            return result;
        }

        private float GetOffset(Vector2 offset, Vector2Int offsetDirection)
        {
            float result = 0;
            if(offsetDirection == Vector2Int.left || offsetDirection == Vector2Int.right)
            {
                result = Mathf.Abs(offset.x);
            }
            if(offsetDirection == Vector2Int.up || offsetDirection == Vector2Int.down)
            {
                result = Mathf.Abs(offset.y);
            }
            return result;
        }

        private Vector2Int GetDirection(Vector2 offset)
        {
            Vector2Int result = Vector2Int.zero;

            if(Mathf.Abs(offset.y) > Mathf.Abs(offset.x) && offset.y > 0)
            {
                result = Vector2Int.up; 
            }
            if(Mathf.Abs(offset.y) > Mathf.Abs(offset.x) && offset.y < 0)
            {
                result = Vector2Int.down;
            }
            if (Mathf.Abs(offset.y) < Mathf.Abs(offset.x) && offset.x > 0)
            {
                result = Vector2Int.right;
            }
            if (Mathf.Abs(offset.y) < Mathf.Abs(offset.x) && offset.x < 0)
            {
                result = Vector2Int.left;
            }
            return result;
        }

        public CellNumberlink GetAdjacentCell(int row, int col, int direction)
        {
            Vector2Int currentDirection = Directions[direction];
            Vector2Int startPos = new Vector2Int(row, col);
            Vector2Int checkPos = startPos + currentDirection;
            while(IsValid(checkPos) && cellGrid[checkPos.x, checkPos.y] == null)
            {
                checkPos += currentDirection;
            }
            return IsValid(checkPos) ? cellGrid[checkPos.x, checkPos.y] : null;
        }

        //checking is vector 2 pos valid
        public bool IsValid(Vector2Int pos)
        {
            return pos.x >= 0 && pos.y >= 0 && pos.x < _levelData.row && pos.y < _levelData.Col;
        }

        #region WakeMethod
        //Initialise
        private void Awake()
        {
            Instance = this;
            hasGameFinished = false;
            _highlightSprite.gameObject.SetActive(false);

            leveGrid = new int[_levelData.row, _levelData.Col];
            cellGrid = new CellNumberlink[_levelData.row, _levelData.Col];

            for (int i = 0; i < _levelData.row; i++)
            {
                for (int j = 0; j < _levelData.Col; j++)
                {
                    leveGrid[i, j] = _levelData.data[i*_levelData.row + j];
                }
            }

            SpawnLevel();
        }

    }

    #endregion

    [Serializable]
    public struct LevelDataNumberLinks
    {
        public int row, Col;
        public List<int> data;
    }


}

