using UnityEngine;

public class TestAsteroidSpawner : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public int count = 3;
    public Vector2 spawnArea = new Vector2(5, 3);

    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 pos = new Vector2(
                Random.Range(-spawnArea.x, spawnArea.x),
                Random.Range(spawnArea.y * 0.5f, spawnArea.y)
            );
            Instantiate(asteroidPrefab, pos, Quaternion.identity);
        }
    }
}
