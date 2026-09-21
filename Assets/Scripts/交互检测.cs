using UnityEngine;

public class 交互检测 : MonoBehaviour
{
    [Header("射线设置")]
    public float 检测距离 = 3f;
    public LayerMask 检测层级 = ~0;

    [Header("背包（自动查找）")]
    public Inventory 背包;

    private IInteractable 当前可交互物体;
    private Camera 主相机;

    public IInteractable 当前物体 => 当前可交互物体;

    void Start()
    {
        主相机 = Camera.main;
        if (背包 == null)
        {
            背包 = GetComponentInParent<Inventory>();
            if (背包 == null)
                背包 = FindObjectOfType<Inventory>();
        }
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

        if (!Physics.Raycast(ray, out RaycastHit hit, 检测距离, 检测层级)) return;

        var interactable = hit.collider.GetComponentInParent<IInteractable>();
        if (interactable == null) return;

        var mb = interactable as MonoBehaviour;
        if (mb != null && !mb.gameObject.activeSelf) return;

        当前可交互物体 = interactable;
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