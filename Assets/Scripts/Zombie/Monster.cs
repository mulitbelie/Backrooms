﻿using UnityEngine;

public class Monster : MonoBehaviour
{   
    [Header("怪物设置")]
    public float 停止距离 = 1.5f;
    [Range(0f, 180f)]
    public float 玩家静止观察角度 = 50f;

    [Header("追赶设置")]
    public float 追赶范围 = 12f;
    public float 追赶速度 = 3f;
    public float 偷窥速度 = 1.5f;

    [Header("丢失目标设置")]
    public float 丢失目标距离 = 20f;
    public float 视线检测高度偏移 = 1f;

    private Transform 玩家;
    private bool beCaught触发过;
    private bool 丢失目标中;
    private bool 玩家在触发器内;
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator != null)
            animator.applyRootMotion = false;
    }

    private bool 追赶中;
    private bool 已发现玩家;

    void Update()
    {
        if (丢失目标中) return;

        if (玩家 == null) return;

        if (beCaught触发过)
        {
            float 攻击中距离 = Vector3.Distance(transform.position, 玩家.position);

            if (攻击中距离 > 停止距离 || 攻击已结束())
            {
                beCaught触发过 = false;
                if (animator != null) animator.SetBool("IsAttacking", false);
            }
            else
            {
                return;
            }
        }

        Vector3 targetPos = new Vector3(玩家.position.x, transform.position.y, 玩家.position.z);
        float 当前距离 = Vector3.Distance(transform.position, targetPos);

        if (当前距离 > 丢失目标距离)
        {
            退出追赶();
            已发现玩家 = false;
            进入丢失目标();
            return;
        }

        if (!玩家在触发器内)
        {
            退出追赶();
            return;
        }

        if (!正在播放Idle() && !(追赶中 && 正在播放Run())) return;

        if (当前距离 <= 停止距离)
        {
            退出追赶();
            CatchPlayer();
            return;
        }

        if (已发现玩家 && 当前距离 <= 追赶范围)
        {
            if (!追赶中) 进入追赶();

            Vector3 移动方向 = (targetPos - transform.position).normalized;
            transform.position += 移动方向 * 追赶速度 * Time.deltaTime;
            return;
        }

        if (追赶中)
        {
            退出追赶();
            return;
        }

        Vector3 玩家到怪物 = (transform.position - 玩家.position).normalized;
        float dot = Vector3.Dot(玩家.forward, 玩家到怪物);
        float 阈值 = Mathf.Cos(玩家静止观察角度 * Mathf.Deg2Rad);
        bool 这一帧在看 = dot >= 阈值;

        已发现玩家 = true;

        if (!这一帧在看)
        {
            Vector3 移动方向 = (targetPos - transform.position).normalized;
            transform.position += 移动方向 * 偷窥速度 * Time.deltaTime;
        }
    }

    void LateUpdate()
    {
        if (玩家 == null || beCaught触发过 || 丢失目标中) return;

        if (!追赶中 && !正在播放Idle()) return;

        Vector3 targetPos = new Vector3(玩家.position.x, transform.position.y, 玩家.position.z);
        transform.LookAt(targetPos);
    }

    private bool 正在播放Idle()
    {
        if (animator == null) return true;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Idle") && !animator.IsInTransition(0);
    }

    private bool 正在播放Run()
    {
        if (animator == null) return true;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Run") && !animator.IsInTransition(0);
    }

    private bool 攻击已结束()
    {
        if (animator == null) return true;
        if (animator.IsInTransition(0)) return false;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return !stateInfo.IsName("Attack");
    }

    private bool 视线被遮挡()
    {
        if (玩家 == null) return true;

        Vector3 起点 = transform.position + Vector3.up * 视线检测高度偏移;
        Vector3 方向 = 玩家.position - 起点;
        float 距离 = 方向.magnitude;

        if (Physics.Raycast(起点, 方向.normalized, out RaycastHit hit, 距离))
        {
            if (!hit.collider.CompareTag("Player"))
                return true;
        }

        return false;
    }

    private void 进入丢失目标()
    {
        丢失目标中 = true;
        玩家 = null;

        if (animator != null)
        {
            animator.SetBool("IsStanding", false);
            animator.SetBool("IsAgonizing", true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        玩家 = other.transform;

        if (丢失目标中)
        {
            丢失目标中 = false;
            if (animator != null) animator.SetBool("IsAgonizing", false);
        }

        if (!视线被遮挡())
        {
            玩家在触发器内 = true;
            if (animator != null) animator.SetBool("IsStanding", true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (玩家 == null) 玩家 = other.transform;

        bool 视线可达 = !视线被遮挡();

        if (视线可达 && !玩家在触发器内)
        {
            玩家在触发器内 = true;
            if (animator != null)
            {
                animator.SetBool("IsAgonizing", false);
                animator.SetBool("IsStanding", true);
            }
        }
        else if (!视线可达 && 玩家在触发器内)
        {
            玩家在触发器内 = false;
            if (animator != null) animator.SetBool("IsStanding", false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        玩家在触发器内 = false;
    }

    private void CatchPlayer()
    {
        beCaught触发过 = true;

        if (animator != null)
            animator.SetBool("IsAttacking", true);
    }

    private void 进入追赶()
    {
        追赶中 = true;
        if (animator != null)
        {
            animator.SetBool("IsRunning", true);
        }
    }

    private void 退出追赶()
    {
        追赶中 = false;
        if (animator != null)
        {
            animator.SetBool("IsRunning", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 追赶范围);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 停止距离);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 丢失目标距离);
    }
}