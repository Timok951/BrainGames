using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Connect.Core
{
    public class Cell : MonoBehaviour
    {
        #region START_VARIABLES
        [HideInInspector] public Color Color;
        [HideInInspector] public Vector2Int Position;

        [SerializeField] public float _startScalelDelay = 0.04f;
        [SerializeField] public float _startScaleTime = 0.2f;
        [SerializeField] public float _startMoveAnimationTime = 0.32f;
        [SerializeField] public float _selectedMoveAnimationTime = 0.16f;
        [SerializeField] public float _moveAnimationTime = 0.32f;
        public Tween startAnimation;
        public Tween startMoveAnimation;
        public Tween selectedMoveAnimation;
        public Tween moveAnimation;

        [SerializeField] private SpriteRenderer _bgSprite;

        private const int FRONT = 1;
        private const int BACK = 0;

        public bool IsStartTweenPlaying => startAnimation != null && startAnimation.IsActive();
        public bool IsStarMovePlaying => startMoveAnimation != null && startMoveAnimation.IsActive();
        public bool hasSelectedMoveFinished => selectedMoveAnimation == null || !selectedMoveAnimation.IsActive();
        public bool hasMoveFinished => moveAnimation == null || !moveAnimation.IsActive();
        #endregion

        #region Init
        public void Init(Color color, int x, int y, float offsetX, float offsetY)
        {
            Color = color;
            _bgSprite.color = Color;
            Position = new Vector2Int(x, y);
            transform.localPosition = new Vector3(x - offsetX, y - offsetY, 0);
            transform.localScale = Vector3.zero;
            float delay = (x + y) * _startScalelDelay;
            startAnimation = transform.DOScale(0.4f, _startScaleTime);
            startAnimation.SetEase(Ease.OutExpo);
            startAnimation.SetDelay(0.5f + delay);
            startAnimation.Play();
            Debug.Log($"Sprite size: {_bgSprite.bounds.size}, Scale: {transform.localScale}");
        }

        public void AnimateStartPosition(float offsetX, float offsetY)
        {
            startMoveAnimation = transform.DOLocalMove(
                new Vector3(Position.x - offsetX, Position.y - offsetY, 0), _startMoveAnimationTime);
            startMoveAnimation.SetEase(Ease.InSine);
            startMoveAnimation.Play();
        }
        #endregion

        #region UPDATE_METHODS
        public void GameFinished()
        {
            transform.localScale = Vector3.one;
            float delay = (Position.x + Position.y) * _startScalelDelay;
            startAnimation = transform.DOScale(0.5f, _startScaleTime);
            startAnimation.SetLoops(2, LoopType.Yoyo);
            startAnimation.SetDelay(delay + 0.5f);
            startAnimation.Play();
        }

        public void SelectedMoveStart()
        {
            _bgSprite.sortingOrder = FRONT;
            transform.localScale = Vector3.one * 0.6f;
        }

        public void SelectedMoveEnd()
        {
            _bgSprite.sortingOrder = BACK;
            selectedMoveAnimation = transform.DOLocalMove(
                new Vector3(Position.x - GameplayManagerColorSort.Instance.offsetX, Position.y - GameplayManagerColorSort.Instance.offsetY, 0f), _selectedMoveAnimationTime);
            selectedMoveAnimation.onComplete = () =>
            {
                _bgSprite.sortingOrder = BACK;
                transform.localScale = Vector3.one * 0.4f;
            };
            selectedMoveAnimation.Play();
        }

        public void SelectedMove(Vector2 offset)
        {
            transform.localPosition = new Vector3(Position.x - GameplayManagerColorSort.Instance.offsetX, Position.y - GameplayManagerColorSort.Instance.offsetY, 0) + (Vector3)offset;
            float minY = -GameplayManagerColorSort.Instance.offsetY;
            float maxY = GameplayManagerColorSort.Rows - 1 - GameplayManagerColorSort.Instance.offsetY;
            float minX = -GameplayManagerColorSort.Instance.offsetX;
            float maxX = GameplayManagerColorSort.Cols - 1 - GameplayManagerColorSort.Instance.offsetX;

            Vector2 pos = transform.localPosition;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            transform.localPosition = pos;
        }

        public void MoveEnd()
        {
            _bgSprite.sortingOrder = FRONT;
            moveAnimation = transform.DOLocalMove(
                new Vector3(Position.x - GameplayManagerColorSort.Instance.offsetX, Position.y - GameplayManagerColorSort.Instance.offsetY, 0f), _moveAnimationTime);
            moveAnimation.onComplete = () =>
            {
                _bgSprite.sortingOrder = BACK;
            };
            moveAnimation.Play();
        }
        #endregion



    }
}

