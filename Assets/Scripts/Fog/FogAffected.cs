using UnityEngine;

public class FogAffected : MonoBehaviour
{
    [Header("Dot Visual")]
    public Color dotColor = Color.white;
    [Range(0.1f, 2f)]
    public float dotSize = 0.5f;

    private SpriteRenderer dotSprite;
    private static Sprite dotTexture;

    private void Start()
    {
        // Generate a small circle texture once
        if (dotTexture == null)
            dotTexture = GenerateDotTexture();

        // Create child dot renderer
        GameObject dotObj = new GameObject("FogDot", typeof(SpriteRenderer));
        dotObj.transform.SetParent(transform);
        dotObj.transform.localPosition = Vector3.zero;
        dotObj.transform.localRotation = Quaternion.identity;
        dotObj.transform.localScale = Vector3.one * dotSize;

        dotSprite = dotObj.GetComponent<SpriteRenderer>();
        dotSprite.sprite = dotTexture;
        dotSprite.color = dotColor;
        dotSprite.sortingLayerName = "FogDots";
        dotSprite.enabled = false;

        // Register with FogManager
        if (FogManager.Instance != null)
            FogManager.Instance.RegisterEntity(this);
    }

    private void OnDestroy()
    {
        if (FogManager.Instance != null)
            FogManager.Instance.UnregisterEntity(this);
    }

    /// <summary>
    /// Called each frame by FogManager. Controls dot visibility based on distance from ship.
    /// Dot visible only in the band fogEnd < dist ≤ radarEnd.
    /// </summary>
    public void UpdateDot(float distanceFromShip, float fogEnd, float radarEnd)
    {
        if (dotSprite == null) return;

        bool visible = distanceFromShip > fogEnd && distanceFromShip <= radarEnd;
        dotSprite.enabled = visible;
    }

    private static Sprite GenerateDotTexture()
    {
        int size = 16;
        Texture2D tex = new Texture2D(size, size);
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 1f;

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
        tex.filterMode = FilterMode.Bilinear;

        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}
