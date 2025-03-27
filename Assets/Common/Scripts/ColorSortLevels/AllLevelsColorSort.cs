using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connect.Common
{
    [CreateAssetMenu(fileName = "Level", menuName = "ColorSort/AllLevelsColorSort")]

    public class AllLevelsColorSort : ScriptableObject
    {
        public List<LevelColorSort> Levels;
    }
}