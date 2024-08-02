using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics.Internal;
using UnityEngine;
using UnityEngine.AI;

public class AlienAnimation : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;


    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

    }

    void Update()
    {
        animator.SetFloat("Speed", GetNormalizedSpeed(agent.velocity.magnitude));
        animator.SetFloat("MotionSpeed", 1);
    }

    float GetNormalizedSpeed(float speed)
    {
        if (speed < 0.1f)
        {
            return 0f; // Idle
        }
        else if (speed <= 2f)
        {
            return 2f; // Walking
        }
        else
        {
            return 2f; // Running
        }
    }


    private void OnFootstep(AnimationEvent animationEvent)
    {

    }

    private void OnLand(AnimationEvent animationEvent)
    {

    }

}

