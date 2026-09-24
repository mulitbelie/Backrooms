using System.Collections;
using UnityEngine;

public class WardrobeController : MonoBehaviour, IInteractable
{
    const string 打开参数名 = "IsOpen";
    const string 打开状态名 = "Open";
    const string 关闭状态名 = "Close";
    const float 交互冷却秒数 = 0.5f;
    const float 无动画最小延迟秒数 = 0.2f;

    [Header("躲藏点空物体")]
    public Transform 躲藏点;

    [Header("出站设置")]
    [Tooltip("从 HideSpot 朝门外方向推的距离（避免出站穿模）")]
    public float 出站距离 = 0.5f;
    [Tooltip("从 HideSpot 向上偏移的高度（床底出站需要抬高，衣柜出站可用 0）")]
    public float 出站高度 = 0f;

    [Header("触发器")]
    public float 触发半径 = 2.5f;

    [Header("扩展")]
    [Tooltip("有开关门动画（衣柜/柜子）或无动画瞬时进出（床底/浴缸）")]
    public bool 有开关门动画 = true;

    [Header("缩放")]
    [Tooltip("躲藏时玩家缩放，1=不变，0.5=缩小一半")]
    public Vector3 躲藏时缩放 = new Vector3(1f, 1f, 1f);

    [Header("提示文本")]
    public string 躲藏提示 = "to hide";
    public string 离开提示 = "to leave";

    public bool 玩家已躲藏 { get; private set; }

    Animator 衣柜Animator;

    enum 状态 { 空闲, 打开中, 已躲藏, 离开中 }
    状态 当前状态 = 状态.空闲;
    float 冷却截止时间;

    Transform 玩家;
    CharacterController 角色控制器;
    基础移动控制 移动控制;
    鼠标视角控制 视角控制;
    触摸视角控制 触摸视角控制;
    固定滑动 滑动控制;
    bool 之前滑动状态;
    Vector3 之前缩放;
    SphereCollider 触发器;

    public bool 位置在有效范围内(Vector3 pos) =>
        躲藏点 != null && Vector3.Distance(pos, 躲藏点.position) <= 触发半径;

    void Reset()
    {
        if (有开关门动画)
            衣柜Animator = GetComponentInChildren<Animator>();
    }

    void Awake()
    {
        触发器 = gameObject.GetComponent<SphereCollider>();
        if (触发器 == null)
            触发器 = gameObject.AddComponent<SphereCollider>();
        触发器.isTrigger = true;
        触发器.radius = 触发半径;
    }

    void Start()
    {
        if (有开关门动画 && 衣柜Animator == null)
            衣柜Animator = GetComponentInChildren<Animator>();

        查找玩家组件();

        if (有开关门动画 && 衣柜Animator != null)
            衣柜Animator.SetBool(打开参数名, false);
    }

    void 查找玩家组件()
    {
        玩家 = null;
        var candidates = GameObject.FindGameObjectsWithTag("Player");

        foreach (var go in candidates)
        {
            if (go.GetComponentInChildren<CharacterController>(true) != null)
            {
                玩家 = go.transform;
                break;
            }
        }

        if (玩家 == null && candidates.Length > 0)
            玩家 = candidates[0].transform;

        if (玩家 == null) return;

        角色控制器 = 玩家.GetComponent<CharacterController>();
        if (角色控制器 == null) 角色控制器 = 玩家.GetComponentInChildren<CharacterController>(true);
        if (角色控制器 == null) 角色控制器 = 玩家.GetComponentInParent<CharacterController>(true);

        移动控制 = 玩家.GetComponent<基础移动控制>();
        if (移动控制 == null) 移动控制 = 玩家.GetComponentInChildren<基础移动控制>(true);
        if (移动控制 == null) 移动控制 = 玩家.GetComponentInParent<基础移动控制>(true);
        if (移动控制 == null) 移动控制 = FindObjectOfType<基础移动控制>();

        视角控制 = 玩家.GetComponentInChildren<鼠标视角控制>(true);
        if (视角控制 == null) 视角控制 = FindObjectOfType<鼠标视角控制>();

        触摸视角控制 = 玩家.GetComponentInChildren<触摸视角控制>(true);
        if (触摸视角控制 == null) 触摸视角控制 = FindObjectOfType<触摸视角控制>();

        滑动控制 = 玩家.GetComponentInChildren<固定滑动>(true);
        if (滑动控制 == null) 滑动控制 = FindObjectOfType<固定滑动>();
    }

    public string 交互提示
    {
        get
        {
            if (当前状态 == 状态.已躲藏 || 当前状态 == 状态.离开中)
                return $"{平台工具.按键提示} {离开提示}";
            if (当前状态 == 状态.空闲)
                return $"{平台工具.按键提示} {躲藏提示}";
            return "";
        }
    }

    public bool 不需要射线命中 => true;

    public void OnInteract()
    {
        查找玩家组件();
        if (玩家 == null) return;

        if (Time.time < 冷却截止时间) return;

        if (当前状态 == 状态.空闲)
            StartCoroutine(进入衣柜流程());
        else if (当前状态 == 状态.已躲藏)
            StartCoroutine(离开衣柜流程());
    }

    void 暂停玩家()
    {
        if (移动控制 != null) 移动控制.设置暂停(true);
        if (角色控制器 != null) 角色控制器.enabled = false;
    }

    void 恢复玩家()
    {
        if (角色控制器 != null) 角色控制器.enabled = true;
        if (移动控制 != null) 移动控制.设置暂停(false);
        if (视角控制 != null) 视角控制.恢复默认(玩家);
        if (触摸视角控制 != null) 触摸视角控制.恢复默认(玩家);
        if (滑动控制 != null) 滑动控制.是否滑动 = 之前滑动状态;
        if (玩家 != null) 玩家.localScale = 之前缩放;
    }

    IEnumerator 进入衣柜流程()
    {
        当前状态 = 状态.打开中;
        暂停玩家();

        if (有开关门动画)
        {
            衣柜Animator?.SetBool(打开参数名, true);
            yield return 等动画播完(打开状态名);
        }

        if (躲藏点 != null)
        {
            玩家.position = 躲藏点.position;
            玩家.rotation = 躲藏点.rotation;
        }

        if (视角控制 != null) 视角控制.收紧为衣柜(玩家);
        if (触摸视角控制 != null) 触摸视角控制.收紧为衣柜(玩家);
        if (滑动控制 != null) { 之前滑动状态 = 滑动控制.是否滑动; 滑动控制.是否滑动 = false; }

        之前缩放 = 玩家.localScale;
        玩家.localScale = 躲藏时缩放;

        if (有开关门动画)
        {
            衣柜Animator?.SetBool(打开参数名, false);
            yield return 等动画播完(关闭状态名);
        }

        玩家已躲藏 = true;
        当前状态 = 状态.已躲藏;
        冷却截止时间 = Time.time + 交互冷却秒数;
    }

    IEnumerator 离开衣柜流程()
    {
        当前状态 = 状态.离开中;
        暂停玩家();

        if (有开关门动画)
        {
            衣柜Animator?.SetBool(打开参数名, true);
            yield return 等动画播完(打开状态名);
        }
        else
        {
            yield return new WaitForSeconds(无动画最小延迟秒数);
        }

        if (躲藏点 != null)
        {
            Vector3 出站方向 = -躲藏点.forward;
            Vector3 出站位置 = 躲藏点.position + 出站方向 * 出站距离 + Vector3.up * 出站高度;
            Quaternion 出站旋转 = 躲藏点.rotation * Quaternion.Euler(0f, 180f, 0f);

            玩家.rotation = 出站旋转;
            玩家.position = 出站位置;
        }

        恢复玩家();

        if (有开关门动画)
        {
            衣柜Animator?.SetBool(打开参数名, false);
            yield return 等动画播完(关闭状态名);
        }

        玩家已躲藏 = false;
        当前状态 = 状态.空闲;
        冷却截止时间 = Time.time + 交互冷却秒数;
    }

    IEnumerator 等动画播完(string 状态名)
    {
        if (衣柜Animator == null) yield break;
        var wait = new WaitForEndOfFrame();

        yield return wait;

        while (true)
        {
            var info = 衣柜Animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName(状态名) && info.normalizedTime >= 1f)
                break;
            yield return wait;
        }
    }

    void OnDestroy()
    {
        恢复玩家();
        if (有开关门动画 && 衣柜Animator != null)
            衣柜Animator.SetBool(打开参数名, false);
    }
}