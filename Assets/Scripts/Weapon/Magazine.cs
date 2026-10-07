using UnityEngine;

public class Magazine : InventoryItem
{
    [Header("弹夹设置")]
    [Tooltip("弹夹内弹药数量，换弹时武器会填充这么多子弹（不超过武器弹夹容量）")]
    public int ammoCount = 12;
    [Tooltip("弹夹标签，武器通过此标签查找弹夹")]
    public string magTag = "Magazine";

    void Awake()
    {
        itemName = string.IsNullOrEmpty(itemName) ? "Magazine" : itemName;
        if (gameObject.tag != magTag && !string.IsNullOrEmpty(magTag))
        {
            try { gameObject.tag = magTag; } catch { }
        }
    }
}