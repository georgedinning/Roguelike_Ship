using UnityEngine;
using UnityEngine.UI;

public abstract class Encounter : MonoBehaviour
{
    public float triggerPoint;
    public string warningMessage = "Unknown Encounter";
    public Texture2D _hudIconTexture;

    public Image _hudIconImage { get; private set; }

    public bool IsActive { get; private set; }

    public void AssignIconImage(Image image)
    {
        _hudIconImage = image;
    }

    public void Fire()
    {
        IsActive = true;
        SetIconColor(Color.red);
        OnTrigger();
    }

    public void MarkCompleted()
    {
        IsActive = false;
        SetIconColor(new Color(0.7f, 0.7f, 0.7f));
        enabled = false;
    }

    public void SetIconPosition(Vector2 position)
    {
        if (_hudIconImage == null) return;
        _hudIconImage.rectTransform.SetLocalPositionAndRotation(position, Quaternion.identity);
    }

    public abstract void OnTrigger();

    private void SetIconColor(Color color)
    {
        if (_hudIconImage == null) return;
        _hudIconImage.color = color;
    }
}
