using Connect.Common;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Connect.Core
{

    public class GameplayManagerPipes : MonoBehaviour
    {

        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _winText;
        [SerializeField] private Pipe _cellPrefab;

        private bool hasGameFinished;
        private Pipe[,] pipes;
        private List<Pipe> startPipes;


        public static GameplayManagerPipes Instance;

        private LevelDataPipe _currentLevelData;

        private void Awake()
        {
            Instance = this;

            _currentLevelData = GameManager.Instance.GetLevelPipes();
            SpawnLevel();
        }
        private void SpawnLevel()
        {
            pipes = new Pipe[_currentLevelData.Row, _currentLevelData.Col];

            for (int i = 0; i < _currentLevelData.Row; i++)
            {
                for (int j = 0; j < _currentLevelData.Col; j++)
                {
                    Vector2 spawnpos = new Vector2(j + 0.5f, i + 0.5f);

                    Pipe tempPipe = Instantiate(_cellPrefab);
                    tempPipe.transform.position = spawnpos;
                    tempPipe.Init(_currentLevelData.Data[i * _currentLevelData.Col + j]);
                    pipes[i,j] = tempPipe;
                    if (tempPipe.PipeType == 1)
                    {
                        startPipes.Add(tempPipe);
                    }
                }
            }
            Camera.main.orthographicSize = Mathf.Max(_currentLevelData.Row, _currentLevelData.Col);
            Vector3 cameraPos = Camera.main.transform.position;
            cameraPos.x = _currentLevelData.Col * 0.5f;
            cameraPos.y = _currentLevelData.Row * 0.5f;
            Camera.main.transform.position = cameraPos;
        }


    }
}

