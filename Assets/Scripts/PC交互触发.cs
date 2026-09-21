using UnityEngine;

public class PC交互触发 : MonoBehaviour
{
    public 交互检测 检测;
    public KeyCode 交互键 = KeyCode.E;

    void Start()
    {
        if (检测 == null)
            检测 = GetComponent<交互检测>();
    }

    void Update()
    {
        if (检测 == null) return;
        if (!检测.有可交互物体()) return;
        if (!Input.GetKeyDown(交互键)) return;

        检测.执行交互();
    }
}