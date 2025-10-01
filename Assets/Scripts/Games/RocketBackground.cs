using UnityEngine;

public class RocketBackground : MonoBehaviour
{
    [SerializeField] private RectTransform _background;
    [SerializeField] private RocketGame _rocketGame;
    [SerializeField] private float _scrollSpeed = 100f;
    private float _currentMultiplier;
    private bool _isGameRunning = false;
    private float _startPositionY = 1380f;
    private float _endPositionY = -1104f;

    private void Start()
    {
        _background.anchoredPosition = new Vector2(_background.anchoredPosition.x, _startPositionY);
    }

    private void Update()
    {
        _currentMultiplier = Mathf.Max(1f, _rocketGame.CurrentMultiplier / 2);

        if (_isGameRunning)
        {
            ScrollBackground();
        }
    }

    private void ScrollBackground()
    {
        _background.anchoredPosition -= new Vector2(0, _scrollSpeed * _currentMultiplier * Time.deltaTime);

        if (_background.anchoredPosition.y <= _endPositionY)
        {
            _background.anchoredPosition = new Vector2(_background.anchoredPosition.x, _startPositionY);
        }
    }

    public void StartScrolling()
    {
        _isGameRunning = true;
    }

    public void StopScrolling()
    {
        _isGameRunning = false;
    }
}
