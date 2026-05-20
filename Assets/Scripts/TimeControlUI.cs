using UnityEngine;
using UnityEngine.UI;

public class TimeControlUI : MonoBehaviour
{
    [Header("Button References")]
    public Button _pauseButton;
    public Button _playButton;
    public Button _fastButton;

    [Header("Colors")]
    public Color activeColor = Color.white;
    public Color inactiveColor = new Color(0.6f, 0.6f, 0.6f, 1f);

    private void Start()
    {
        if (TimeController.Instance != null)
        {
            TimeController.Instance.OnTimeStateChanged += UpdateButtonVisuals;
            UpdateButtonVisuals(TimeController.Instance.CurrentState);
        }
        else
        {
            Debug.LogError("TimeControlUI: No TimeController found in scene.");
        }

        if (_pauseButton != null)
            _pauseButton.onClick.AddListener(() => TimeController.Instance?.Pause());
        if (_playButton != null)
            _playButton.onClick.AddListener(() => TimeController.Instance?.SetNormal());
        if (_fastButton != null)
            _fastButton.onClick.AddListener(() => TimeController.Instance?.SetFast());
    }

    private void OnDestroy()
    {
        if (TimeController.Instance != null)
            TimeController.Instance.OnTimeStateChanged -= UpdateButtonVisuals;
    }

    private void UpdateButtonVisuals(TimeController.TimeState state)
    {
        SetButtonColor(_pauseButton, state == TimeController.TimeState.Paused);
        SetButtonColor(_playButton, state == TimeController.TimeState.Normal);
        SetButtonColor(_fastButton, state == TimeController.TimeState.Fast);
    }

    private void SetButtonColor(Button button, bool active)
    {
        if (button == null) return;
        Image image = button.GetComponent<Image>();
        if (image != null)
            image.color = active ? activeColor : inactiveColor;
    }
}
