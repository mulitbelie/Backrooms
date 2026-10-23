using Unity.VisualScripting;
using UnityEngine;

public class ZombieState : MonoBehaviour
{
    [Header("--- 感知参数 ---")]
    public float detectionRadius = 18f;
    public float viewAngle = 90f;
    public LayerMask obstacleLayer;

    [Header("--- 攻击参数 ---")]
    public float attackRange = 2.5f;
    public float stopChaseDistance = 21f;

    [Header("--- 强制感知（被击中触发）---")]
    public float forcedAwarenessDuration = 5f;
    private float forcedAwarenessTimer;

    [Header("--- 运行时状态（只读）---")]
    public bool canSeePlayer;
    public bool playerInAttackRange;
    public float playerDistance;
    public Transform player;
    [Header("--- 隐藏状态 ---")]
    public PlayerHideState playerHide;

    void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    public void ForceAware(Transform attacker)
    {
        forcedAwarenessTimer = forcedAwarenessDuration;
        player = attacker;
    }

    void Update()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                canSeePlayer = false;
                playerInAttackRange = false;
                playerDistance = float.MaxValue;
                return;
            }
        }

        if (playerHide == null)
        {
            playerHide = player.GetComponent<PlayerHideState>();
        }
        // 计算玩家距离
        Vector3 toPlayer = player.position - transform.position;
        playerDistance = toPlayer.magnitude;
        // 计算玩家是否在攻击范围内
        playerInAttackRange = playerDistance <= attackRange;

        // 优先级 1：玩家躲藏中 → 僵尸失明
        if (playerHide != null && playerHide.isHiding)
        {
            canSeePlayer = false;
        }    
        // 优先级 2：强制感知（被击中）→ 无视一切
        else if (forcedAwarenessTimer > 0)
        {
            forcedAwarenessTimer -= Time.deltaTime;
            canSeePlayer = true;
        }
        // 优先级 3：正常感知（玩家在检测范围内且在视野内）
        else
        {
            canSeePlayer =
                playerDistance <= detectionRadius
                && Vector3.Angle(transform.forward, toPlayer.normalized) < viewAngle / 2f
                && HasLineOfSight();
        }
    }

    bool HasLineOfSight()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up;
        Vector3 dir = player.position + Vector3.up - origin;
        float distance = dir.magnitude;

        if (Physics.Raycast(origin, dir.normalized, out hit, distance, obstacleLayer))
        {
            return false;
        }
        return true;
    }
}