using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connect.Common{
    [CreateAssetMenu(fileName = "Level", menuName = "ColorSort/Level")]
    public class LevelColorSort : ScriptableObject
    {
        public string LevelName;

        public Color BackgroundColor;

        public Color TopLeftColor;
        public Color TopRightColor;
        public Color BottomLeftColor;
        public Color BottomRightColor;

        public int Row;
        public int Col;

        public List<Vector2Int> LockedCells;

    }

}
