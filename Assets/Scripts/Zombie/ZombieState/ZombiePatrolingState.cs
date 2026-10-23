using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiePatrolingState : StateMachineBehaviour
{
    float timer;
    public float patrollingTime = 10f;
    NavMeshAgent agent;
    ZombieState perception;
    public float patrollingSpeed = 2f;

    List<Transform> waypointsList = new List<Transform>();

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        perception = animator.GetComponent<ZombieState>();
        agent = animator.GetComponent<NavMeshAgent>();

        agent.speed = patrollingSpeed;
        timer = 0;

        animator.SetBool("IsChasing", false);
        animator.SetBool("IsAttacking", false);

        GameObject waypointCluster = GameObject.FindGameObjectWithTag("Waypoints");
        foreach (Transform t in waypointCluster.transform)
        {
            waypointsList.Add(t);
        }

        Vector3 nextPosition = waypointsList[Random.Range(0, waypointsList.Count)].position;
        agent.SetDestination(nextPosition);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (perception.canSeePlayer)
        {
            animator.SetBool("IsPatrolling", false);
            animator.SetBool("IsChasing", true);
            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.SetDestination(waypointsList[Random.Range(0, waypointsList.Count)].position);
        }

        timer += Time.deltaTime;
        if (timer >= patrollingTime)
        {
            animator.SetBool("IsPatrolling", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent != null)
        {
            agent.ResetPath();
        }
    }
}