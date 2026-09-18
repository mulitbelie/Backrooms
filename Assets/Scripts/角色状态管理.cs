using UnityEngine;

public class 角色状态管理 : MonoBehaviour
{
    public enum 角色状态 { 站立, 走路, 跑步, 下蹲走路, 跳跃 }

    [Header("状态设置")]
    public float 走路速度阈值 = 0.5f;

    private 基础移动控制 移动控制;
    public 角色状态 当前状态 { get; private set; }
    public 角色状态 上一帧状态 { get; private set; }

    void Start()
    {
        移动控制 = GetComponent<基础移动控制>();
        当前状态 = 角色状态.站立;
        上一帧状态 = 角色状态.站立;
    }

    void Update()
    {
        上一帧状态 = 当前状态;
        更新状态();
    }

    void 更新状态()
    {
        bool 在地面 = 移动控制.角色地面检测;
        bool 在奔跑 = 移动控制.正在奔跑;
        bool 在下蹲 = 移动控制.正在下蹲;
        float 当前速度 = 移动控制.移动;

        if (!在地面)
            当前状态 = 角色状态.跳跃;
        else if (在下蹲)
            当前状态 = 角色状态.下蹲走路;
        else if (在奔跑)
            当前状态 = 角色状态.跑步;
        else if (当前速度 > 走路速度阈值)
            当前状态 = 角色状态.走路;
        else
            当前状态 = 角色状态.站立;
    }

    public bool 状态刚变化()
    {
        return 当前状态 != 上一帧状态;
    }

    public bool 刚进入(角色状态 目标状态)
    {
        return 上一帧状态 != 目标状态 && 当前状态 == 目标状态;
    }

    public bool 刚离开(角色状态 原状态)
    {
        return 上一帧状态 == 原状态 && 当前状态 != 原状态;
    }
}