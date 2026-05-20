using UnityEngine;

public class EncounterSpawner : MonoBehaviour
{
    public GameObject _asteroidFieldPrefab;

    public void GenerateStageEncounters()
    {
        SpawnAsteroidField(0.3f);
        SpawnAsteroidField(0.6f);
    }

    public void SpawnAsteroidField(float triggerPoint)
    {
        GameObject go = Instantiate(_asteroidFieldPrefab);
        AsteroidFieldEncounter encounter = go.GetComponent<AsteroidFieldEncounter>();
        encounter.triggerPoint = triggerPoint;

        GameManager.Instance._distanceBar.PositionEncounterMarker(encounter);
        GameManager.Instance.RegisterEncounter(encounter);
    }
}
