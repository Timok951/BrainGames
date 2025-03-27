using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connect.Common
{
    [CreateAssetMenu(fileName = "LevelPipe", menuName = "Pipes/Level")]

    public class LevelDataPipe : ScriptableObject
    {
        public string LevelName;
        public int Row;
        public int Col;
        public List<int> Data;
    }
}
