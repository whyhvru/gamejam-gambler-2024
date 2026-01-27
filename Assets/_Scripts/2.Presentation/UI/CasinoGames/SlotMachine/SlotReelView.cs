using System.Collections;
using UnityEngine;

namespace Module.Presentation.UI
{
    public sealed class SlotReelView : MonoBehaviour
    {
        [SerializeField] private RectTransform _reel;

        [SerializeField] private float[] _slotPositions = { -574f, -446f, -318f, -190f, -62f, 66f, 194f, 322f, 446f };

        [Header("Spin Timing (seconds)")]
        [SerializeField, Min(0.05f)] private float _accelDuration = 250f;
        [SerializeField, Min(0.05f)] private float _decelDuration = 500f;

        [Header("Speed")]
        [SerializeField, Min(1f)] private float _maxSpeed = 1400f;

        [Header("Curves (0..1 -> 0..1)")]
        [SerializeField] private AnimationCurve _accelCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _decelCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        public Coroutine Spin(MonoBehaviour runner, float delay, float stopAfter, System.Action<int> onStopped)
            => runner.StartCoroutine(SpinCoroutine(delay, stopAfter, onStopped));

        private IEnumerator SpinCoroutine(float delay, float stopAfter, System.Action<int> onStopped)
        {
            if (delay > 0f)
                yield return new WaitForSeconds(delay);

            float total = Mathf.Max(0.1f, stopAfter);
            float accel = Mathf.Min(_accelDuration, total * 0.45f);
            float decel = Mathf.Min(_decelDuration, total * 0.45f);
            float steady = Mathf.Max(0f, total - accel - decel);

            // --- Accel ---
            for (float t = 0f; t < accel; t += Time.deltaTime)
            {
                float k = accel <= 0f ? 1f : Mathf.Clamp01(t / accel);
                float speed = _maxSpeed * _accelCurve.Evaluate(k);
                Move(speed);
                yield return null;
            }

            // --- Steady ---
            for (float t = 0f; t < steady; t += Time.deltaTime)
            {
                Move(_maxSpeed);
                yield return null;
            }

            // --- Decel ---
            for (float t = 0f; t < decel; t += Time.deltaTime)
            {
                float k = decel <= 0f ? 1f : Mathf.Clamp01(t / decel);
                float speed = _maxSpeed * _decelCurve.Evaluate(k);
                Move(speed);
                yield return null;
            }

            // Snap to nearest
            int index = FindClosestIndex(_reel.anchoredPosition.y);
            float snapY = _slotPositions[index];
            _reel.anchoredPosition = new Vector2(_reel.anchoredPosition.x, snapY);

            onStopped?.Invoke(index);
        }

        private void Move(float speed)
        {
            _reel.anchoredPosition -= new Vector2(0f, speed * Time.deltaTime);

            if (_reel.anchoredPosition.y <= -574f)
                _reel.anchoredPosition = new Vector2(_reel.anchoredPosition.x, 574f);
        }

        private int FindClosestIndex(float y)
        {
            int bestIndex = 0;
            float bestDist = Mathf.Abs(y - _slotPositions[0]);

            for (int i = 1; i < _slotPositions.Length; i++)
            {
                float dist = Mathf.Abs(y - _slotPositions[i]);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }
    }
}
