using UnityEngine;
using UnityEngine.AI;

public class ZombieChaseState : StateMachineBehaviour
{
    NavMeshAgent agent;
    ZombieState perception;
    public float chaseSpeed = 6f;
    public float loseTargetDelay = 0.8f;
    private float loseTargetTimer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        perception = animator.GetComponent<ZombieState>();
        agent = animator.GetComponent<NavMeshAgent>();
        agent.speed = chaseSpeed;
        animator.SetBool("IsPatrolling", false);
        animator.SetBool("IsAttacking", false);
        loseTargetTimer = 0;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (perception.player == null) return;

        agent.SetDestination(perception.player.position);
        animator.transform.LookAt(perception.player);

        if (perception.playerInAttackRange && perception.canSeePlayer)
        {
            animator.SetBool("IsChasing", false);
            animator.SetBool("IsAttacking", true);
            return;
        }

        if (!perception.canSeePlayer || perception.playerDistance > perception.stopChaseDistance)
        {
            loseTargetTimer += Time.deltaTime;
            if (loseTargetTimer >= loseTargetDelay)
            {
                animator.SetBool("IsChasing", false);
                return;
            }
        }
        else
        {
            loseTargetTimer = 0;
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