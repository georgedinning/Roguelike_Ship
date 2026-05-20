using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Collision")]
    public float invincibilityTime = 0.5f;
    private float invincibilityTimer;

    public bool IsInvincible => invincibilityTimer > 0;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (invincibilityTimer > 0)
            invincibilityTimer -= Time.deltaTime;
    }

    public void TakeDamage(int amount)
    {
        if (IsInvincible) return;
        currentHealth = Mathf.Max(0, currentHealth - amount);
        invincibilityTimer = invincibilityTime;
        Debug.Log($"Player took {amount} damage, HP: {currentHealth}");

        if (currentHealth <= 0)
            OnDeath();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hazard"))
        {
            Debug.Log($"Player hit by {collision.gameObject.name}");
            TakeDamage(10);
            Destroy(collision.gameObject);
        }
    }

    private void OnDeath()
    {
        Debug.Log("Player destroyed");
        gameObject.SetActive(false);
    }

}
