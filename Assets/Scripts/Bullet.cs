using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float _lifetime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _lifetime-= Time.deltaTime;
        if (_lifetime <= 0)
        {
            this.enabled = false;
            Destroy(transform.gameObject);
        }
    }
}
