using UnityEngine;
using UnityEngine.AI;

public class ZombieAttackState : StateMachineBehaviour
{
    ZombieState perception;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        perception = animator.GetComponent<ZombieState>();
        NavMeshAgent agent = animator.GetComponent<NavMeshAgent>();
        animator.SetBool("IsChasing", false);
        animator.SetBool("IsPatrolling", false);
        if (agent != null)
        {
            agent.ResetPath();
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (perception.player == null) return;

        bool canAttack = perception.canSeePlayer && perception.playerInAttackRange;

        if (canAttack)
        {
            animator.SetBool("IsAttacking", true);
            LookAtPlayer(animator);
        }
        else
        {
            animator.SetBool("IsAttacking", false);
        }
    }

    void LookAtPlayer(Animator animator)
    {
        Vector3 direction = perception.player.position - animator.transform.position;
        direction.y = 0;
        if (direction.sqrMagnitude < 0.001f) return;
        Quaternion targetRot = Quaternion.LookRotation(direction);
        animator.transform.rotation = Quaternion.Lerp(animator.transform.rotation, targetRot, Time.deltaTime * 5f);
    }
}