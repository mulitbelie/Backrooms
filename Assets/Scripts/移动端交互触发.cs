using UnityEngine;

public class 移动端交互触发 : MonoBehaviour
{
    public 交互检测 检测;

    void Start()
    {
        if (检测 == null)
            检测 = GetComponent<交互检测>();
    }

    public void 交互按钮点击()
    {
        if (检测 == null) return;
        检测.执行交互();
    }
}