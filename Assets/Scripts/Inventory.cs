using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public const int SLOTS = 3;

    [Header("放下物品设置")]
    public float 放下前方距离 = 0.8f;
    public float 放下空中高度 = 5f;
    public float 射线最大距离 = 30f;
    public float 贴地偏移 = 0.02f;
    [Tooltip("只检测指定层，留空则穿透所有物体找最低的落点")]
    public LayerMask 地面层 = ~0;

    private List<IInventoryItem> mItems = new List<IInventoryItem>();

    public event EventHandler<InventoryEventArgs> ItemAdded;
    public event EventHandler<InventoryEventArgs> ItemRemoved;

    public int Count => mItems.Count;
    public int Capacity => SLOTS;
    public bool IsFull => mItems.Count >= SLOTS;

    public IInventoryItem this[int index]
    {
        get { return mItems[index]; }
    }

    public bool AddItem(IInventoryItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("[Inventory] AddItem: item 为 null");
            return false;
        }
        if (mItems.Count >= SLOTS)
        {
            Debug.LogWarning($"[Inventory] AddItem: 背包已满 ({mItems.Count}/{SLOTS})");
            return false;
        }
        if (mItems.Contains(item))
        {
            Debug.LogWarning($"[Inventory] AddItem: 已包含该物品");
            return false;
        }

        Collider[] colliders = (item as MonoBehaviour)?.GetComponentsInChildren<Collider>();
        if (colliders != null)
        {
            foreach (var c in colliders)
            {
                if (c != null && c.enabled) c.enabled = false;
            }
        }

        mItems.Add(item);
        item.OnPickup();

        ItemAdded?.Invoke(this, new InventoryEventArgs(item));
        return true;
    }

    public bool RemoveItem(IInventoryItem item)
    {
        if (item == null) return false;
        if (!mItems.Contains(item)) return false;

        mItems.Remove(item);

        var mb = item as MonoBehaviour;
        if (mb != null)
        {
            var itemTf = mb.transform;
            var ownerTf = transform;

            // 把物品放到玩家前方空中，再激活避免在老位置闪现
            Vector3 dropPos = ownerTf.position + ownerTf.forward * 放下前方距离 + Vector3.up * 放下空中高度;
            itemTf.position = dropPos;
            itemTf.rotation = Quaternion.identity;
            item.OnDrop();

            // 临时禁用物品自身 Collider 再射线，避免自挡
            Collider[] itemColliders = mb.GetComponentsInChildren<Collider>();
            foreach (var c in itemColliders) c.enabled = false;

            // 射线穿通用 RaycastAll，跳过中间小物体，取最低的有效 hit
            int 最终Mask = 地面层 & ~(1 << ownerTf.gameObject.layer);
            RaycastHit[] hits = Physics.RaycastAll(dropPos, Vector3.down, 射线最大距离, 最终Mask);

            RaycastHit? 最低Hit = null;
            foreach (var h in hits)
            {
                bool 是自身 = false;
                foreach (var c in itemColliders)
                    if (h.collider == c) { 是自身 = true; break; }
                if (是自身) continue;

                if (!最低Hit.HasValue || h.point.y < 最低Hit.Value.point.y)
                    最低Hit = h;
            }

            if (最低Hit.HasValue)
                itemTf.position = 最低Hit.Value.point + Vector3.up * 贴地偏移;

            // 恢复 Collider
            foreach (var c in itemColliders) c.enabled = true;
        }
        else
        {
            item.OnDrop();
        }

        ItemRemoved?.Invoke(this, new InventoryEventArgs(item));
        return true;
    }

    public IInventoryItem RemoveItem(int index)
    {
        if (index < 0 || index >= mItems.Count) return null;
        var item = mItems[index];
        RemoveItem(item);
        return item;
    }

    public bool DestroyItem(IInventoryItem item)
    {
        if (item == null) return false;
        if (!mItems.Contains(item)) return false;

        mItems.Remove(item);

        var mb = item as MonoBehaviour;
        if (mb != null)
        {
            Destroy(mb.gameObject);
        }

        ItemRemoved?.Invoke(this, new InventoryEventArgs(item));
        return true;
    }

    public bool Contains(IInventoryItem item)
    {
        return item != null && mItems.Contains(item);
    }

    public bool Contains(string itemName)
    {
        return mItems.Exists(i => i.Name == itemName);
    }

    public IInventoryItem GetItem(string itemName)
    {
        return mItems.Find(i => i.Name == itemName);
    }

    public IInventoryItem GetItem(int index)
    {
        if (index < 0 || index >= mItems.Count) return null;
        return mItems[index];
    }

    public void Clear()
    {
        for (int i = mItems.Count - 1; i >= 0; i--)
        {
            RemoveItem(mItems[i]);
        }
    }

    public IEnumerator<IInventoryItem> GetEnumerator()
    {
        return mItems.GetEnumerator();
    }
}