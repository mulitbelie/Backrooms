using UnityEngine;
using UnityEngine.UI;

public class 射击UI控制 : MonoBehaviour
{
    public GameObject 目标UI;

    private Weapon 武器组件;
    private bool 使用CanvasGroup隐藏;
    private CanvasGroup canvasGroup;

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

        Invoke(nameof(初始化), 0.1f);
    }

    void 初始化()
    {
        查找武器组件();
        if (武器组件 != null)
        {
            武器组件.OnPickupStateChanged += 处理拾取状态;
            处理拾取状态(武器组件.HasWeapon());
        }
    }

    void OnDestroy()
    {
        if (武器组件 != null)
        {
            武器组件.OnPickupStateChanged -= 处理拾取状态;
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

    void 查找武器组件()
    {
        if (全局脚本.Instance != null && 全局脚本.Instance.玩家 != null)
        {
            武器组件 = 全局脚本.Instance.玩家.GetComponentInChildren<Weapon>();
            if (武器组件 != null) return;
        }

        武器组件 = FindObjectOfType<Weapon>(true);
    }

    public void 射击按下()
    {
        if (武器组件 == null) return;

        武器组件.外部持续射击 = true;
        武器组件.外部点击射击 = true;
    }

    public void 射击抬起()
    {
        if (武器组件 == null) return;

        武器组件.外部持续射击 = false;
    }
}