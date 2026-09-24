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
    public string 交互提示文本 = "pick up";

    public string Name => itemName;
    public Sprite Image => itemImage;

    public string 交互提示 => $"{平台工具.按键提示} to {交互提示文本} {itemName}";

    public float triggerRadius = 1.5f;

    void Awake()
    {
        SphereCollider trigger = gameObject.GetComponent<SphereCollider>();
        if (trigger == null)
            trigger = gameObject.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = triggerRadius;
        if(triggerRadius < 1){
            triggerRadius = 1;
        }
    }

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