using UnityEngine;

public class GameManager : MonoBehaviour
{
    public DistanceBar _distanceBar;
    private float timeSinceStageStart;
    public float totalStageDuration;

    void Start()
    {
        timeSinceStageStart = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceStageStart += Time.deltaTime * TimeController.CurrentTimeScale;
        _distanceBar.progressBar(TimeController.CurrentTimeScale, timeSinceStageStart, totalStageDuration);
    }
}
