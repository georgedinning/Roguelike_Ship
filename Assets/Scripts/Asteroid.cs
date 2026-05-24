using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float speed = 1.5f;
    public float driftAmount = 0.7f;
    public float spinSpeed = 30f;
    public float health = 20f;

    private Rigidbody2D rb;
    private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(Random.Range(-driftAmount, driftAmount), -speed);
        rb.angularVelocity = Random.Range(-spinSpeed, spinSpeed);
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (mainCamera != null)
        {
            float bottomEdge = mainCamera.transform.position.y - mainCamera.orthographicSize - 2f;
            if (transform.position.y < bottomEdge)
                Destroy(gameObject);
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"Asteroid took {amount} damage, HP: {health}");
        if (health <= 0) Destroy(gameObject);
    }
}
