using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Playables;

public class CutsceneArea : MonoBehaviour
{
    [Header("Area Settings")]
    public List<CutscenePoint> cutscenePoints = new List<CutscenePoint>();

    [Header("Player Detection")]
    public LayerMask playerLayer = 12;
    public float checkInterval = 0.1f;

    private Transform player;
    private float lastCheckTime;

    [System.Serializable]
    public class CutscenePoint
    {
        public string name;
        public Transform triggerPoint;
        public float triggerRadius = 2f;
        public PlayableDirector cutsceneDirector;
        public bool hasTriggered = false;
        public bool playOnce = true;

        [Header("Conditions")]
        public bool requireLineOfSight = false;
        public LayerMask obstacleLayer = -1;

        [Header("Priority")]
        public int priority = 0; // Higher priority triggers first
    }

    void Start()
    {
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Sort by priority
        cutscenePoints.Sort((a, b) => b.priority.CompareTo(a.priority));
    }

    void Update()
    {
        if (player == null) return;

        // Check at intervals for performance
        if (Time.time - lastCheckTime >= checkInterval)
        {
            CheckCutsceneTriggers();
            lastCheckTime = Time.time;
        }
    }

    void CheckCutsceneTriggers()
    {
        foreach (var point in cutscenePoints)
        {
            if (point.hasTriggered && point.playOnce) continue;
            if (point.triggerPoint == null || point.cutsceneDirector == null) continue;

            float distance = Vector3.Distance(player.position, point.triggerPoint.position);

            if (distance <= point.triggerRadius)
            {
                // Check line of sight if required
                if (point.requireLineOfSight && !HasLineOfSight(point))
                    continue;

                // Trigger cutscene
                TriggerCutscene(point);
                break; // Only trigger one at a time
            }
        }
    }

    bool HasLineOfSight(CutscenePoint point)
    {
        Vector3 direction = (player.position - point.triggerPoint.position).normalized;
        float distance = Vector3.Distance(player.position, point.triggerPoint.position);

        return !Physics.Raycast(point.triggerPoint.position, direction, distance, point.obstacleLayer);
    }

    void TriggerCutscene(CutscenePoint point)
    {
        Debug.Log($"Triggering cutscene: {point.name}");

        if (point.playOnce)
            point.hasTriggered = true;

        point.cutsceneDirector.Play();
    }

    // Reset all triggers (useful for testing)
    public void ResetAllTriggers()
    {
        foreach (var point in cutscenePoints)
        {
            point.hasTriggered = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        foreach (var point in cutscenePoints)
        {
            if (point.triggerPoint == null) continue;

            Gizmos.color = point.hasTriggered ? Color.red : Color.cyan;
            Gizmos.DrawWireSphere(point.triggerPoint.position, point.triggerRadius);

            // Draw line of sight check
            if (point.requireLineOfSight && player != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(point.triggerPoint.position, player.position);
            }
        }
    }
}