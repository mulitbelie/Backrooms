using UnityEngine;
using UnityEngine.UI;

public class 手电筒UI控制 : MonoBehaviour
{
    public Sprite[] 图片ui;

    private FleshLight 手电筒组件;
    private Image image组件;
    private bool 已查找过;

    void Start()
    {
        image组件 = GetComponent<Image>();
    }

    void 查找手电筒组件()
    {
        已查找过 = true;

        var 全局 = FindObjectOfType<全局脚本>();
        if (全局 != null && 全局.玩家 != null)
        {
            手电筒组件 = 全局.玩家.GetComponentInChildren<FleshLight>();
            if (手电筒组件 != null) return;
        }

        手电筒组件 = FindObjectOfType<FleshLight>(true);
        if (手电筒组件 == null)
            Debug.LogWarning("[手电筒] 没找到 FleshLight 组件");
    }

    public void 切换手电筒()
    {
        if (!已查找过) 查找手电筒组件();

        if (手电筒组件 == null)
        {
            Debug.LogWarning("[手电筒] 手电筒组件为 null");
            return;
        }

        手电筒组件.ToggleFlashlight();
        更新图片();
    }

    void 更新图片()
    {
        if (image组件 == null) return;

        bool 开着 = 手电筒组件 != null && 手电筒组件.IsOn();

        if (开着)
        {
            if (图片ui.Length > 1) image组件.sprite = 图片ui[1];
        }
        else
        {
            if (图片ui.Length > 0) image组件.sprite = 图片ui[0];
        }
    }
}