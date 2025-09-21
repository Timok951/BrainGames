using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Connect.Common
{     /// <summary>
      /// Class for one stroke level
      /// </summary>
    [CreateAssetMenu(fileName = "LevelOneStroke", menuName = "OneStrokeLevels/LevelOneStroke")]

    public class LevelOneStroke : ScriptableObject
    {
        public string LevelName;
        public int Row, Col;
        public List<Vector4> Points;
        public List<Vector2Int> Edges;

    }

}

