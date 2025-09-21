using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connect.Core{
    public class PointOneStroke : MonoBehaviour
    {
        [HideInInspector] public int Id;
        [HideInInspector] public Vector3 Position;

        public void Init(Vector3 pos, int id)
        {
            Id = id;
            Position = pos;
            transform.position = Position;
        }


    }

}
