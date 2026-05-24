using UnityEngine;

public class GatlingGunModule : ShipModule
{
    public GameObject _turret, _bulletSpawnPoint, _projectiles, _bulletPrefab;
    public float rotationSpeed;

    public float fireRate;
    private float cooldown;

    void Awake()
    {
        powerCost = 2;
    }

    void Update()
    {
        //Point at cursor (aiming works regardless of power)
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - new Vector2( _turret.transform.position.x, _turret.transform.position.y) );

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        _turret.transform.rotation = Quaternion.RotateTowards(
            _turret.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (!powered) return;

        if (cooldown > 0)
        {
            cooldown = Mathf.Max(0, cooldown - Time.deltaTime);
        }
        if (Input.GetMouseButton(0))
        {
            if (cooldown == 0)
            {
                GameObject bullet = GameObject.Instantiate(_bulletPrefab);
                bullet.transform.position = _bulletSpawnPoint.transform.position;
                bullet.transform.parent = _projectiles.transform;
                bullet.GetComponent<Rigidbody2D>().AddForce(_turret.transform.up * 750f);
                cooldown = fireRate;
            }
        }
    }
}
