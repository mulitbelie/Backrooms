using System;
using UnityEngine;

public interface IInventoryItem
{
    string Name { get; }
    Sprite Image { get; }

    void OnPickup();
    void OnDrop();
}

public class InventoryItem : MonoBehaviour, IInventoryItem, IInteractable
{
    [Header("物品设置")]
    public string itemName = "物品";
    public Sprite itemImage;
    [Header("交互提示")]
    public string 交互提示文本 = "拾取";

    public string Name => itemName;
    public Sprite Image => itemImage;
    public string 交互提示 => $"{交互提示文本} {itemName}";

    public virtual void OnPickup()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnDrop()
    {
        gameObject.SetActive(true);
    }

    public virtual void OnInteract()
    {
    }
}

public class InventoryEventArgs : EventArgs
{
    public InventoryEventArgs(IInventoryItem item)
    {
        Item = item;
    }
    public IInventoryItem Item;
}