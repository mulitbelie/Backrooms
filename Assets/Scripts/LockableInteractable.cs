using UnityEngine;

public abstract class LockableInteractable : MonoBehaviour, IInteractable
{
    [Header("锁设置")]
    public bool isLocked = false;
    public string keyItemName = "key";

    public bool isOpen = false;

    private Inventory _背包;
    public float triggerRadius = 2f;
    protected Inventory 背包
    {
        get
        {
            if (_背包 == null)
                _背包 = FindObjectOfType<Inventory>();
            return _背包;
        }
    }

    protected abstract string 对象名 { get; }

    public virtual bool isFullyOpen => isOpen;

    public string 交互提示
    {
        get
        {
            if (isLocked)
            {
                if (背包 != null && 背包.Contains(keyItemName))
                    return $"locked {对象名} ({平台工具.按键提示} to unlock with {keyItemName})";
                else
                    return $"locked {对象名} (need key)";
            }
            return isOpen
                ? $"{平台工具.按键提示} to close {对象名}"
                : $"{平台工具.按键提示} to open {对象名}";
        }
    }

    void Awake()
    {
        SphereCollider trigger = gameObject.GetComponent<SphereCollider>();
        if (trigger == null)
            trigger = gameObject.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = triggerRadius;
    }

    public void OnInteract()
    {
        if (isLocked)
        {
            if (背包 != null && 背包.Contains(keyItemName))
            {
                var key = 背包.GetItem(keyItemName);
                if (key != null)
                {
                    背包.DestroyItem(key);
                    isLocked = false;
                    Toggle();
                }
            }
            return;
        }

        Toggle();
    }

    protected virtual void Toggle()
    {
        isOpen = !isOpen;
    }
    // 检查物品是否可访问 （所有父容器都必须isOpen 为 true ）
    // 用于检查物品是否在柜子里，是否需要先打开柜门才能拾取
    public static bool IsAccessible(InventoryItem item)
    {
        Transform current = item.transform;
        while (current != null)
        {
            var container = current.GetComponent<LockableInteractable>();
            if (container != null && !container.isFullyOpen)
                return false;

            for (int i = 0; i < current.childCount; i++)
            {
                var childContainer = current.GetChild(i).GetComponent<LockableInteractable>();
                if (childContainer != null && !childContainer.isFullyOpen)
                    return false;
            }

            current = current.parent;
        }
        return true;
    }
}