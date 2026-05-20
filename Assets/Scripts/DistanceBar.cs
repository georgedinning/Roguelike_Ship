using UnityEngine;

public class DistanceBar : MonoBehaviour
{
    public float percent;
    private float lastPercent;

    public RectTransform _blankBackBar;
    public RectTransform _travelledBar;
    public RectTransform _shipIcon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    { 

    }

    public void progressBar(float timeFactor, float timeSinceStageStart, float totalStageDuration)
    {
        percent = timeSinceStageStart / totalStageDuration;

        percent = Mathf.Min(Mathf.Max(0, percent), 1);
        updateVisual();
    }

    public void updateVisual()
    {
        _shipIcon.SetLocalPositionAndRotation(
            new Vector2(_shipIcon.rect.position.x + _shipIcon.rect.width / 2f,
                _blankBackBar.rect.position.y + percent * _blankBackBar.rect.height),
            Quaternion.identity);

        _travelledBar.sizeDelta = new Vector2(_travelledBar.rect.width, percent * _blankBackBar.rect.height);

        _travelledBar.SetLocalPositionAndRotation(
            new Vector2(_travelledBar.rect.x + _travelledBar.rect.width / 2f,
            -1.5f * _blankBackBar.rect.height + _blankBackBar.rect.height + _travelledBar.rect.height / 2f),
        Quaternion.identity);
    }
}
