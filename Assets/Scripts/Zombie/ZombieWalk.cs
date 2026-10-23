using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieWalk : MonoBehaviour
{
    //声明一个用来接收【动画组件】的变量
    
    private Animator animator;
    private NavMeshAgent navAgent;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    // void Update()
    // {
    //     //判断是否在移动
    //     if(navAgent.velocity.magnitude > 0.1f)
    //     {
    //         animator.SetBool("IsWalking", true);
    //     }
    //     else
    //     {
    //         animator.SetBool("IsWalking", false);
    //     }
    // }
     //绘制Gizmos可视化颜色范围
    private void OnDrawGizmos()
    {
        //红色 攻击范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2.5f);//Attacking// StopAttacking
        //蓝色 检测玩家追击范围
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 12f);//Detectiong(Start Chasing)
        //绿色 停止追击范围
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 15f);//Stop Chasing
    }

}
