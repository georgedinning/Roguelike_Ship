using UnityEngine;

public class SensorModule : ShipModule
{
    public float fogStartRadius = 10f;
    public float fogEndRadius = 30f;
    public float radarEndRadius = 50f;

    private void Start()
    {
        if (FogManager.Instance != null)
            FogManager.Instance.RegisterSensor(this);
    }

    private void OnDestroy()
    {
        if (FogManager.Instance != null)
            FogManager.Instance.UnregisterSensor(this);
    }

    protected override void OnPowerStateChanged()
    {
        // Visual feedback — sensors could glow when powered
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        Color target = powered ? Color.white : new Color(0.35f, 0.35f, 0.35f);
        foreach (SpriteRenderer sr in sprites)
            sr.color = target;
    }
}
