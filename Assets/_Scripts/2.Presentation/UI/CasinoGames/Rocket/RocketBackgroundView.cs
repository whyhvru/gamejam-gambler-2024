using UnityEngine;

namespace Module.Presentation.UI
{
    public sealed class RocketBackgroundView : MonoBehaviour
    {
        [SerializeField] private RectTransform _background;

        [SerializeField] private float _scrollSpeed = 100f;
        [SerializeField] private float _startPositionY = 1380f;
        [SerializeField] private float _endPositionY = -1104f;

        private bool _isRunning;
        private float _multiplier = 1f;

        private Vector2 _startPosition;
        private Vector2 _endPosition;

        private void Awake()
        {
            _startPosition = new Vector2(_background.anchoredPosition.x, _startPositionY);
            _endPosition = new Vector2(_background.anchoredPosition.x, _endPositionY);
        }

        private void Start() => ResetBackground();

        private void Update()
        {
            if (!_isRunning) return;

            float m = Mathf.Max(1f, _multiplier / 2f);
            _background.anchoredPosition -= Vector2.up * (_scrollSpeed * m * Time.deltaTime);

            if (_background.anchoredPosition.y <= _endPosition.y)
                ResetBackground();
        }

        public void SetMultiplier(float multiplier) => _multiplier = multiplier;

        public void StartScrolling()
        {
            _isRunning = true;
            ResetBackground();
        }

        public void StopScrolling() => _isRunning = false;

        private void ResetBackground() => _background.anchoredPosition = _startPosition;
    }
}