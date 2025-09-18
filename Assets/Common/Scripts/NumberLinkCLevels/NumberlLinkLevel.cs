using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connect.Common
{
    [CreateAssetMenu(fileName = "Level", menuName = "NumberLink/Level")]
    public class NumberLinkLevel : ScriptableObject
    {
        public string LevelName;  
        public int Rows;           
        public int Columns;        

        
        public List<int> Data;     
    }
}

