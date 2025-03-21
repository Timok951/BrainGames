using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

using UnityEngine.UI;


namespace Connect.Core{
    public class MainMenuManager : MonoBehaviour
    {
        public static MainMenuManager Instance;

        [SerializeField] private GameObject _titlePanel;
        [SerializeField] private GameObject _levelPanel;

        private void Awake()
        {

            Instance = this;
            _titlePanel.SetActive(true);
            _levelPanel.SetActive(false);
        }

        public UnityAction LevelOpened;

        [SerializeField]
        public Color CurremtColor;

        [SerializeField]
        private TMP_Text _levelTitleText;

        [SerializeField]
        private Image _levelTitleImage;


        public void ClickedPlay()
        {
            _titlePanel.SetActive(false);
            _levelPanel.SetActive(true);
            _levelTitleImage.color = CurremtColor;
            LevelOpened?.Invoke();
        }

        public void ClickedBackToTitle()
        {
            _titlePanel.SetActive(true);
            _levelPanel.SetActive(false);
        }





    }
}