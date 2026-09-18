using UnityEngine;

public class 键盘控制 : MonoBehaviour
{
    private 基础移动控制 移动控制;

    [Header("移动按键")]
    public KeyCode 前进键 = KeyCode.W;
    public KeyCode 后退键 = KeyCode.S;
    public KeyCode 左移键 = KeyCode.A;
    public KeyCode 右移键 = KeyCode.D;

    [Header("动作按键")]
    public KeyCode 跳跃键 = KeyCode.Space;
    public KeyCode 下蹲键 = KeyCode.LeftControl;

    void Start()
    {
        移动控制 = GetComponent<基础移动控制>();
    }

    void Update()
    {
        更新移动输入();
        检测动作按键();
    }

    void 更新移动输入()
    {
        Vector2 键盘输入 = Vector2.zero;
        if (Input.GetKey(左移键)) 键盘输入.x -= 1;
        if (Input.GetKey(右移键)) 键盘输入.x += 1;
        if (Input.GetKey(后退键)) 键盘输入.y -= 1;
        if (Input.GetKey(前进键)) 键盘输入.y += 1;
        if (键盘输入.magnitude > 1f) 键盘输入.Normalize();

        移动控制.外部输入方向 = 键盘输入;
    }

    void 检测动作按键()
    {
        if (Input.GetKeyDown(跳跃键)) 移动控制.跳跃();
        if (Input.GetKeyDown(下蹲键) || Input.GetKeyUp(下蹲键)) 移动控制.切换下蹲状态();
    }
}