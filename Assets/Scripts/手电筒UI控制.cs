using UnityEngine;
using UnityEngine.UI;

public class 手电筒UI控制 : MonoBehaviour
{
    public Sprite[] 图片ui;
    public GameObject 目标UI;

    private FlashLight 手电筒组件;
    private Image image组件;
    private CanvasGroup canvasGroup;
    private bool 使用CanvasGroup隐藏;

    void Start()
    {
        if (目标UI == null)
        {
            目标UI = gameObject;
        }

        if (目标UI == gameObject)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            使用CanvasGroup隐藏 = true;
        }
        else
        {
            目标UI.SetActive(false);
            使用CanvasGroup隐藏 = false;
        }

        image组件 = GetComponent<Image>();

        Invoke(nameof(初始化), 0.1f);
    }

    void 初始化()
    {
        查找手电筒组件();
        if (手电筒组件 != null)
        {
            手电筒组件.OnPickupStateChanged += 处理拾取状态;
            手电筒组件.OnLightStateChanged += 处理灯光状态;

            处理拾取状态(手电筒组件.HasFlashlight());
            处理灯光状态(手电筒组件.IsOn());
        }
    }

    void OnDestroy()
    {
        if (手电筒组件 != null)
        {
            手电筒组件.OnPickupStateChanged -= 处理拾取状态;
            手电筒组件.OnLightStateChanged -= 处理灯光状态;
        }
    }

    void 处理拾取状态(bool 已持有)
    {
        if (使用CanvasGroup隐藏)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 已持有 ? 1f : 0f;
                canvasGroup.blocksRaycasts = 已持有;
                canvasGroup.interactable = 已持有;
            }
        }
        else
        {
            目标UI.SetActive(已持有);
        }
    }

    void 处理灯光状态(bool 开着)
    {
        if (image组件 == null) return;

        if (开着)
        {
            if (图片ui.Length > 1) image组件.sprite = 图片ui[1];
        }
        else
        {
            if (图片ui.Length > 0) image组件.sprite = 图片ui[0];
        }
    }

    void 查找手电筒组件()
    {
        if (全局脚本.Instance != null && 全局脚本.Instance.玩家 != null)
        {
            手电筒组件 = 全局脚本.Instance.玩家.GetComponentInChildren<FlashLight>();
            if (手电筒组件 != null) return;
        }

        手电筒组件 = FindObjectOfType<FlashLight>(true);
        if (手电筒组件 == null)
            Debug.LogWarning("[手电筒] 没找到 FlashLight 组件");
    }

    public void 切换手电筒()
    {
        if (手电筒组件 == null) return;
        手电筒组件.ToggleFlashlight();
    }
}