using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connect.Common
{
    [CreateAssetMenu(fileName = "AllLevelsPipes", menuName = "Pipes/AllLevelsPipes")]

    public class AllLevelsPipes : ScriptableObject
    {
        public List<LevelDataPipe> LevelsPipes;
    }
}

