using Connect.Common;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Connect.Core{
    
    public class GameplayManagerOneStroke : MonoBehaviour
    {
        /// <summary>
        /// Base Logic for One stroke Game
        /// </summary>
        #region startMethods

        [Header("UI")]
        [SerializeField] private GameObject _winText;
        [SerializeField] private GameObject _nextLevelButton;
        [SerializeField] private GameObject _restartButton;
        [SerializeField] private GameObject _backButton;
        [SerializeField] private TMP_Text _titleText;
        private bool _isDailyChallengeMode;

        [SerializeField] private LevelOneStroke _level;
        [SerializeField] private EdgeOneStroke _edgePrefab;
        [SerializeField] private PointOneStroke _pointPrefab;
        [SerializeField] private LineRenderer _highlight;

        private Dictionary<int, PointOneStroke> points;
        private Dictionary<Vector2Int, EdgeOneStroke> edges;
        private PointOneStroke startPoint, endPoint;
        private int currentId;
        private bool hasGameFinished;
        private Tween playStartTween;

        #endregion

        #region awakeMethods
        private void Awake()
        {
            hasGameFinished = false;
            points = new Dictionary<int, PointOneStroke>();
            edges = new Dictionary<Vector2Int, EdgeOneStroke>();
            _highlight.gameObject.SetActive(false);
            currentId = -1;
            SpawnLevel();

        }

        private void SpawnLevel()
        {
            _winText.SetActive(false);
            Vector3 camPos = Camera.main.transform.position;
            camPos.x = _level.Col * 0.5f;
            camPos.y = _level.Row * 0.5f;
            Camera.main.transform.position = camPos;
            Camera.main.orthographicSize = Mathf.Max(_level.Col, _level.Row) +2f;
            for (int i = 0; i < _level.Points.Count; i++)
            {
                Vector4 posData = _level.Points[i];
                Vector3 spawnPos = new Vector3(posData.x, posData.y, posData.z);
                int id = (int)posData.w;
                points[id] = Instantiate(_pointPrefab);
                points[id].Init(spawnPos, id);
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
            _winText.SetActive(true);

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

                _highlight.gameObject.SetActive(true);
                _highlight.positionCount = 2;
                _highlight.SetPosition(0, startPoint.Position);
                _highlight.SetPosition(1, startPoint.Position);
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

                _highlight.SetPosition(1, mousePos2D);

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
                    _highlight.SetPosition(0, startPoint.Position);
                    _highlight.SetPosition(1, startPoint.Position);
                }
                else if (IsEndAdd())
                {
                    currentId = endPoint.Id;
                    edges[new Vector2Int(startPoint.Id, endPoint.Id)].Add();

                    Debug.Log($" Edge added (End): {startPoint.Id} -> {endPoint.Id}");

                    CheckWin();

                    startPoint = endPoint;
                    _highlight.SetPosition(0, startPoint.Position);
                    _highlight.SetPosition(1, startPoint.Position);
                }
                else
                {
                    Debug.Log($" Cannot add edge: {startPoint.Id} -> {endPoint.Id}");
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Mouse released — reset highlight");

                _highlight.gameObject.SetActive(false);
                startPoint = null;
                endPoint = null;
                currentId = -1;

                CheckWin();
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



        public void ClickedBack(Button button)
        {
            AnimateAndSwitch(button, () =>
            {
                GameManager.Instance.GoToMainMenu();
            });
        }





        #endregion
    }
}

