using Codice.CM.Common;
using Connect.Common;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Connect.Core
{
    /// <summary>
    /// Class for generating level for one stroke 
    /// </summary>
    public class LevelGenerator : MonoBehaviour
    {
        #region LevelGeneragor region 
        #region startMethods

        [Header("UI")]
        private bool _isDailyChallengeMode;
        [SerializeField] private int _row, _col;    
        [SerializeField] private LevelOneStroke _level;
        [SerializeField] private EdgeOneStroke _edgePrefab;
        [SerializeField] private PointOneStroke _pointPrefab;

        private static int spawnId;

        private Dictionary<int, PointOneStroke> points;
        private Dictionary<Vector2Int, EdgeOneStroke> edges;
        private PointOneStroke startPoint, endPoint;
        private int currentId;
        private PointOneStroke startSpawnPoint, endSpawnPoint;
        private bool hasGameFinished;

        #endregion

        #region awakeMethods
        private void Awake()
        {
            hasGameFinished = false;
            points = new Dictionary<int, PointOneStroke>();
            edges = new Dictionary<Vector2Int, EdgeOneStroke>();
            currentId = -1;
            CreateLevel();
            SpawnLevel();

        }

        private void SpawnLevel()
        {
            Vector3 camPos = Camera.main.transform.position;
            camPos.x = _level.Col * 0.5f;
            camPos.y = _level.Row * 0.5f;
            Camera.main.transform.position = camPos;
            Camera.main.orthographicSize = Mathf.Max(_level.Col, _level.Row) + 2f;
            for (int i = 0; i < _level.Points.Count; i++)
            {
                Vector4 posData = _level.Points[i];
                Vector3 spawnPos = new Vector3(posData.x, posData.y, posData.z);
                int id = (int)posData.w;
                points[id] = Instantiate(_pointPrefab);
                points[id].Init(spawnPos, id);
                spawnId = id + 1;
            }
            for (int i = 0; i < _level.Edges.Count; i++)
            {
                Vector2Int normal = _level.Edges[i];
                Vector2Int reversed = new Vector2Int(normal.y, normal.x);
                EdgeOneStroke spawnEdge = Instantiate(_edgePrefab);
                edges[normal] = spawnEdge;
                edges[reversed] = spawnEdge;
                spawnEdge.Init(points[normal.x].Position, points[normal.y].Position);
                spawnEdge.gameObject.SetActive(true);

            }

        }
        #endregion

        #region UpdateMethods 
        private void CheckWin()
        {
            foreach (var item in edges)
            {
                if (!item.Value.Filled)
                    return;
            }
            hasGameFinished = true;

        }


        private void Update()
        {
            if (hasGameFinished) return;

            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Mouse Down detected!");

                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                if (!hit)
                {
                    Debug.Log("No collider hit!");
                    return;
                }

                startPoint = hit.collider.GetComponent<PointOneStroke>();
                if (startPoint == null)
                {
                    Debug.Log("Clicked object is not PointOneStroke");
                    return;
                }

                Debug.Log($"Start point selected: {startPoint.Id}");

            }
            else if (Input.GetMouseButton(0) && startPoint != null)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                if (hit)
                {
                    endPoint = hit.collider.GetComponent<PointOneStroke>();
                }

                if (endPoint == null)
                {
                    Debug.Log("No end point selected");
                    return;
                }

                if (startPoint == endPoint)
                {
                    Debug.Log("Start == End, skipping");
                    return;
                }

                Debug.Log($"Trying edge: {startPoint.Id} -> {endPoint.Id}");

                if (IsStartAdd())
                {
                    currentId = endPoint.Id;
                    edges[new Vector2Int(startPoint.Id, endPoint.Id)].Add();

                    Debug.Log($" Edge added (Start): {startPoint.Id} -> {endPoint.Id}");

                    startPoint = endPoint;

                }
                else if (IsEndAdd())
                {
                    currentId = endPoint.Id;
                    edges[new Vector2Int(startPoint.Id, endPoint.Id)].Add();

                    Debug.Log($" Edge added (End): {startPoint.Id} -> {endPoint.Id}");


                    startPoint = endPoint;

                }
                else
                {
                    Debug.Log($" Cannot add edge: {startPoint.Id} -> {endPoint.Id}");
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Mouse released — reset highlight");

                startPoint = null;
                endPoint = null;
                currentId = -1;

                CheckWin();
            }
            if (Input.GetKeyDown(KeyCode.Space)) {

                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

                if(hit)return;
                int id = spawnId++;
                Vector3 spawnPos = new Vector3(mousePos.x, mousePos.y, id);
                Vector4 point = new Vector4(mousePos.x, mousePos.y,0 ,id);
                _level.Points.Add(point);

                points[id] = Instantiate(_pointPrefab);
                points[id].Init(spawnPos, id);
                EditorUtility.SetDirty(_level);
            }
            if (Input.GetKeyDown(KeyCode.A)) {

                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                if(!hit) return;
                startSpawnPoint = hit.collider.gameObject.GetComponent<PointOneStroke>();

            }
            if (Input.GetKeyDown(KeyCode.D))
            {

                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                if (!hit) return;
                endSpawnPoint = hit.collider.gameObject.GetComponent<PointOneStroke>();
            }

            if (Input.GetKeyDown(KeyCode.W)) {
            
                if(startPoint == null || endPoint == null) return;
                if (startSpawnPoint.Id == endSpawnPoint.Id) return;
                Vector2Int normal = new Vector2Int(startSpawnPoint.Id, endSpawnPoint.Id);
                Vector2Int reversed = new Vector2Int(startSpawnPoint.Id, endSpawnPoint.Id);
                if (edges.ContainsKey(normal)) return;
                EdgeOneStroke spawnEdge = Instantiate(_edgePrefab);
                edges[normal] = spawnEdge;
                edges[reversed] = spawnEdge;
                spawnEdge.Init(points[normal.x].Position, points[normal.y].Position);
                spawnEdge.gameObject.SetActive(true);
                _level.Edges.Add(normal);
                EditorUtility.SetDirty(_level);
            }
        }

        private bool IsStartAdd()
        {
            if (currentId != -1)
            {
                Debug.Log("IsStartAdd failed: currentId already set");
                return false;
            }

            Vector2Int edge = new Vector2Int(startPoint.Id, endPoint.Id);
            if (!edges.ContainsKey(edge))
            {
                Debug.Log($"IsStartAdd failed: edge {startPoint.Id}->{endPoint.Id} not in edges dict");
                return false;
            }

            Debug.Log("IsStartAdd OK");
            return true;
        }


        private bool IsEndAdd()
        {
            if (currentId != startPoint.Id)
            {
                Debug.Log($"IsEndAdd failed: currentId {currentId} != startPoint.Id {startPoint.Id}");
                return false;
            }

            Vector2Int edge = new Vector2Int(startPoint.Id, endPoint.Id);
            if (edges.TryGetValue(edge, out EdgeOneStroke result))
            {
                if (result == null || result.Filled)
                {
                    Debug.Log("IsEndAdd failed: edge already filled or null");
                    return false;
                }

                Debug.Log("IsEndAdd OK");
                return true;
            }

            Debug.Log("IsEndAdd failed: edge not found in dict");
            return false;
        }

        #endregion

        #endregion

        #region CreatingLevelregion 
        private void CreateLevel()
        {
            if(_level.Row == _row && _level.Col == _col)
            {
                return;
            }

            _level.Row = _row;
            _level.Col = _col;
            _level.Points = new List<Vector4>();
            _level.Edges = new List<Vector2Int>();
            spawnId = 0;
        }
        #endregion
    }
}


