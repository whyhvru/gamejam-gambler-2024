using UnityEngine;

[RequireComponent(typeof(RocketGame))]
public class RocketBackground : MonoBehaviour
{
    private readonly float _scrollSpeed = 100f;
    private readonly float _startPositionY = 1380f;
    private readonly float _endPositionY = -1104f;

    [SerializeField] private RectTransform _background;

    private RocketGame _rocketGame;
    private bool _isGameRunning;
    private Vector2 _startPosition;
    private Vector2 _endPosition;

    private void Awake()
    {
        _rocketGame = GetComponent<RocketGame>();
        _startPosition = new Vector2(_background.anchoredPosition.x, _startPositionY);
        _endPosition = new Vector2(_background.anchoredPosition.x, _endPositionY);
    }

    private void Start() => ResetBackground();

    private void Update()
    {
        if (!_isGameRunning) return;
        ScrollBackground();
    }

    private void ScrollBackground()
    {
        float multiplier = Mathf.Max(1f, _rocketGame.CurrentMultiplier / 2f);
        _background.anchoredPosition -= Vector2.up * (_scrollSpeed * multiplier * Time.deltaTime);

        if (_background.anchoredPosition.y <= _endPosition.y)
        {
            ResetBackground();
        }
    }

    private void ResetBackground() => _background.anchoredPosition = _startPosition;
    public void StartScrolling() => _isGameRunning = true;
    public void StopScrolling() => _isGameRunning = false;
}
