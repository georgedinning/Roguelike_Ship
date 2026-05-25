using UnityEngine;

public class ShieldModule : ShipModule
{
    [Header("Shield Settings")]
    public float shieldRadius = 3f;       // max radius in world units
    public float shieldSpeed = 10f;       // expansion speed (units/sec)
    public float rechargeSpeed = 3f;      // seconds to recharge after a hit
    public float collapseSpeed = 20f;     // shrink speed after absorbing a hit
    public float shieldDamage = 50f;      // damage dealt to hazard on collision
    public GameObject _bubble;            // assigned child with SpriteRenderer + CircleCollider2D

    private enum ShieldState { Idle, Expanding, Deployed, Collapsing, Recharging }
    private ShieldState state = ShieldState.Idle;

    private float currentRadius;
    private float rechargeTimer;
    private bool hasDeployedHit;

    private CircleCollider2D shieldCollider;
    private SpriteRenderer _bubbleRenderer;

    private void Awake()
    {
        powerCost = 2;

        SetupCollider();
        SetupVisual();

        // Sync structural tint + state to match the powered field
        // (ShipModule.Awake doesn't fire because we override it)
        OnPowerStateChanged();
    }

    private void SetupCollider()
    {
        if (_bubble == null)
        {
            Debug.LogError("ShieldModule: _bubble not assigned on " + name);
            return;
        }

        shieldCollider = _bubble.GetComponent<CircleCollider2D>();
        if (shieldCollider == null)
        {
            Debug.LogError("ShieldModule: _bubble has no CircleCollider2D on " + name);
            return;
        }

        shieldCollider.isTrigger = true;
        // Collider radius is left at the prefab-assigned value (matching the
        // sprite at scale 1.0).  The transform scale in UpdateVisual handles
        // both the visual and the collider together.
        shieldCollider.enabled = false;
    }

    private void SetupVisual()
    {
        if (_bubble == null)
        {
            Debug.LogError("ShieldModule: _bubble not assigned on " + name);
            return;
        }

        _bubbleRenderer = _bubble.GetComponent<SpriteRenderer>();
        _bubble.SetActive(false);
        _bubble.transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        switch (state)
        {
            case ShieldState.Expanding:
                currentRadius += shieldSpeed * Time.deltaTime;
                if (currentRadius >= shieldRadius)
                {
                    currentRadius = shieldRadius;
                    TransitionTo(ShieldState.Deployed);
                }
                UpdateVisual();
                break;

            case ShieldState.Deployed:
                break;

            case ShieldState.Collapsing:
                currentRadius -= collapseSpeed * Time.deltaTime;
                if (currentRadius <= 0f)
                {
                    currentRadius = 0f;
                    shieldCollider.enabled = false;
                    _bubble.SetActive(false);
                    UpdateVisual();
                    TransitionTo(ShieldState.Recharging);
                }
                else
                {
                    UpdateVisual();
                }
                break;

            case ShieldState.Recharging:
                rechargeTimer -= Time.deltaTime;
                if (rechargeTimer <= 0f)
                {
                    hasDeployedHit = false;
                    currentRadius = 0f;
                    shieldCollider.enabled = true;
                    _bubble.SetActive(true);
                    TransitionTo(ShieldState.Expanding);
                }
                break;

            case ShieldState.Idle:
                break;
        }
    }

    public void OnHitboxTriggerEnter(Collider2D other)
    {
        // Shield can block while expanding or fully deployed (one hit per cycle)
        if ((state != ShieldState.Expanding && state != ShieldState.Deployed) || hasDeployedHit)
            return;

        if (!other.CompareTag("Hazard"))
            return;

        hasDeployedHit = true;

        if (other != null)
            other.enabled = false;

        // Apply damage so the hazard's own script handles destruction
        Asteroid asteroid = other.GetComponentInParent<Asteroid>();
        if (asteroid != null)
            asteroid.TakeDamage(shieldDamage);
        else
            Destroy(other.gameObject);

        TransitionTo(ShieldState.Collapsing);
    }

    private void UpdateVisual()
    {
        if (_bubble != null)
        {
            float baseWorldWidth = _bubbleRenderer != null && _bubbleRenderer.sprite != null
                ? _bubbleRenderer.sprite.bounds.size.x
                : 1f;

            float targetDiameter = currentRadius * 2f;
            float scale = baseWorldWidth > 0.0001f ? targetDiameter / baseWorldWidth : 0f;
            _bubble.transform.localScale = new Vector3(scale, scale, 1f);
        }

    }

    private void TransitionTo(ShieldState newState)
    {
        state = newState;

        switch (newState)
        {
            case ShieldState.Expanding:
                rechargeTimer = 0f;
                break;

            case ShieldState.Deployed:
                hasDeployedHit = false;
                break;

            case ShieldState.Collapsing:
                break;

            case ShieldState.Recharging:
                rechargeTimer = rechargeSpeed;
                break;

            case ShieldState.Idle:
                currentRadius = 0f;
                shieldCollider.enabled = false;
                _bubble.SetActive(false);
                UpdateVisual();
                break;
        }
    }

    protected override void OnPowerStateChanged()
    {
        if (powered)
        {
            _bubble.SetActive(true);
            shieldCollider.enabled = true;
            TransitionTo(ShieldState.Expanding);
        }
        else
        {
            TransitionTo(ShieldState.Idle);
        }

        // Tint the module's structural sprites (same pattern as SensorModule)
        SpriteRenderer[] structuralSprites = GetComponentsInChildren<SpriteRenderer>();
        Color target = powered ? Color.white : new Color(0.35f, 0.35f, 0.35f);
        foreach (SpriteRenderer sr in structuralSprites)
        {
            if (sr != _bubbleRenderer)
                sr.color = target;
        }
    }
}
