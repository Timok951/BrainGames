using Connect.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connect.Core
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
                Debug.Log($"Node {name}: Connected nodes = {ConnectedNodes.Count}, ConnectedNodes = {ConnectedNodes.Count}");

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
            ConnectedNodes = new List<Node>();
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

        //Updating nodee
        public void UpdateInput(Node connectedNode)
        {
            //Invalid Input
            if (!ConnectEdges.ContainsKey(connectedNode))
            {
                Debug.Log($"{connectedNode.name} is not a neighbor of {name}");
                return;
            }

            //Connected Node already exist
            //Delete the Edge and the parts
            if (ConnectedNodes.Contains(connectedNode))
            {
                DeleteChain(this);
                return;

            }

            if (IsEndNode && ConnectedNodes.Count >= 1)
            {
                Debug.Log($"{name} is an end node and already has a connection");
                return;
            }

            if (connectedNode.IsEndNode && connectedNode.ConnectedNodes.Count >= 1)
            {
                Debug.Log($"{connectedNode.name} is an end node and already has a connection");
                return;
            }

            AddEdge(connectedNode);
            Debug.Log($"Connected {name} -> {connectedNode.name}");


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



        private void DeleteChain(Node startNode, HashSet<Node> visited = null)
        {
            if (visited == null) visited = new HashSet<Node>();
            if (visited.Contains(startNode)) return;

            visited.Add(startNode);
            Debug.Log($"Deleting node: {startNode.name}, Connections: {startNode.ConnectedNodes.Count}");

            List<Node> nodesToDelete = new List<Node>(startNode.ConnectedNodes);

            foreach (var node in nodesToDelete)
            {
                startNode.ConnectedNodes.Remove(node);
                node.ConnectedNodes.Remove(startNode);
                RemoveEdge(node);
                DeleteChain(node, visited);
            }
        }

        //RemoveEdge if we clicked on past cell

        private void RemoveEdge(Node node)
        {
            if (ConnectEdges.ContainsKey(node))
            {
                GameObject edge = ConnectEdges[node];
                edge.SetActive(false);
            }
            if (node.ConnectEdges.ContainsKey(this))
            {
                GameObject edge = node.ConnectEdges[this];
                edge.SetActive(false);
            }
        }

        private void DeleteNode()
        {
            Node startNode = this;
            if (startNode.IsConnectedToEndNode())
            {
                return;
            }
            while (startNode != null)
            {
                Node tempNode = null;
                if(startNode.ConnectedNodes.Count != 0)
                {
                    tempNode = startNode.ConnectedNodes[0];
                    startNode.ConnectedNodes.Clear();
                    tempNode.ConnectedNodes.Remove(startNode);
                    startNode = tempNode;
                    startNode.RemoveEdge(tempNode);
                }
                startNode = tempNode;
            }
        }
         
        //Checking if node was connected to other node
        public bool IsConnectedToEndNode(List<Node> checkedNode=null)
        {
            if (checkedNode == null)
            {
                checkedNode = new List<Node>();
            }
            if (IsEndNode)
            {
                return true;
            }
            foreach(var item in ConnectedNodes)
            {
                if (!checkedNode.Contains(item))
                {
                    checkedNode.Add(item);
                    return item.IsConnectedToEndNode(checkedNode);
                }
            }

            return false;
        }


        //Highlighting solving Node
        internal void SolveHighlight()
        {


        }

        #endregion
    }
}