using System.Collections.Generic;
using UnityEngine;

public class AsteroidFieldEncounter : Encounter
{
    public GameObject _asteroidPrefab;
    public int count = 30;
    public Vector2 spawnArea = new Vector2(0, 50);

    private void Awake()
    {
        warningMessage = "Asteroid Field Detected";
    }

    private List<GameObject> spawnedAsteroids;

    public override void OnTrigger()
    {
        GameObject player = GameObject.FindWithTag("Player");
        float playerX = player != null ? player.transform.position.x : 0f;

        spawnedAsteroids = new List<GameObject>(count);

        for (int i = 0; i < count; i++)
        {
            Vector2 pos = new Vector2(
                playerX + Random.Range(-25, 25),
                Random.Range(spawnArea.y-10, spawnArea.y+10)
            );
            GameObject asteroid = Instantiate(_asteroidPrefab, pos, Quaternion.identity);
            spawnedAsteroids.Add(asteroid);
        }
    }

    void Update()
    {
        if (spawnedAsteroids == null) return;
        spawnedAsteroids.RemoveAll(a => a == null);
        if (spawnedAsteroids.Count == 0)
            MarkCompleted();
    }
}
