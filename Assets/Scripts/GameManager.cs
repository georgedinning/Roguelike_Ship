using UnityEngine;

public class GameManager : MonoBehaviour
{
    public DistanceBar _distanceBar;
    public float timeFactor;
    private float timeSinceStageStart;
    public float totalStageDuration;

    void Start()
    {
        timeSinceStageStart = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceStageStart += Time.deltaTime * timeFactor;
        _distanceBar.progressBar(timeFactor, timeSinceStageStart, totalStageDuration);
    }
}
