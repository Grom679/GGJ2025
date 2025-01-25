using System;
using UnityEngine;
using UnityEngine.UI;

public class ReactionMinigame : MonoBehaviour
{
    public Action OnSuccess { get; set; }
    public Action OnFailure { get; set; }
    public Action OnClose { get; set; }

    [SerializeField]
    private float _speed = 5f; // Speed of the moving object
    [SerializeField]
    private float _movementRange = 10f; // How far the object moves left and right
    [SerializeField]
    private float _zoneHeight = 15f;

    private int score = 0;

    public Image reactionZoneImage; // Visual for reaction zone
    public Image rangeImage; // Visual for range of movement

    private bool _isInReactionZone = false;
    private float _initialPosX;
    private RectTransform _rectTransform;
    private float _reactionZoneStart;
    private float _reactionZoneEnd;
    private bool _isOpen;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _initialPosX = _rectTransform.anchoredPosition.x;
    }

    private void Update()
    { 
        if(!_isOpen) return;
        MoveObject();
        CheckReactionZone();
        CheckPlayerInput();
    }

    public void ActivateGame(float zoneSize)
    {
        if(zoneSize >= _movementRange)
        {
            _reactionZoneStart = rangeImage.rectTransform.anchoredPosition.x;
            _reactionZoneEnd = (_movementRange + _rectTransform.sizeDelta.x) / 2;
        }
        else
        {
            float endingPoint = ((_movementRange + _rectTransform.sizeDelta.x) / 2) - zoneSize;
            _reactionZoneStart = UnityEngine.Random.Range(rangeImage.rectTransform.anchoredPosition.x, endingPoint);
            _reactionZoneEnd = _reactionZoneStart + zoneSize;
        }

        UpdateZoneVisualization();
        _isOpen = true;
    }

    public void Close()
    {
        _isOpen = false;
        ResetGame();
    }

    private void CloseUI()
    {
        Close();
        OnClose?.Invoke();
    }

    // Move the object left and right
    private void MoveObject()
    {
        float newX = _initialPosX + Mathf.PingPong(Time.time * _speed, _movementRange);
        _rectTransform.anchoredPosition = new Vector3(newX, _rectTransform.anchoredPosition.y);
    }

    // Check if the object is in the reaction zone
    private void CheckReactionZone()
    {
        float currentX = _rectTransform.anchoredPosition.x;
        if (currentX >= _reactionZoneStart && currentX <= _reactionZoneEnd)
        {
            _isInReactionZone = true;
        }
        else
        {
            _isInReactionZone = false;
        }
    }

    // Check if the player pressed space while in the reaction zone
    private void CheckPlayerInput()
    {
        if (_isInReactionZone && Input.GetKeyDown(KeyCode.Space))
        {
            score++;
            OnSuccess?.Invoke();
        }
        else if (!_isInReactionZone && Input.GetKeyDown(KeyCode.Space))
        {
            OnFailure?.Invoke();
        }
    }

    // Reset the game after each attempt
    private void ResetGame()
    {
        // Reset the position of the object to the starting point
        _rectTransform.anchoredPosition = new Vector3(_initialPosX, _rectTransform.anchoredPosition.y);
    }

    // Update the visuals for the range and reaction zone
    private void UpdateZoneVisualization()
    {
        // Position and size the range image to cover the whole movement area
        rangeImage.rectTransform.sizeDelta = new Vector2(_movementRange + _rectTransform.sizeDelta.x, _zoneHeight); // Adjust height and width as needed

        // Position and size the reaction zone image
        float reactionZoneWidth = _reactionZoneEnd - _reactionZoneStart; // Adjust size based on the zone
        reactionZoneImage.rectTransform.sizeDelta = new Vector2(reactionZoneWidth, _zoneHeight); // Make it a bar
        reactionZoneImage.rectTransform.anchoredPosition = new Vector2(_reactionZoneStart, 0); // Position it at the right spot
    }
}
