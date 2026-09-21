using UnityEngine;

public class 物品拾取检测 : MonoBehaviour
{
    public Inventory inventory;
    public string itemTag = "";
    public KeyCode pickKey = KeyCode.E;
    public float pickRange = 3f;
    public LayerMask 检测层级 = ~0;

    private IInventoryItem 当前可拾取物品;
    private Camera 主相机;

    void Start()
    {
        if (inventory == null)
            inventory = GetComponent<Inventory>();
        if (inventory == null)
            inventory = FindObjectOfType<Inventory>();

        主相机 = Camera.main;
    }

    void Update()
    {
        刷新可拾取物品();

        if (当前可拾取物品 != null && Input.GetKeyDown(pickKey))
        {
            执行拾取();
        }
    }

    void 刷新可拾取物品()
    {
        当前可拾取物品 = null;
        if (主相机 == null) return;

        Ray ray = 主相机.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (!Physics.Raycast(ray, out RaycastHit hit, pickRange, 检测层级)) return;

        var item = hit.collider.GetComponentInParent<InventoryItem>();
        if (item == null) return;
        if (!string.IsNullOrEmpty(itemTag) && !item.CompareTag(itemTag)) return;
        if (!item.gameObject.activeSelf) return;

        当前可拾取物品 = item;
    }

    public void 执行拾取()
    {
        if (当前可拾取物品 == null) return;

        if (inventory != null)
        {
            inventory.AddItem(当前可拾取物品);
        }
        当前可拾取物品 = null;
    }

    public bool 有可拾取物品()
    {
        return 当前可拾取物品 != null;
    }

    public IInventoryItem 获取当前可拾取物品()
    {
        return 当前可拾取物品;
    }
}