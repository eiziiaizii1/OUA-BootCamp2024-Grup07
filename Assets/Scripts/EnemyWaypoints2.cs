using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWaypoints2 : MonoBehaviour
{
    public Transform[] waypoints;
    public int currentWaypoint = 0;
    private NavMeshAgent navMeshAgent;
    public bool isReversing = false;
    public Transform player;
    public float chaseDistance = 4f; // The distance at which the enemy will start chasing the player
    public float loseDistance = 1.5f; // The distance between player and enemy for player to lose
    public float patrolSpeed = 2.25f; // Speed while patrolling
    public float chaseSpeed = 3f; // Speed while chasing

    private TurtleSkill invisiblityScript;

    private void Start()
    {
        invisiblityScript = player.GetComponent<TurtleSkill>();

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = patrolSpeed; // Set initial speed to patrol speed
        navMeshAgent.SetDestination(waypoints[0].position);
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);


        if (distanceToPlayer <= loseDistance)
        {
            // Player is too close to the enemy, player loses
            HandlePlayerLoss();
        }
            
        if (distanceToPlayer <= chaseDistance && !invisiblityScript.isInvisible)
        {
            // Player is within chase distance, chase the player
            navMeshAgent.speed = chaseSpeed;
            navMeshAgent.SetDestination(player.position);
            
        }
        else
        {
            navMeshAgent.speed = patrolSpeed;
            // Player is out of chase distance, continue patrolling
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
            {
                UpdateWaypoint();
            }
        }
    }

    private void UpdateWaypoint()
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == waypoints[currentWaypoint].transform)
        {
            UpdateWaypoint();
        }
    }

    private void HandlePlayerLoss()
    {
        Debug.Log("Player has been caught by the enemy!");
    }
}
