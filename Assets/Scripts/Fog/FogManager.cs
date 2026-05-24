using System.Collections.Generic;
using UnityEngine;

public class FogManager : MonoBehaviour
{
    public static FogManager Instance { get; private set; }

    [Header("Overlay Reference")]
    public SpriteRenderer _fogOverlay;

    [Header("Radar Visuals")]
    public Color ringColor = Color.green;
    [Range(0.1f, 5f)]
    public float ringPulseSpeed = 1f;
    [Range(0.01f, 0.5f)]
    public float ringWidth = 0.015f;
    [Range(0.01f, 0.3f)]
    public float spokeWidth = 0.01f;
    public int circleSegments = 64;

    [Header("Base Fog (no sensors)")]
    public float baseFogStart = 5f;
    public float baseFogEnd = 15f;
    public float baseRadarEnd = 25f;

    // Stacked radii from all powered sensor modules
    public float TotalFogStart { get; private set; }
    public float TotalFogEnd { get; private set; }
    public float TotalRadarEnd { get; private set; }

    private readonly List<SensorModule> activeSensors = new List<SensorModule>();
    private readonly List<FogAffected> registeredEntities = new List<FogAffected>();

    private LineRenderer ringLine;
    private LineRenderer[] spokes;
    private Gradient spokeGradient;
    private readonly GradientAlphaKey[] spokeAlphaKeys = new GradientAlphaKey[3];
    private Transform shipTransform;
    private Material overlayMaterial;
    private bool overlayReady;

    // Shader property IDs
    private static readonly int ShipPosId = Shader.PropertyToID("_ShipPos");
    private static readonly int FogStartId = Shader.PropertyToID("_FogStart");
    private static readonly int FogEndId = Shader.PropertyToID("_FogEnd");

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple FogManagers found, destroying duplicate");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Find the ship
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            shipTransform = player.transform;

        // Cache overlay material reference
        if (_fogOverlay != null)
        {
            overlayMaterial = _fogOverlay.material;
            overlayReady = overlayMaterial != null;
        }

        // Create the radar ring (green boundary)
        GameObject ringObj = new GameObject("RadarRing", typeof(LineRenderer));
        ringObj.transform.SetParent(shipTransform ?? transform);
        ringObj.transform.localPosition = Vector3.zero;
        ringObj.transform.localRotation = Quaternion.identity;

        ringLine = ringObj.GetComponent<LineRenderer>();
        ringLine.loop = true;
        ringLine.useWorldSpace = false;
        ringLine.positionCount = circleSegments;
        ringLine.startWidth = ringWidth;
        ringLine.endWidth = ringWidth;
        ringLine.material = new Material(Shader.Find("Sprites/Default"));
        ringLine.material.color = ringColor;
        ringLine.sortingLayerName = "FogDots";

        // Create the 8 radial spokes spanning fogEnd → radarEnd
        int spokeCount = 8;
        spokes = new LineRenderer[spokeCount];
        for (int i = 0; i < spokeCount; i++)
        {
            GameObject spokeObj = new GameObject($"RadarSpoke{i}", typeof(LineRenderer));
            spokeObj.transform.SetParent(shipTransform ?? transform);
            spokeObj.transform.localPosition = Vector3.zero;
            spokeObj.transform.localRotation = Quaternion.identity;

            LineRenderer spoke = spokeObj.GetComponent<LineRenderer>();
            spoke.positionCount = 2;
            spoke.useWorldSpace = false;
            spoke.startWidth = spokeWidth;
            spoke.endWidth = spokeWidth;
            spoke.material = ringLine.material; // share ring material so pulse syncs
            spoke.sortingLayerName = "FogDots";
            spokes[i] = spoke;
        }

        // Gradient for spoke alpha fading (stream — set each frame in Update)
        spokeGradient = new Gradient();

        // Initial radii (sensors haven't registered yet — base values only)
        TotalFogStart = baseFogStart;
        TotalFogEnd = baseFogEnd;
        TotalRadarEnd = baseRadarEnd;
    }

    private void Update()
    {
        // 1. Push radii to shader
        if (overlayReady)
        {
            overlayMaterial.SetVector(ShipPosId,
                shipTransform != null ? (Vector3)shipTransform.position : Vector3.zero);
            overlayMaterial.SetFloat(FogStartId, TotalFogStart);
            overlayMaterial.SetFloat(FogEndId, TotalFogEnd);
        }

        // 3. Update radar visuals (ring + spokes, all pulse together)
        if (ringLine != null && spokes != null && shipTransform != null)
        {
            // Hide radar when there's no radar buffer zone
            bool hasRadarSpace = TotalRadarEnd > TotalFogEnd + 0.001f;
            ringLine.enabled = hasRadarSpace;
            for (int i = 0; i < spokes.Length; i++)
                spokes[i].enabled = hasRadarSpace;

            if (!hasRadarSpace)
                return;

            float pulseAlpha = 0.3f + 0.7f * (0.5f + 0.5f * Mathf.Sin(Time.time * ringPulseSpeed * Mathf.PI * 2f));
            Color pulseColor = new Color(ringColor.r, ringColor.g, ringColor.b, pulseAlpha);
            ringLine.material.color = pulseColor;

            float ringRadius = Mathf.Max(TotalRadarEnd, 0.1f);
            Vector3[] points = new Vector3[circleSegments];
            for (int i = 0; i < circleSegments; i++)
            {
                float angle = (float)i / circleSegments * Mathf.PI * 2f;
                points[i] = new Vector3(Mathf.Cos(angle) * ringRadius, Mathf.Sin(angle) * ringRadius, 0f);
            }
            ringLine.SetPositions(points);
            ringLine.transform.position = shipTransform.position;
            ringLine.startWidth = ringWidth;
            ringLine.endWidth = ringWidth;

            // Update spokes: transparent at fogStart, opaque at fogEnd → radarEnd
            float fogStart = Mathf.Max(TotalFogStart, 0.1f);
            float fogEnd = Mathf.Max(TotalFogEnd, fogStart + 0.1f);
            float radarEnd = TotalRadarEnd;
            float fadeDuration = fogEnd - fogStart;
            float innerT = fadeDuration > 0.001f
                ? (fogEnd - fogStart) / (radarEnd - fogStart)
                : 0.5f;

            spokeAlphaKeys[0] = new GradientAlphaKey(0f, 0f);       // fogStart → transparent
            spokeAlphaKeys[1] = new GradientAlphaKey(1f, innerT);   // fogEnd → opaque
            spokeAlphaKeys[2] = new GradientAlphaKey(1f, 1f);       // radarEnd → opaque
            spokeGradient.alphaKeys = spokeAlphaKeys;

            Vector3 shipPos = shipTransform.position;
            for (int i = 0; i < spokes.Length; i++)
            {
                float angle = (float)i / spokes.Length * Mathf.PI * 2f;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);

                spokes[i].colorGradient = spokeGradient;
                spokes[i].positionCount = 3;
                spokes[i].SetPositions(new Vector3[]
                {
                    new Vector3(cos * fogStart, sin * fogStart, 0f),
                    new Vector3(cos * fogEnd, sin * fogEnd, 0f),
                    new Vector3(cos * radarEnd, sin * radarEnd, 0f)
                });
                spokes[i].transform.position = shipPos;
                spokes[i].startWidth = spokeWidth;
                spokes[i].endWidth = spokeWidth;
            }
        }

        // 4. Update entity dot visibility
        if (shipTransform != null)
        {
            Vector3 shipPos = shipTransform.position;
            for (int i = registeredEntities.Count - 1; i >= 0; i--)
            {
                FogAffected entity = registeredEntities[i];
                if (entity == null)
                {
                    registeredEntities.RemoveAt(i);
                    continue;
                }

                float dist = Vector3.Distance(shipPos, entity.transform.position);
                entity.UpdateDot(dist, TotalFogEnd, TotalRadarEnd);
            }
        }
    }

    public void RegisterSensor(SensorModule sensor)
    {
        if (!activeSensors.Contains(sensor))
            activeSensors.Add(sensor);
    }

    public void UnregisterSensor(SensorModule sensor)
    {
        activeSensors.Remove(sensor);
    }

    public void RegisterEntity(FogAffected entity)
    {
        if (!registeredEntities.Contains(entity))
            registeredEntities.Add(entity);
    }

    public void UnregisterEntity(FogAffected entity)
    {
        registeredEntities.Remove(entity);
    }

    public void RecalculateRadii()
    {
        float fogStart = baseFogStart;
        float fogEnd = baseFogEnd;
        float radarEnd = baseRadarEnd;
        for (int i = activeSensors.Count - 1; i >= 0; i--)
        {
            SensorModule s = activeSensors[i];
            if (s == null)
            {
                activeSensors.RemoveAt(i);
                continue;
            }
            if (s.powered)
            {
                fogStart += s.fogStartRadius;
                fogEnd += s.fogEndRadius;
                radarEnd += s.radarEndRadius;
            }
        }
        TotalFogStart = fogStart;
        TotalFogEnd = fogEnd;
        TotalRadarEnd = radarEnd;
    }
}
