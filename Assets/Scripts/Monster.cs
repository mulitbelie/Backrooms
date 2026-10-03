﻿using UnityEngine;

public class Monster : MonoBehaviour
{   
    [Header("怪物设置")]
    public float 停止距离 = 1.5f;
    [Range(0f, 180f)]
    public float 玩家静止观察角度 = 50f;

    [Header("丢失目标设置")]
    public float 丢失目标距离 = 10f;
    public float 视线检测高度偏移 = 1f;

    [Header("状态名")]
    public string idle状态名 = "Idle";
    public string 攻击状态名 = "Attack";

    private Transform 玩家;
    private bool beCaught触发过;
    private bool 上一帧在看;
    private bool 丢失目标中;
    private bool 玩家在触发器内;
    private 基础移动控制 玩家控制;
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator != null)
            animator.applyRootMotion = false;
    }

    void Update()
    {
        if (丢失目标中) return;

        if (玩家 == null) return;

        if (beCaught触发过)
        {
            if (攻击已结束())
            {
                beCaught触发过 = false;
                if (animator != null) animator.SetBool("IsAttacking", false);
            }
            return;
        }

        Vector3 targetPos = new Vector3(玩家.position.x, transform.position.y, 玩家.position.z);
        float 当前距离 = Vector3.Distance(transform.position, targetPos);

        if (!玩家在触发器内)
        {
            if (当前距离 > 丢失目标距离)
            {
                进入丢失目标();
            }
            return;
        }

        if (!正在播放Idle()) return;

        if (当前距离 <= 停止距离)
        {
            CatchPlayer();
            return;
        }

        Vector3 玩家到怪物 = (transform.position - 玩家.position).normalized;
        float dot = Vector3.Dot(玩家.forward, 玩家到怪物);
        float 阈值 = Mathf.Cos(玩家静止观察角度 * Mathf.Deg2Rad);
        bool 这一帧在看 = dot >= 阈值;

        if (上一帧在看 && !这一帧在看)
        {
            transform.position += (targetPos - transform.position) / 2;
        }

        上一帧在看 = 这一帧在看;
    }

    void LateUpdate()
    {
        if (玩家 == null || beCaught触发过 || 丢失目标中) return;

        if (!正在播放Idle()) return;

        Vector3 targetPos = new Vector3(玩家.position.x, transform.position.y, 玩家.position.z);
        transform.LookAt(targetPos);
    }

    private bool 正在播放Idle()
    {
        if (animator == null) return true;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(idle状态名) && !animator.IsInTransition(0);
    }

    private bool 攻击已结束()
    {
        if (animator == null) return true;
        if (animator.IsInTransition(0)) return false;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return !stateInfo.IsName(攻击状态名);
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
        玩家控制 = null;

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
        玩家控制 = other.GetComponent<基础移动控制>();

        if (丢失目标中)
        {
            丢失目标中 = false;
            上一帧在看 = false;
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
            上一帧在看 = false;
            if (animator != null)
            {
                animator.SetBool("IsAgonizing", false);
                animator.SetBool("IsStanding", true);
            }
        }
        else if (!视线可达 && 玩家在触发器内)
        {
            玩家在触发器内 = false;
            上一帧在看 = false;
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
}