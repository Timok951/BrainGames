using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connect.Common
{

    /// <summary>
    /// Scriptable object for level
    /// </summary>

    [CreateAssetMenu (fileName = "PaintLevel", menuName ="Paint/PaintLevel")]
    public class PaintLevel : ScriptableObject
    {
        public string LevelName;
        public int Row;
        public int Col;
        public Vector2Int Start;
        public List<int> Data;

    }

}
