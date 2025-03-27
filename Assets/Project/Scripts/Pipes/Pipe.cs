using log4net.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Transform = UnityEngine.Transform;

namespace Connect.Core
{
    public class Pipe : MonoBehaviour
    {
        #region basic_variables
        [HideInInspector] public bool IsFilled;
        [HideInInspector] public int PipeType;

        [SerializeField] private UnityEngine.Transform[] _pipePrefabs;

        private UnityEngine.Transform currentPipe;
        private int rotation;

        private SpriteRenderer emptySprite;

        private SpriteRenderer filledSprite;
        private List<Transform> connectBoxes;
        private const int minRotation = 0;
        private const int maxRotation = 3;
        private const int rotatinMultiplier = 90;
        #endregion

        #region START_VARIABLES
        internal void Init(int pipe)
        {
            PipeType = pipe % 10;
            currentPipe = Instantiate(_pipePrefabs[PipeType], transform);
            currentPipe.transform.localPosition = Vector3.zero; 

            //RotatingSprite
            if(PipeType == 1 || PipeType == 2)
            {
                rotation = 0 / 10;

            }
            else
            {
                rotation = UnityEngine.Random.Range(minRotation, maxRotation + 1);
            }

            //FillingPipe
            if(PipeType ==0 || PipeType == 1)
            {
                IsFilled = true;
            }
            if (PipeType == 0)
            {
                //Returning Nothing
                return;
            }



            //Getting PipeFilled 
            currentPipe.transform.eulerAngles = new Vector3(0,0,rotation * rotatinMultiplier);
            emptySprite = currentPipe.GetChild(0).GetComponent<SpriteRenderer>();
            emptySprite.gameObject.SetActive(!IsFilled);
            emptySprite = currentPipe.GetChild(1).GetComponent<SpriteRenderer>();
            filledSprite.gameObject.SetActive(IsFilled);

            //Adding connectBoxes
            connectBoxes = new List<Transform>();
            for (int i = 2; i < currentPipe.childCount; i++){

                connectBoxes.Add(currentPipe.GetChild(i));

            }

        }
        #endregion


    }
}

