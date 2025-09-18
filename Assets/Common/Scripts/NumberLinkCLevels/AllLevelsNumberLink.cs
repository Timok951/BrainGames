using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connect.Common
{
    [CreateAssetMenu(fileName = "Level", menuName = "NumberLink/Levels")]

    public class AllLevelsNumberLink : ScriptableObject
    {
        public List<NumberLinkLevel> Levels;

    }
}

