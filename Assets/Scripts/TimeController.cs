using UnityEngine;

public class TimeController : MonoBehaviour
{
    public enum TimeState
    {
        Paused,
        Normal,
        Fast
    }

    [Header("Time Scale Values")]
    public float pausedScale = 0f;
    public float normalScale = 1f;
    public float fastScale = 2f;

    [Header("Input")]
    public KeyCode pauseToggleKey = KeyCode.Space;

    public static TimeController Instance { get; private set; }

    public TimeState CurrentState { get; private set; } = TimeState.Normal;
    public bool IsPaused => CurrentState == TimeState.Paused;
    public static float CurrentTimeScale => Time.timeScale;

    public delegate void TimeStateChangedHandler(TimeState newState);
    public event TimeStateChangedHandler OnTimeStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple TimeControllers found, destroying duplicate");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        SetState(TimeState.Normal);
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseToggleKey))
        {
            TogglePause();
        }
    }

    public void Pause()
    {
        SetState(TimeState.Paused);
    }

    public void SetNormal()
    {
        SetState(TimeState.Normal);
    }

    public void SetFast()
    {
        SetState(TimeState.Fast);
    }

    public void TogglePause()
    {
        if (CurrentState == TimeState.Paused)
            SetState(TimeState.Normal);
        else
            SetState(TimeState.Paused);
    }

    private void SetState(TimeState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;

        switch (newState)
        {
            case TimeState.Paused:
                Time.timeScale = pausedScale;
                break;
            case TimeState.Normal:
                Time.timeScale = normalScale;
                break;
            case TimeState.Fast:
                Time.timeScale = fastScale;
                break;
        }

        OnTimeStateChanged?.Invoke(newState);
        Debug.Log($"Time state changed to: {newState} (timeScale={Time.timeScale})");
    }
}
