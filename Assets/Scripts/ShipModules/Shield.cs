using UnityEngine;

/// <summary>
/// Placed on the ShieldBubble child GameObject.
/// Forwards trigger collisions to the parent ShieldModule so the hitbox
/// can live on a different layer (e.g. "Default") without interfering with
/// shift+click module selection (which uses the "ShipModules" layer).
/// </summary>
public class Shield : MonoBehaviour
{
    private ShieldModule parent;

    private void Awake()
    {
        parent = GetComponentInParent<ShieldModule>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (parent != null)
            parent.OnHitboxTriggerEnter(other);
    }
}
