using UnityEngine;
using System.Collections.Generic;

public class 交互检测 : MonoBehaviour
{
    [Header("射线设置")]
    public float 检测距离 = 3f;
    public LayerMask 检测层级 = ~0;

    [Header("背包（自动查找）")]
    public Inventory 背包;

    private IInteractable 当前可交互物体;
    private Camera 主相机;
    private 基础移动控制 移动控制;

    public IInteractable 当前物体 => 当前可交互物体;

    void Start()
    {
        主相机 = Camera.main;
        if (检测层级.value == 64)
        {
            检测层级 = ~0;
            Debug.LogWarning("[交互检测] 检测层级只有 Layer 6，已自动扩展为所有层以支持门交互");
        }
        if (背包 == null)
        {
            背包 = GetComponentInParent<Inventory>();
            if (背包 == null)
                背包 = FindObjectOfType<Inventory>();
        }
        移动控制 = GetComponentInParent<基础移动控制>();
        if (移动控制 == null) 移动控制 = FindObjectOfType<基础移动控制>();
    }

    void Update()
    {
        刷新可交互物体();
    }

    void 刷新可交互物体()
    {
        当前可交互物体 = null;
        if (主相机 == null) return;

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
            当前可交互物体 = 范围内Interactable[最佳命中物];
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