using JetBrains.Annotations;
using log4net.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Transform = UnityEngine.Transform;

namespace Connect.Core
{
    public class Pipe : MonoBehaviour
    {
        #region basic_variables
        [HideInInspector] public bool IsFilled;
        //Pipetype will be int to represent multiploe pipes
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
            //Pipes will be two number value we need extract rotation and pipetype
            PipeType = pipe % 10;
            currentPipe = Instantiate(_pipePrefabs[PipeType], transform);
            currentPipe.transform.localPosition = Vector3.zero;

            if (_pipePrefabs == null || _pipePrefabs.Length <= PipeType || _pipePrefabs[PipeType] == null)
            {
                Debug.LogError($"Pipe prefab for type {PipeType} is null or invalid!");
                return;
            }

            //Rotating Sprite 
            if (pipe >= 10 || PipeType != 1 || PipeType != 2)
            {
                rotation = pipe / 10;

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
            filledSprite = currentPipe.GetChild(1).GetComponent<SpriteRenderer>();
            filledSprite.gameObject.SetActive(IsFilled);

            //Adding connectBoxes
            connectBoxes = new List<Transform>();
            for (int i = 2; i < currentPipe.childCount; i++){

                connectBoxes.Add(currentPipe.GetChild(i));

            }

        }
        #endregion
        #region UPDATE_FUNCTIONS
        public void UpdateInput()
        {

            if (PipeType == 0 ||  PipeType == 1 || PipeType == 2)
            {
                return;
            }

            //MaxRotation will be walue 3
            rotation = (rotation + 1) % (maxRotation + 1);
            //Increasing rotation value by 1
            currentPipe.transform.eulerAngles = new Vector3(0,0, rotation * rotatinMultiplier);

            
        }

        //Updating filled pipe
        public void UpdateFilled()
        {
            if (PipeType == 0) return;
            emptySprite.gameObject.SetActive(!IsFilled);
            filledSprite = currentPipe.GetChild(1).GetComponent<SpriteRenderer>();
            filledSprite.gameObject.SetActive(IsFilled);
        }

        public List<Pipe> ConnectedPipes()
        {
            List<Pipe> result = new List<Pipe>();
            foreach (var box in connectBoxes)
            {
                RaycastHit2D[] hit = Physics2D.RaycastAll(box.transform.position, Vector2.zero, 0.1f);
                Debug.Log($"Checking box at {box.transform.position}, hits: {hit.Length}");                //if we hit pipe
                for (int i = 0; i < hit.Length; i++)
                {
                    //Adding pipe to result
                    Pipe connectedPipe = hit[i].transform.parent.parent.GetComponent<Pipe>();
                    if (connectedPipe != null && connectedPipe != this) // Исключаем саму трубу
                    {
                        result.Add(connectedPipe);
                        Debug.Log($"Connected to pipe at {connectedPipe.transform.position} (Type: {connectedPipe.PipeType})");
                    }
                }
            }
            return result;
        }
        #endregion

    }
}

