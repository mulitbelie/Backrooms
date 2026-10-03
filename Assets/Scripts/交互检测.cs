using UnityEngine;
using System;
using System.Collections.Generic;

public class 交互检测 : MonoBehaviour
{
    [Header("射线设置")]
    public float 检测距离 = 3f;
    public LayerMask 检测层级 = ~0;

    [Header("背包（自动查找）")]
    public Inventory 背包;

    //只有真的变了才更新，避免重复触发
    public event Action<IInteractable> On选中变化;

    // 当前被射线/Trigger 选中的可交互物体引用，null 表示没有选中任何东西
    private IInteractable 当前可交互物体;
    // 缓存玩家主相机，射线检测需要用它做视口坐标转世界射线
    private Camera 主相机;
    // 缓存玩家的移动控制，用于执行交互前判断"是否被僵尸抓住"
    private 基础移动控制 移动控制;

    // 对外只读属性，让其他脚本（如 UI 提示）可以安全地读取当前选中的物体
    public IInteractable 当前物体 => 当前可交互物体;

    void Start()
    {
        // 从场景中查找主相机（Tag 为 MainCamera 的 Camera）
        主相机 = Camera.main;

        // 64 是 Unity LayerMask 在 Inspector 选 "Nothing" 时的默认值（二进制 1000000），
        // 表示没有选择任何层。这里兜底改为检测所有层，避免射线永远射不中
        if (检测层级.value == 64)
        {
            检测层级 = ~0;
        }

        // 背包如果没在 Inspector 里手动拖，就自动查找：
        // 优先从玩家自身或父级找（支持多玩家各自隔离），找不到再全场景扫描
        if (背包 == null)
        {
            背包 = GetComponentInParent<Inventory>();
            if (背包 == null)
                背包 = FindObjectOfType<Inventory>();
        }

        // 同样自动查找移动控制组件，后面执行交互前要用它判断玩家状态
        移动控制 = GetComponentInParent<基础移动控制>();
        if (移动控制 == null) 移动控制 = FindObjectOfType<基础移动控制>();
    }

    void Update()
    {
        刷新可交互物体();
    }

    void 刷新可交互物体()
    {
        IInteractable 新选中 = null;
        if (主相机 != null)
        {
            Ray ray = 主相机.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            RaycastHit[] 命中列表 = Physics.RaycastAll(ray, 检测距离, 检测层级, QueryTriggerInteraction.Collide);

            var 触发物体Set = new HashSet<GameObject>();
            var 触发到Interactable = new Dictionary<GameObject, IInteractable>();

            foreach (var hit in 命中列表)
            {
                var interactable = hit.collider.GetComponentInParent<IInteractable>();
                if (interactable == null) continue;

                var mb = interactable as MonoBehaviour;
                if (mb == null || !mb.gameObject.activeSelf) continue;

                var item = mb as InventoryItem;
                if (item != null && !LockableInteractable.IsAccessible(item)) continue;

                if (触发物体Set.Add(mb.gameObject))
                    触发到Interactable[mb.gameObject] = interactable;
            }

            Collider[] 周围Trigger = Physics.OverlapSphere(
                transform.position, 检测距离 + 2f, 检测层级, QueryTriggerInteraction.Collide);

            var 范围内Interactable = new Dictionary<GameObject, IInteractable>();

            foreach (var col in 周围Trigger)
            {
                var interactable = col.GetComponentInParent<IInteractable>();
                if (interactable == null) continue;

                var mb = interactable as MonoBehaviour;
                if (mb == null || !mb.gameObject.activeSelf) continue;

                var item = mb as InventoryItem;
                if (item != null && !LockableInteractable.IsAccessible(item)) continue;

                if (!范围内Interactable.ContainsKey(mb.gameObject) && 玩家在交互范围内(mb.gameObject))
                    范围内Interactable[mb.gameObject] = interactable;
            }

            GameObject 最佳命中物 = null;
            float 最近距离 = float.MaxValue;

            foreach (var kvp in 范围内Interactable)
            {
                var go = kvp.Key;
                bool 被射线命中 = 触发物体Set.Contains(go);

                if (!被射线命中 && !kvp.Value.不需要射线命中)
                    continue;

                float 距离 = Vector3.Distance(transform.position, go.transform.position);
                if (距离 < 最近距离)
                {
                    最近距离 = 距离;
                    最佳命中物 = go;
                }
            }

            if (最佳命中物 != null)
                新选中 = 范围内Interactable[最佳命中物];
        }

        if (新选中 != 当前可交互物体)
        {
            当前可交互物体 = 新选中;
            On选中变化?.Invoke(当前可交互物体);
        }
    }

    bool 玩家在交互范围内(GameObject obj)
    {
        var w = obj.GetComponentInParent<WardrobeController>();
        if (w != null && w.位置在有效范围内(transform.position))
            return true;

        var colliders = obj.GetComponentsInChildren<Collider>(true);

        foreach (var col in colliders)
        {
            if (!col.isTrigger) continue;

            if (col is SphereCollider sc)
            {
                Vector3 世界中心 = sc.transform.TransformPoint(sc.center);
                float 世界半径 = sc.radius * Mathf.Max(
                    sc.transform.lossyScale.x, sc.transform.lossyScale.y, sc.transform.lossyScale.z);
                if (Vector3.Distance(transform.position, 世界中心) <= 世界半径)
                    return true;
            }
            else
            {
                Vector3 最近点 = col.ClosestPoint(transform.position);
                if (Vector3.Distance(transform.position, 最近点) < 0.001f)
                    return true;
            }
        }

        foreach (var col in colliders)
        {
            if (col.isTrigger) continue;
            var bounds = col.bounds;
            bounds.Expand(0.5f);
            if (bounds.Contains(transform.position)) return true;
        }

        if (colliders.Length == 0)
            return true;

        return Vector3.Distance(transform.position, obj.transform.position) <= 检测距离;
    }

    public bool 有可交互物体()
    {
        return 当前可交互物体 != null;
    }

    public string 获取交互提示()
    {
        return 当前可交互物体 != null ? 当前可交互物体.交互提示 : "";
    }

    public void 执行交互()
    {
        if (当前可交互物体 == null) return;

        if (移动控制 != null && 移动控制.是否被抓住) return;

        var item = 当前可交互物体 as IInventoryItem;
        if (item != null && 背包 != null)
        {
            背包.AddItem(item);
        }
        else
        {
            当前可交互物体.OnInteract();
        }

        当前可交互物体 = null;
    }
}