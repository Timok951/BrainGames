using System.Collections;
using UnityEngine;

namespace Connect.Core
{
    /// <summary>
    /// Player logic for Paint mode
    /// </summary>
    public class PlayerPaint : MonoBehaviour
    {
        [HideInInspector] public Vector2Int Pos;

        [SerializeField] private float _moveTime = 0.2f;
        [SerializeField] private AnimationCurve _speedCurve;

        private int maxRow;
        private int maxCol;

        public void Init(Vector2Int start, int rowCount, int colCount)
        {
            Pos = start;
            transform.position = new Vector3(Pos.x, Pos.y, 0f);
            maxRow = rowCount;
            maxCol = colCount;
        }

        public IEnumerator Move(Vector2Int offset, int distance)
        {
            Vector2Int step = new Vector2Int(
                offset.x != 0 ? (offset.x > 0 ? 1 : -1) : 0,
                offset.y != 0 ? (offset.y > 0 ? 1 : -1) : 0
            );

            for (int i = 0; i < distance; i++)
            {
                Pos += step;
                Vector3 startPos = transform.position;
                Vector3 targetPos = new Vector3(Pos.x, Pos.y, 0f);
                float t = 0f;

                while (t < 1f)
                {
                    t += Time.deltaTime / _moveTime;
                    transform.position = Vector3.Lerp(startPos, targetPos, _speedCurve.Evaluate(t));
                    yield return null;
                }

                transform.position = targetPos;
                GameplayManagerPaint.Instance.HighLightBlock(Pos);
            }

            GameplayManagerPaint.Instance.CanClick = true;
            GameplayManagerPaint.Instance.CheckWin();
        }
    }
}
