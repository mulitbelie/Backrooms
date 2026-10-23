using UnityEngine;

public class ZombieIdleState : StateMachineBehaviour
{
    float timer;
    public float idleTime = 4f;
    ZombieState perception;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        perception = animator.GetComponent<ZombieState>();
        timer = 0f;
        animator.SetBool("IsPatrolling", false);
        animator.SetBool("IsChasing", false);
        animator.SetBool("IsAttacking", false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (perception.canSeePlayer)
        {
            animator.SetBool("IsPatrolling", false);
            animator.SetBool("IsChasing", true);
        }
        else
        {
            timer += Time.deltaTime;
            if (timer >= idleTime)
            {
                animator.SetBool("IsPatrolling", true);
            }
        }
    }
}