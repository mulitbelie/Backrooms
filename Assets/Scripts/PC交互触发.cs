using UnityEngine;

public class PC交互触发 : MonoBehaviour
{
    public 交互检测 检测;
    public 物品拾取检测 拾取检测;
    public KeyCode 交互键 = KeyCode.E;
    bool 上帧笔记显示中;

    void Start()
    {
        if (检测 == null)
            检测 = GetComponent<交互检测>();
        if (拾取检测 == null)
            拾取检测 = GetComponent<物品拾取检测>();
    }

    void Update()
    {
        if (检测 == null && 拾取检测 == null) return;

        bool 笔记显示中 = PlayerNote.Instance != null && PlayerNote.Instance.IsShowing;
        if (笔记显示中)
        {
            上帧笔记显示中 = true;
            return;
        }

        if (上帧笔记显示中)
        {
            上帧笔记显示中 = false;
            return;
        }

        if (!Input.GetKeyDown(交互键)) return;

        if (检测 != null && 检测.有可交互物体())
        {
            检测.执行交互();
        }
        else if (拾取检测 != null && 拾取检测.有可拾取物品())
        {
            拾取检测.执行拾取();
        }
    }
}