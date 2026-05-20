using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float speed = 2f;
    public float driftAmount = 0.5f;
    public float spinSpeed = 30f;
    public float health = 30f;

    private Vector2 driftOffset;
    private float startX;

    void Start()
    {
        startX = transform.position.x;
        driftOffset = new Vector2(Random.Range(-1f, 1f), 0);
    }

    void Update()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);
        float drift = Mathf.Sin(Time.time + driftOffset.x) * driftAmount * Time.deltaTime;
        transform.Translate(Vector2.right * drift);
        transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0) Destroy(gameObject);
    }
}
