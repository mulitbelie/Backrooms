using UnityEngine;

public class 射击UI控制 : MonoBehaviour
{
    private Weapon 武器组件;
    private bool 已查找过武器;

    void 查找武器组件()
    {
        已查找过武器 = true;

        var 全局 = FindObjectOfType<全局脚本>();
        if (全局 != null && 全局.玩家 != null)
        {
            武器组件 = 全局.玩家.GetComponentInChildren<Weapon>();
            if (武器组件 != null) return;
        }

        武器组件 = FindObjectOfType<Weapon>(true);
        if (武器组件 == null)
            Debug.LogWarning("[射击] 没找到 Weapon 组件");
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