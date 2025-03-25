using Connect.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connect.Common
{
    public class Node : MonoBehaviour
    {
        #region Node_Variables
        [SerializeField] private GameObject _point;
        [SerializeField] private GameObject _topEdge;
        [SerializeField] private GameObject _bottomEdge;
        [SerializeField] private GameObject _leftEdge;
        [SerializeField] private GameObject _rightEdge;
        [SerializeField] private GameObject _highlight;

        [HideInInspector] private Dictionary<Node, GameObject> ConnectEdges;

        [HideInInspector] public int colorId;

        public Vector2Int Pos2D { get; set; }
        #endregion

        #region DYNAMIC_VARIABLES
        //if node connected to another node ConnectNodes.Count = 2 than IsWin = true
        public bool IsWin
        {
            get
            {
                if (_point.activeSelf)
                {
                    return ConnectedNodes.Count == 1;
                }
                return ConnectedNodes.Count == 2;
            }
        }

        //If active-clickable if not-not clickable
        public bool IsClickable
        {
            get
            {
                if (_point.activeSelf)
                {
                    return true;
                }
                //If thero we cannot edit
                return ConnectedNodes.Count > 0;
            }
        }
        //Expression-bodied member(getter) if activeself == true if not false
        public bool IsEndNode => _point.activeSelf;

        public bool Iswin { get; internal set; }
        #endregion

        #region StartingMethods
        //Disable edges(display only node) 
        internal void Init()
        {
            _point.SetActive(false);
            _topEdge.SetActive(false);
            _bottomEdge.SetActive(false);
            _leftEdge.SetActive(false);
            _highlight.SetActive(false);

            _rightEdge.SetActive(false);

            ConnectEdges = new Dictionary<Node, GameObject>();
        }

        //Display color for nodes in level
        public void SetColorForPoint(int colorIdForSpawnNode)
        {
            colorId = colorIdForSpawnNode;
            _point.SetActive(true);
            Color color = GameplayManager.Instance.NodeColors[colorId];
            Debug.Log($"Setting color for node {gameObject.name}: {color}, Alpha: {color.a}");

            _point.GetComponent<SpriteRenderer>().color = GameplayManager.Instance.NodeColors[colorId];

        }

        //Seting edgedges for extending them
        public void SetEdge(Vector2Int offset, Node node)
        {

            if (offset == Vector2Int.up)
            {
                ConnectEdges[node] = _topEdge;
                return;
            }
            if (offset == Vector2Int.down)
            {
                ConnectEdges[node] = _bottomEdge;
                return;
            }
            if (offset == Vector2Int.right)
            {
                ConnectEdges[node] = _rightEdge;
                return;
            }
            if (offset == Vector2Int.left)
            {
                ConnectEdges[node] = _leftEdge;
                return;
            }


        }
        #endregion

        #region UPDATE_METHODS

        public void UpdateInput(Node connectedNode)
        {
            if (!ConnectEdges.ContainsKey(connectedNode))
            {
                return;
            }
            AddEdge(connectedNode);
        }
        [HideInInspector] public List<Node> ConnectedNodes;


        //Adding edge for node 
        private void AddEdge(Node connectedNode)
        {

            connectedNode.colorId = colorId;
            connectedNode.ConnectedNodes.Add(this);
            ConnectedNodes.Add(connectedNode);
            GameObject connectedEdge = ConnectEdges[connectedNode];
            connectedEdge.SetActive(true);
            connectedEdge.GetComponent<SpriteRenderer>().color = GameplayManager.Instance.NodeColors[colorId];
            Debug.Log($"Added edge: {this.name} <--> {connectedNode.name}, Total connections: {ConnectedNodes.Count}");
        }


        //Highlighting solving Node
        internal void SolveHighlight()
        {


        }

        #endregion
    }
}