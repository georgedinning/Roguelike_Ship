using UnityEngine;

public class ShipModule : MonoBehaviour
{
    public int powerCost = 1;
    public bool powered = true;

    public System.Action OnPowerChanged;

    public void SetPowered(bool on)
    {
        if (powered == on) return;
        powered = on;
        OnPowerChanged?.Invoke();
        OnPowerStateChanged();
    }

    protected virtual void OnPowerStateChanged() { }
}
