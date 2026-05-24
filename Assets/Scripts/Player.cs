using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [Header("Health")]
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Collision")]
    public float invincibilityTime = 0.5f;
    private float invincibilityTimer;

    [Header("Power")]
    public PowerBar powerBar;
    public ShipModule[] modules;

    public bool IsInvincible => invincibilityTimer > 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple Players found, destroying duplicate");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        currentHealth = maxHealth;
        if (powerBar != null)
            powerBar.RecalculateFromModules(modules);
    }

    void Update()
    {
        if (invincibilityTimer > 0)
            invincibilityTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Alpha1) && modules.Length > 0)
            ToggleModulePower(modules[0]);
    }

    public void ToggleModulePower(ShipModule module)
    {
        if (module == null) return;

        if (module.powered)
        {
            powerBar.DecreaseUsage(module.powerCost);
            module.SetPowered(false);
        }
        else if (powerBar.HasAvailablePower(module.powerCost))
        {
            powerBar.IncreaseUsage(module.powerCost);
            module.SetPowered(true);
        }
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
