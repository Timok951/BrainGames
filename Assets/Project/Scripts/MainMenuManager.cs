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
        [SerializeField] private GameObject _levelPanelConnect;
        [SerializeField] private GameObject _levelPanelColorsort;
        [SerializeField] private GameObject _levelPanelPipes;

        [SerializeField] private GameObject _choosegamescreen;



        private void Awake()
        {

            Instance = this;
            _titlePanel.SetActive(true);
            _levelPanelConnect.SetActive(false);
            _levelPanelPipes.SetActive(false);
            _choosegamescreen.SetActive(false);
        }

        public UnityAction LevelOpened;

        [SerializeField]
        public Color CurrentColor;

        [SerializeField]
        private TMP_Text _levelTitleText;

        [SerializeField]
        private Image _levelTitleImage;


        public void ClickedPlay()
        {
            _titlePanel.SetActive(false);
            _choosegamescreen.SetActive(true);
            _levelPanelColorsort.SetActive(false);
            _levelPanelPipes.SetActive(false);

        }

        public void ClickedBackToTitle()
        {
            _titlePanel.SetActive(true);
            _levelPanelConnect.SetActive(false);
            _choosegamescreen.SetActive(false);
        }

        public void ClickedBackToGamemodes()
        {

            _titlePanel.SetActive(false);
            _levelPanelConnect.SetActive(false);
            _levelPanelColorsort.SetActive(false);
            _levelPanelPipes.SetActive(false) ;
            _choosegamescreen.SetActive(true) ;
        }

        public void ClickedConnect()
        {
            _titlePanel.SetActive(false);
            _choosegamescreen.SetActive(false);

            _levelPanelConnect.SetActive(true);

            _levelTitleImage.color = CurrentColor;
            LevelOpened?.Invoke();
        }

        public void ClickedColorSort()
        {
            _titlePanel.SetActive(false);
            _choosegamescreen.SetActive(false);

            _levelPanelColorsort.SetActive(true);

            _levelTitleImage.color = CurrentColor;
            LevelOpened?.Invoke();
        }


        public void ClickedPipes()
        {
            _titlePanel.SetActive(false);
            _choosegamescreen.SetActive(false);
            _choosegamescreen.SetActive(false);

            _levelPanelPipes.SetActive(true);

            _levelTitleImage.color = CurrentColor;
            LevelOpened?.Invoke();
        }





    }
}