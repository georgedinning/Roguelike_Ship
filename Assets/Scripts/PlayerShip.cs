using UnityEngine;

public class PlayerShip : MonoBehaviour
{
    public static PlayerShip Instance { get; private set; }

    [System.Serializable]
    public class ModuleSlot
    {
        public GameObject _modulePrefab;
        [HideInInspector] public ShipModule spawnedModule;
    }

    [Header("Health")]
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Power")]
    public PowerBar powerBar;

    [Header("Module Grid")]
    public int gridColumns = 5;
    public int gridRows = 5;
    public Vector2 cellSpacing = new Vector2(1f, 1f);
    [Header("Module Prefabs")]
    public GameObject _gatlingGunPrefab;
    private ModuleSlot[] moduleSlots;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple PlayerShips found, destroying duplicate");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Allocate a fixed number of module slots. Call before Start() to define the grid size.
    /// </summary>
    public void AllocateSlots(int slotCount)
    {
        moduleSlots = new ModuleSlot[slotCount];
        for (int i = 0; i < slotCount; i++)
            moduleSlots[i] = new ModuleSlot();
    }

    /// <summary>
    /// Place a module prefab at a specific grid slot index.
    /// </summary>
    public void SetModule(int slotIndex, GameObject modulePrefab)
    {
        if (moduleSlots == null)
        {
            Debug.LogError("Cannot set module: slots not allocated. Call AllocateSlots first.");
            return;
        }
        if (slotIndex < 0 || slotIndex >= moduleSlots.Length)
        {
            Debug.LogError($"Slot index {slotIndex} out of range (slots: {moduleSlots.Length})");
            return;
        }
        moduleSlots[slotIndex]._modulePrefab = modulePrefab;
    }

    /// <summary>
    /// Spawn the module in the given slot and return its ShipModule component.
    /// Slot must have a prefab assigned. Position is computed from the grid layout.
    /// </summary>
    public ShipModule SpawnModuleAtSlot(int slotIndex)
    {
        if (moduleSlots == null || slotIndex < 0 || slotIndex >= moduleSlots.Length)
            return null;

        ModuleSlot slot = moduleSlots[slotIndex];
        if (slot._modulePrefab == null) return null;

        int row = slotIndex / gridColumns;
        int col = slotIndex % gridColumns;
        Vector3 localPos = new Vector3(
            col * cellSpacing.x - cellSpacing.x * (gridColumns - 1) / 2f,
            row * cellSpacing.y - cellSpacing.y * (gridRows - 1) / 2f,
            0f
        );

        GameObject go = Instantiate(slot._modulePrefab, transform);
        go.transform.localPosition = localPos;
        go.transform.localRotation = Quaternion.identity;
        slot.spawnedModule = go.GetComponent<ShipModule>();
        return slot.spawnedModule;
    }

    /// <summary>
    /// Force the CompositeCollider2D to rebuild from current child colliders.
    /// Call after adding or removing modules at runtime.
    /// </summary>
    public void RefreshHullCollider()
    {
        CompositeCollider2D composite = GetComponent<CompositeCollider2D>();
        if (composite != null)
        {
            composite.enabled = false;
            composite.enabled = true;
        }
    }

    void Start()
    {
        currentHealth = maxHealth;

        int totalSlots = gridRows * gridColumns;
        AllocateSlots(totalSlots);

        // Fill the middle row (row 2, 0-indexed) with gatling guns
        int middleRow = 2;
        for (int col = 0; col < gridColumns; col++)
        {
            int slotIndex = middleRow * gridColumns + col;
            SetModule(slotIndex, _gatlingGunPrefab);
        }

        // Spawn all modules and refresh the composite hull collider
        ShipModule[] spawnedModules = new ShipModule[moduleSlots.Length];
        for (int i = 0; i < moduleSlots.Length; i++)
            spawnedModules[i] = SpawnModuleAtSlot(i);

        RefreshHullCollider();

        if (powerBar != null)
            powerBar.RecalculateFromModules(spawnedModules);
    }

    void Update()
    {

        // Shift + Left Click → toggle the module under the cursor
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetMouseButtonDown(0))
            {
                int moduleLayer = LayerMask.GetMask("ShipModules");
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D hit = Physics2D.OverlapPoint(worldPoint, moduleLayer);
                if (hit != null)
                {
                    ShipModule module = hit.GetComponentInParent<ShipModule>();
                    if (module != null)
                        ToggleModulePower(module);
                }
            }
        }
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
        currentHealth = Mathf.Max(0, currentHealth - amount);
        Debug.Log($"PlayerShip took {amount} damage, HP: {currentHealth}");

        if (currentHealth <= 0)
            OnDeath();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hazard"))
        {
            Debug.Log($"PlayerShip hit by {collision.gameObject.name}");
            TakeDamage(10);
            Destroy(collision.gameObject);
        }
    }

    private void OnDeath()
    {
        Debug.Log("PlayerShip destroyed");
        gameObject.SetActive(false);
    }
}
