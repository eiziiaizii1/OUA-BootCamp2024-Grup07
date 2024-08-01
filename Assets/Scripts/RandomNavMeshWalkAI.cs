using UnityEngine;
using UnityEngine.AI;

public class RandomNavMeshWalkAI : MonoBehaviour
{
    public float moveRadius = 10.0f;
    public float changeDestinationInterval = 5.0f;

    private NavMeshAgent agent;
    private Animator animator;
    private float changeDestinationTimer;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        ChangeDestination();
    }

    void Update()
    {
        changeDestinationTimer -= Time.deltaTime;
        if (changeDestinationTimer <= 0)
        {
            ChangeDestination();
        }

        animator.SetFloat("Speed", GetNormalizedSpeed(agent.velocity.magnitude));
        animator.SetFloat("MotionSpeed", 1);
    }

    void ChangeDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * moveRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, moveRadius, 1))
        {
            agent.SetDestination(hit.position);
        }
        changeDestinationTimer = changeDestinationInterval;
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
