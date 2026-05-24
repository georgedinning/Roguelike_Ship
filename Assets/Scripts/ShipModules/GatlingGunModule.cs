using System;
using UnityEngine;

public class GatlingGunModule : ShipModule
{
    public GameObject _turret, _bulletSpawnPoint, _projectiles, _bulletPrefab;
    public GameObject[] _ammoSprites;
    public float rotationSpeed;

    public float fireRate;
    public int magazineSize = 10;
    public float reloadTime = 2f;
    private int currentAmmo;
    private bool isReloading;
    private float reloadTimer;
    private float cooldown;

    void Start()
    {
        currentAmmo = magazineSize;
        _projectiles = GameObject.Find("Projectiles");
    }

    protected override void OnPowerStateChanged()
    {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        Color target = powered ? Color.white : new Color(0.35f, 0.35f, 0.35f);
        foreach (SpriteRenderer sr in sprites)
            sr.color = target;
    }

    void Update()
    {
        // Suppress turret entirely while shift is held (player is selecting a module)
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            Debug.Log("GatlingGun: Shift held, suppressing turret");
            return;
        }

        // No aiming or firing when unpowered (reload timer also pauses)
        if (!powered)
        {
            Debug.Log("GatlingGun: Not powered, suppressing turret");
            return;
        }

        // Point at cursor (aiming works during reload and while out of ammo)
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - new Vector2( _turret.transform.position.x, _turret.transform.position.y) );

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        _turret.transform.rotation = Quaternion.RotateTowards(
            _turret.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Reload in progress — can't fire
        if (isReloading)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0)
            {
                currentAmmo = magazineSize;
                foreach (GameObject ammoSprite in _ammoSprites) ammoSprite.SetActive(true);
                isReloading = false;
                Debug.Log($"GatlingGun: Reload complete, {currentAmmo} rounds");
            }
            return;
        }

        if (cooldown > 0)
        {
            cooldown = Mathf.Max(0, cooldown - Time.deltaTime);
        }
        if (Input.GetMouseButton(0) && currentAmmo > 0)
        {
            if (cooldown == 0)
            {
                GameObject bullet = GameObject.Instantiate(_bulletPrefab,
                    _bulletSpawnPoint.transform.position, _turret.transform.rotation);
                bullet.transform.parent = _projectiles.transform;
                bullet.GetComponent<Rigidbody2D>().AddForce(bullet.transform.up * 750f);
                cooldown = fireRate;

                _ammoSprites[magazineSize - currentAmmo].SetActive(false);
                currentAmmo--;
                if (currentAmmo <= 0)
                {
                    isReloading = true;
                    reloadTimer = reloadTime;
                    Debug.Log($"GatlingGun: Magazine empty, reloading ({reloadTime}s)");
                }
            }
        }
    }
}
