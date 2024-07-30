using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private WayPointPath waypointPath;
    [SerializeField] private float speed;

    private int targetWaypointIndex;

    private Transform prevWaypoint;
    private Transform targetWaypoint;

    private float timeToWayPoint;
    private float elapsedTime;

    private void Start()
    {
        TargetWaypoint();
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        float elapsedPercentage = elapsedTime / timeToWayPoint;
        transform.position = Vector3.Lerp(prevWaypoint.position, targetWaypoint.position, elapsedPercentage);

        if (elapsedPercentage >= 1f)
        {
            TargetWaypoint();
        }
    }


    private void TargetWaypoint()
    {
        prevWaypoint = waypointPath.GetWaypoint(targetWaypointIndex);
        targetWaypointIndex = waypointPath.GetNextWaypointIndex(targetWaypointIndex);
        targetWaypoint = waypointPath.GetWaypoint(targetWaypointIndex);

        elapsedTime = 0f;

        float distanceToWaypoint = Vector3.Distance(prevWaypoint.position, targetWaypoint.position);
        timeToWayPoint = distanceToWaypoint / speed;
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.parent = null;
            other.transform.localScale = Vector3.one;
        }
    }
}
