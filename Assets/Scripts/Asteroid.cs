using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float speed = 2f;
    public float driftAmount = 0.5f;
    public float spinSpeed = 30f;
    public float health = 20f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(Random.Range(-driftAmount, driftAmount), -speed);
        rb.angularVelocity = Random.Range(-spinSpeed, spinSpeed);
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"Asteroid took {amount} damage, HP: {health}");
        if (health <= 0) Destroy(gameObject);
    }
}
