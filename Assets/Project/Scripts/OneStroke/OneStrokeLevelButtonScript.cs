using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Connect.Core
{

    /// <summary>
    /// Logic for one Stroke Level Buttons
    /// </summary>
    public class OneStrokeLevelButtonScript : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private Color _inactiveColor;
        [SerializeField] private Image _image;

        private bool isLevelUnlocked;
        private int currentLevel;

        private void Awake()
        {
            if (_button != null)
                _button.onClick.AddListener(Clicked);
            else
                Debug.LogError("Button reference is missing on " + gameObject.name);
        }

        private void Start()
        {
            if (MainMenuManager.Instance != null)
                MainMenuManager.Instance.LevelOpened += LevelOpened;
            else
                Debug.LogWarning("MainMenuManager.Instance is null in Start for " + gameObject.name);

            LevelOpened();
        }

        private void OnDisable()
        {
            if (MainMenuManager.Instance != null)
                MainMenuManager.Instance.LevelOpened -= LevelOpened;
        }

        private void LevelOpened()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("GameManager.Instance is null in LevelButtonNumberLinks");
                return;
            }

            string gameObjectName = gameObject.name;
            string[] parts = gameObjectName.Split('_');
            if (parts.Length > 1 && int.TryParse(parts[parts.Length - 1], out int levelNum))
            {
                currentLevel = levelNum;
                _levelText.text = parts[parts.Length - 1];

                isLevelUnlocked = GameManager.Instance.IsLevelUnlockedOneStroke(currentLevel);

                if (_image != null)
                    _image.color = isLevelUnlocked && MainMenuManager.Instance != null
                        ? MainMenuManager.Instance.CurrentColor
                        : _inactiveColor;
            }
            else
            {
                Debug.LogError($"Invalid level name format: {gameObjectName}", gameObject);
                if (_levelText != null)
                    _levelText.text = "Error";
            }
        }

        private void Clicked()
        {
            if (!isLevelUnlocked || GameManager.Instance == null) return;

            GameManager.Instance.CurrentLevelOneStroke = currentLevel;
            GameManager.Instance.GoToGameplayOneStroke();
        }
    }

}
