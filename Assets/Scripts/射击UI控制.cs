using UnityEngine;
using UnityEngine.UI;

public class 射击UI控制 : MonoBehaviour
{
    public GameObject 目标UI;

    private Weapon 武器组件;
    private bool 已查找过武器;
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
            Debug.Log("[射击UI] 使用 CanvasGroup 隐藏自身");
        }
        else
        {
            目标UI.SetActive(false);
            使用CanvasGroup隐藏 = false;
            Debug.Log("[射击UI] SetActive 隐藏目标: " + 目标UI.name);
        }
    }

    void Update()
    {
        if (!已查找过武器) 查找武器组件();
        if (武器组件 == null) return;

        bool 持有枪 = 武器组件.HasWeapon();

        if (使用CanvasGroup隐藏)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 持有枪 ? 1f : 0f;
                canvasGroup.blocksRaycasts = 持有枪;
                canvasGroup.interactable = 持有枪;
            }
        }
        else
        {
            目标UI.SetActive(持有枪);
        }
    }

    void 查找武器组件()
    {
        已查找过武器 = true;

        var 全局 = FindObjectOfType<全局脚本>();
        if (全局 != null && 全局.玩家 != null)
        {
            武器组件 = 全局.玩家.GetComponentInChildren<Weapon>();
            if (武器组件 != null)
            {
                Debug.Log("[射击UI] 找到 Weapon (从玩家): " + 武器组件.name);
                return;
            }
        }

        武器组件 = FindObjectOfType<Weapon>(true);
        if (武器组件 != null)
        {
            Debug.Log("[射击UI] 找到 Weapon (全场景): " + 武器组件.name);
        }
        else
        {
            Debug.LogWarning("[射击UI] 没找到 Weapon 组件");
        }
    }

    public void 射击按下()
    {
        if (!已查找过武器) 查找武器组件();
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