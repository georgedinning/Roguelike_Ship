using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public DistanceBar _distanceBar;
    public EncounterSpawner _encounterSpawner;
    public List<Encounter> encounters = new List<Encounter>();

    private float timeSinceStageStart;
    public float totalStageDuration;

    private int nextEncounterIndex;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple GameManagers found, destroying duplicate");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        timeSinceStageStart = 0;
        encounters.Sort((a, b) => a.triggerPoint.CompareTo(b.triggerPoint));
        nextEncounterIndex = 0;

        if (_encounterSpawner != null)
            _encounterSpawner.GenerateStageEncounters();
    }

    void Update()
    {
        bool blocked = _distanceBar.waitForEncounterCompletion && HasActiveEncounter();

        if (!blocked)
        {
            timeSinceStageStart += Time.deltaTime * TimeController.CurrentTimeScale;
            _distanceBar.progressBar(TimeController.CurrentTimeScale, timeSinceStageStart, totalStageDuration);
        }

        CheckEncounters();
    }

    public bool HasNextEncounter => nextEncounterIndex < encounters.Count;

    public float NextEncounterTriggerPoint
    {
        get
        {
            if (nextEncounterIndex < encounters.Count)
                return encounters[nextEncounterIndex].triggerPoint;
            return 1f;
        }
    }

    public string NextEncounterWarningMessage
    {
        get
        {
            if (nextEncounterIndex < encounters.Count)
                return encounters[nextEncounterIndex].warningMessage;
            return "";
        }
    }

    private bool HasActiveEncounter()
    {
        foreach (var e in encounters)
        {
            if (e != null && e.IsActive)
                return true;
        }
        return false;
    }

    public void RegisterEncounter(Encounter encounter)
    {
        encounters.Add(encounter);
        encounters.Sort((a, b) => a.triggerPoint.CompareTo(b.triggerPoint));
    }

    private void CheckEncounters()
    {
        while (nextEncounterIndex < encounters.Count &&
               encounters[nextEncounterIndex].triggerPoint <= _distanceBar.percent)
        {
            encounters[nextEncounterIndex].Fire();

            // Clamp progress to exact trigger point when waiting for encounter completion
            if (_distanceBar.waitForEncounterCompletion)
            {
                timeSinceStageStart = encounters[nextEncounterIndex].triggerPoint * totalStageDuration;
                _distanceBar.progressBar(TimeController.CurrentTimeScale, timeSinceStageStart, totalStageDuration);
            }

            nextEncounterIndex++;
        }
    }
}
