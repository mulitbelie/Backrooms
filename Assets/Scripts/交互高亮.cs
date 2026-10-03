using UnityEngine;

public class 交互高亮 : MonoBehaviour
{
    public 交互检测 检测;

    private Outline 当前高亮;

    void Start()
    {
        if (检测 == null)
            检测 = GetComponent<交互检测>();

        if (检测 != null)
            检测.On选中变化 += 处理选中变化;
    }

    void OnDestroy()
    {
        if (检测 != null)
            检测.On选中变化 -= 处理选中变化;
    }

    void 处理选中变化(IInteractable 新选中物)
    {
        if (当前高亮 != null)
        {
            当前高亮.enabled = false;
            当前高亮 = null;
        }

        if (新选中物 == null) return;

        var mb = 新选中物 as MonoBehaviour;
        if (mb == null) return;

        var outline = mb.GetComponent<Outline>();
        if (outline == null)
            outline = mb.GetComponentInChildren<Outline>();

        if (outline != null)
        {
            outline.enabled = true;
            当前高亮 = outline;
        }
    }
}