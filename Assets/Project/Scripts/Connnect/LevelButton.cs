using Connect.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Connect.Core{

    /// <summary>
    /// Class that controls connect button levels
    /// </summary>
    public class LevelButtonConnect : MonoBehaviour
    {

        [SerializeField] private Button _button;
        [SerializeField] TMP_Text _leveltext;
        [SerializeField] private Color _inactiveColor;
        [SerializeField] private Image _image;

        private bool isLevelUnlocked;
        private int currentLevel;

        private void Awake()
        {
            _button.onClick.AddListener(Clicked);
        }

        private void OnEnable()
        {
            MainMenuManager.Instance.LevelOpened += LevelOpened;
        }

        private void OnDisable()
        {
            MainMenuManager.Instance.LevelOpened -= LevelOpened;
        }

        //Checking is level was uòlcoked
        private void LevelOpened()
        {
            string gameObjectName = gameObject.name;
            string[]parts = gameObjectName.Split('_');
            _leveltext.text = parts[parts.Length - 1];
            currentLevel = int.Parse(_leveltext.text);
            isLevelUnlocked = GameManager.Instance.IsLevelUnlockedConnect(currentLevel);

            _image.color = isLevelUnlocked ? MainMenuManager.Instance.CurrentColor : _inactiveColor;
        }

        private void Clicked()
        {

            if (!isLevelUnlocked)
            {
                return;
            }
            GameManager.Instance.CurrentLevelConnect = currentLevel;
            GameManager.Instance.GoToGameplayConnect();


        }



    }
}
