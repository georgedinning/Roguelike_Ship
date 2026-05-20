using UnityEngine;

public class TestAsteroidSpawner : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public int count = 30;
    public Vector2 spawnArea = new Vector2(5, 80);

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        float playerX = player != null ? player.transform.position.x : 0f;

        for (int i = 0; i < count; i++)
        {
            Vector2 pos = new Vector2(
                playerX + Random.Range(-10, 10),
                Random.Range(spawnArea.y * 0.9f, spawnArea.y)
            );
            Instantiate(asteroidPrefab, pos, Quaternion.identity);
        }
    }
}
