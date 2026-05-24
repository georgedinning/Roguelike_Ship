using UnityEngine;

public class ShipModule : MonoBehaviour
{
    public int powerCost = 1;
    public bool powered = false;

    public System.Action OnPowerChanged;

    private void Awake()
    {
        // Set visual state to match the powered field immediately on spawn
        OnPowerStateChanged();
    }

    public void SetPowered(bool on)
    {
        if (powered == on) return;
        powered = on;
        OnPowerChanged?.Invoke();
        OnPowerStateChanged();
    }

    protected virtual void OnPowerStateChanged() { }
}
