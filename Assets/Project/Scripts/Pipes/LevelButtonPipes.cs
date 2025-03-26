using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Connect.Core
{
    public class LevelButtonPipes : MonoBehaviour
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

        private void LevelOpened()
        {
            string gameObjectName = gameObject.name;
            string[] parts = gameObjectName.Split('_');
            if (parts.Length > 1 && int.TryParse(parts[parts.Length - 1], out int levelNum))
            {
                _leveltext.text = parts[parts.Length - 1];
                currentLevel = levelNum;
                isLevelUnlocked = GameManager.Instance.IsLevelUnlockedPipes(currentLevel);
                _image.color = isLevelUnlocked ? MainMenuManager.Instance.CurrentColor : _inactiveColor;
            }
            else
            {
                Debug.LogError($"Invalid level name format: {gameObjectName}", gameObject);
                _leveltext.text = "Error";
            }
        }

        private void Clicked()
        {
            if (!isLevelUnlocked) return;
            GameManager.Instance.CurrentLevel = currentLevel;
            GameManager.Instance.GoToGameplayPipes();
        }
    }
}