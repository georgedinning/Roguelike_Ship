using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 10f;
    public float damage = 10f;

    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            this.enabled = false;
            Destroy(transform.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hazard"))
        {
            Asteroid asteroid = other.GetComponent<Asteroid>();
            if (asteroid != null)
                asteroid.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
