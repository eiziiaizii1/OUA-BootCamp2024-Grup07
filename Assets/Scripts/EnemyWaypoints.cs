using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWaypoints : MonoBehaviour
{

    public Transform[] waypoints;
    public int currentWaypoint = 0;
    private NavMeshAgent navMeshAgent;
    public bool isReversing = false;
    
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.SetDestination(waypoints[0].position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == waypoints[currentWaypoint].transform)
        {
            if (isReversing)
            {
                currentWaypoint--;
                if (currentWaypoint < 0)
                {
                    currentWaypoint = 1;
                    isReversing = false;
                }
            }
            else
            {
                currentWaypoint++;
                if (currentWaypoint >= waypoints.Length)
                {
                    currentWaypoint = waypoints.Length - 2;
                    isReversing = true;
                }
            }

            navMeshAgent.SetDestination(waypoints[currentWaypoint].position);
        }
    }
}
