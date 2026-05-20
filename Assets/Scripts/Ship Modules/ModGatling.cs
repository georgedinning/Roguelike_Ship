using UnityEngine;

public class ModGatling : ShipModule
{
    public GameObject _turret, _bulletSpawnPoint, _projectiles, _bulletPrefab;
    public float rotationSpeed;

    public float fireRate;
    private float cooldown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Point at cursor
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - new Vector2( _turret.transform.position.x, _turret.transform.position.y) );

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        _turret.transform.rotation = Quaternion.RotateTowards(
            _turret.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (cooldown > 0)
        {
            cooldown = Mathf.Max(0, cooldown - Time.deltaTime);
        }
        if (Input.GetMouseButton(0))
        {
            //Try fire
            if (cooldown == 0)
            {
                //Fire bullet
                GameObject bullet = GameObject.Instantiate(_bulletPrefab);
                bullet.transform.position = _bulletSpawnPoint.transform.position;
                bullet.transform.parent = _projectiles.transform;
                bullet.GetComponent<Rigidbody2D>().AddForce(_turret.transform.up * 1000f);
                cooldown = fireRate;
            }
        }
        

    }
}
