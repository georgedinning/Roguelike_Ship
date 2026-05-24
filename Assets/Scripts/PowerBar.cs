using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerBar : MonoBehaviour
{
    public int capacity = 6;
    public int used;

    public Image _segmentPrefab;
    public RectTransform _container;

    public float _spacing = 2f;

    public Sprite _activeSprite;
    public Sprite _inactiveSprite;

    private List<Image> segments = new List<Image>();

    void Start()
    {
        Rebuild();
    }

    public void SetCapacity(int newCapacity)
    {
        capacity = Mathf.Max(1, newCapacity);
        used = Mathf.Min(used, capacity);
        Rebuild();
    }

    public void SetUsage(int value)
    {
        used = Mathf.Clamp(value, 0, capacity);
        UpdateDisplay();
    }

    public void IncreaseUsage(int amount)
    {
        SetUsage(used + amount);
    }

    public void DecreaseUsage(int amount)
    {
        SetUsage(used - amount);
    }

    public bool HasAvailablePower(int amount)
    {
        return used + amount <= capacity;
    }

    public void RecalculateFromModules(ShipModule[] modules)
    {
        int total = 0;
        foreach (ShipModule m in modules)
        {
            if (m != null && m.powered)
                total += m.powerCost;
        }
        SetUsage(total);
    }

    public void Refresh()
    {
        Rebuild();
    }

    private void Rebuild()
    {
        foreach (Image seg in segments)
        {
            if (seg != null)
                Destroy(seg.gameObject);
        }
        segments.Clear();

        if (_segmentPrefab == null || _container == null)
        {
            Debug.LogWarning("PowerBar: _segmentPrefab or _container is not assigned.", this);
            return;
        }

        float containerHeight = _container.rect.height;
        float containerWidth = _container.rect.width;

        // Try filling the full width first — each segment gets equal horizontal space
        float availableWidth = containerWidth - (capacity - 1) * _spacing;
        float actualWidth = availableWidth / capacity;
        float actualHeight = actualWidth * 3f / 2f;

        // If height exceeds container, cap at container height and derive width
        if (actualHeight > containerHeight)
        {
            actualHeight = containerHeight;
            actualWidth = actualHeight * 2f / 3f;

            // Re-check width: scale down if segments still overflow
            float totalWidthNeeded = capacity * actualWidth + (capacity - 1) * _spacing;
            if (totalWidthNeeded > containerWidth)
            {
                float scale = containerWidth / totalWidthNeeded;
                actualWidth *= scale;
                actualHeight *= scale;
            }
        }

        float leftEdge = -containerWidth / 2f;

        for (int i = 0; i < capacity; i++)
        {
            Image seg = Instantiate(_segmentPrefab, _container);
            seg.rectTransform.sizeDelta = new Vector2(actualWidth, actualHeight);
            seg.rectTransform.anchoredPosition = new Vector2(
                leftEdge + actualWidth / 2f + i * (actualWidth + _spacing),
                0
            );
            segments.Add(seg);
        }

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        for (int i = 0; i < segments.Count; i++)
        {
            if (segments[i] != null)
                segments[i].sprite = i < used ? _activeSprite : _inactiveSprite;
        }
    }
}
