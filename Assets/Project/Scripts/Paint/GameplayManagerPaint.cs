using Connect.Common;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Connect.Core
{
    public class GameplayManagerPaint : MonoBehaviour
    {
        public static GameplayManagerPaint Instance;

        [HideInInspector] public bool CanClick;
        private LocalizedString _levelLocalized;

        [Header("Level Data")]
        [SerializeField] private PaintLevel _level;
        [SerializeField] private BlockPaint _blockPrefab;
        [SerializeField] private PlayerPaint _player;

        [Header("UI")]
        [SerializeField] private GameObject _winText;
        [SerializeField] private GameObject _nextLevelButton;
        [SerializeField] private GameObject _restartButton;
        [SerializeField] private GameObject _backButton;
        [SerializeField] private TMP_Text _titleText;

        private BlockPaint[,] blocks;
        private Vector2 start, end;

        private bool hasGameFinished;
        private Tween playStartTween;
        private bool _isDailyChallengeMode;

        private void Awake()
        {
            Instance = this;
            hasGameFinished = false;
            CanClick = true;

            _isDailyChallengeMode = DailyChallengeManager.Instance != null;

            _winText.SetActive(false);
            _levelLocalized = new LocalizedString("Gameplay", "Level");
            _levelLocalized.Arguments = new object[] { GameManager.Instance.CurrenLevelPaint };
            _levelLocalized.StringChanged += (value) => _titleText.text = value;
            _levelLocalized.RefreshString();

            if (_isDailyChallengeMode)
            {
                _nextLevelButton?.SetActive(false);
                _restartButton?.SetActive(false);
                _backButton?.SetActive(false);
                _titleText?.gameObject.SetActive(false);
            }

            SpawnLevel();
        }

        private void SpawnLevel()
        {
            _level = GameManager.Instance.GetLevelPaint();

            if (blocks != null)
            {
                foreach (var b in blocks)
                {
                    if (b != null) Destroy(b.gameObject);
                }
            }

            blocks = new BlockPaint[_level.Row, _level.Col];

            Vector3 camPos = Camera.main.transform.position;
            camPos.x = _level.Col * 0.5f - 0.5f;
            camPos.y = _level.Row * 0.5f - 0.5f;
            Camera.main.transform.position = camPos;
            Camera.main.orthographicSize = Mathf.Max(_level.Row, _level.Col) + 2f;

            _player.Init(_level.Start, _level.Row, _level.Col);

            for (int row = 0; row < _level.Row; row++)
            {
                for (int col = 0; col < _level.Col; col++)
                {
                    blocks[row, col] = Instantiate(_blockPrefab, new Vector3(col, row, 0), Quaternion.identity, transform);
                    blocks[row, col].Init(_level.Data[row * _level.Col + col]);
                }
            }

            HighLightBlock(_player.Pos);

            _winText.SetActive(false);
        }


        private void Update()
        {
            _levelLocalized.RefreshString();

            if (hasGameFinished || !CanClick) return;

            if (Input.GetMouseButtonDown(0))
                start = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            else if (Input.GetMouseButton(0))
                end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            else if (Input.GetMouseButtonUp(0))
            {
                Vector2Int direction = GetDirection();
                Vector2Int offset = GetOffsetEndPos(direction);
                if (offset == Vector2Int.zero) return;

                StartCoroutine(_player.Move(offset, Mathf.Max(Mathf.Abs(offset.x), Mathf.Abs(offset.y))));
                CanClick = false;
            }
        }

        #region Helpers
        private Vector2Int GetDirection()
        {
            Vector2 delta = end - start;
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                return delta.x > 0 ? Vector2Int.right : Vector2Int.left;
            return delta.y > 0 ? Vector2Int.up : Vector2Int.down;
        }

        private Vector2Int GetOffsetEndPos(Vector2Int direction)
        {
            Vector2Int result = Vector2Int.zero;
            Vector2Int checkPos = _player.Pos;

            while (true)
            {
                Vector2Int nextPos = checkPos + direction;
                if (!IsValid(nextPos) || blocks[nextPos.y, nextPos.x].Blocked) break;

                checkPos = nextPos;
                result += direction;
            }

            return result;
        }

        private bool IsValid(Vector2Int pos)
        {
            return pos.x >= 0 && pos.y >= 0 && pos.x < _level.Col && pos.y < _level.Row;
        }


        public void HighLightBlock(Vector2Int pos)
        {
            blocks[pos.y, pos.x].Add();
        }

        public void CheckWin()
        {
            for (int i = 0; i < _level.Row; i++)
                for (int j = 0; j < _level.Col; j++)
                    if (!blocks[i, j].Filled) return;

            hasGameFinished = true;
            _winText.SetActive(true);

            if (!_isDailyChallengeMode)
                GameManager.Instance.UnlockLevelPaint();
            else
                DailyChallengeManager.Instance?.OnModeCompleted();
        }
        #endregion

        #region UI Buttons
        public void ClickedRestart(Button button)
        {
            AnimateAndSwitch(button, () => GameManager.Instance.GoToGameplayPaint());
        }

        public void ClickedNextLevel(Button button)
        {
            if (!hasGameFinished) return;
            AnimateAndSwitch(button, () => GameManager.Instance.GoToGameplayPaint());
        }

        public void ClickedBack(Button button)
        {
            AnimateAndSwitch(button, () => GameManager.Instance.GoToMainMenu());
        }

        private void Animate(GameObject target, System.Action onComplete, float duration = 1f)
        {
            if (playStartTween != null && playStartTween.IsActive()) playStartTween.Kill();

            playStartTween = target.transform
                .DOScale(1.1f, 0.1f)
                .SetEase(Ease.Linear)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() => onComplete?.Invoke());

            playStartTween.Play();
        }

        private void AnimateAndSwitch(Button button, System.Action switchAction)
        {
            if (button != null) Animate(button.gameObject, switchAction);
            else { Debug.LogError("Button is null"); switchAction?.Invoke(); }
        }
        #endregion
    }
}
