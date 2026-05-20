using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float speed = 2f;
    public float driftAmount = 0.5f;
    public float spinSpeed = 30f;
    public float health = 30f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(Random.Range(-driftAmount, driftAmount), -speed);
        rb.angularVelocity = Random.Range(-spinSpeed, spinSpeed);
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0) Destroy(gameObject);
    }
}
