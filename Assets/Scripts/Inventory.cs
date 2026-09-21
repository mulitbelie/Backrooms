using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public const int SLOTS = 3;

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
        item.OnDrop();

        Collider[] colliders = (item as MonoBehaviour)?.GetComponentsInChildren<Collider>();
        if (colliders != null)
        {
            foreach (var c in colliders)
            {
                if (c != null && !c.enabled) c.enabled = true;
            }
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