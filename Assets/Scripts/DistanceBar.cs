using UnityEngine;
using UnityEngine.UI;

public class DistanceBar : MonoBehaviour
{
    public float percent;
    private float lastPercent;

    public RectTransform _blankBackBar;
    public RectTransform _travelledBar;
    public RectTransform _shipIcon;

    public Sprite _encounterMarkerSprite;

    public bool waitForEncounterCompletion = false;

    void Start()
    {

    }

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

    public void PositionEncounterMarker(Encounter encounter)
    {
        if (encounter == null) return;
        Transform container = _blankBackBar.transform.parent;
        Sprite fallbackSprite = _encounterMarkerSprite != null ? _encounterMarkerSprite : GenerateMarkerSprite();

        GameObject go = new GameObject("EncounterMarker", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(container, false);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(18, 18);

        Image image = go.GetComponent<Image>();
        image.color = Color.white;

        if (encounter._hudIconTexture != null)
        {
            Rect textureRect = new Rect(0, 0, encounter._hudIconTexture.width, encounter._hudIconTexture.height);
            image.sprite = Sprite.Create(encounter._hudIconTexture, textureRect, new Vector2(0.5f, 0.5f));
        }
        else
        {
            image.sprite = fallbackSprite;
        }

        float centerX = _blankBackBar.rect.center.x;
        float y = _blankBackBar.rect.position.y + encounter.triggerPoint * _blankBackBar.rect.height;
        rect.SetLocalPositionAndRotation(
            new Vector2(centerX, y),
            Quaternion.identity);

        encounter.AssignIconImage(image);
    }

    private Sprite GenerateMarkerSprite()
    {
        int size = 18;
        Texture2D tex = new Texture2D(size, size);
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 0.5f;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float dx = x - center.x;
                float dy = y - center.y;
                bool inside = dx * dx + dy * dy <= radius * radius;
                tex.SetPixel(x, y, inside ? Color.white : Color.clear);
            }
        }
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}
