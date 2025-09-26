using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connect.Common
{
    [CreateAssetMenu(fileName = "AllLevelsPaint", menuName = "Paint/AllLevels")]

    public class AllLevelsPaint : ScriptableObject
    {
        public List<PaintLevel> Levels;

    }

}
