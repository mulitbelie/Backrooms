using UnityEngine;

public class 键盘控制 : MonoBehaviour
{
    private 基础移动控制 移动控制;

    [Header("动作按键")]
    public KeyCode 跳跃键 = KeyCode.Space;
    public KeyCode 下蹲键 = KeyCode.LeftControl;

    void Start()
    {
        移动控制 = GetComponent<基础移动控制>();
        if (移动控制 == null)
            移动控制 = transform.root.GetComponentInChildren<基础移动控制>();
    }

    void Update()
    {
        if (移动控制 == null) return;

        if (Input.GetKeyDown(跳跃键)) 移动控制.跳跃();
        if (Input.GetKeyDown(下蹲键) || Input.GetKeyUp(下蹲键)) 移动控制.切换下蹲状态();
    }
}