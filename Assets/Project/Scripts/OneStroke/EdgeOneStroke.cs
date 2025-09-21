using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connect.Core
{
    public class EdgeOneStroke : MonoBehaviour
    {
        #region variables
        [HideInInspector] public bool Filled;

        [SerializeField] private LineRenderer _line;
        [SerializeField] private Gradient _startColor;
        [SerializeField] private Gradient _activeColor;

        public void Init(Vector3 start, Vector3 end)
        {
            _line.useWorldSpace = true;
            _line.positionCount = 2;
            _line.SetPosition(0, start);
            _line.SetPosition(1, end);
            _line.colorGradient = _startColor;
            Filled = false;
        }

        public void Add()
        {
            Filled = true;
            _line.colorGradient = _activeColor;
        } 

    }
    #endregion
}
