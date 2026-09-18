using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 基础移动控制 : MonoBehaviour
{
    private CharacterController 角色控制器组件;
    public float 速度;
    private float 移动速度;
    private float 平滑度=7;
    private Vector3 角色方向;
    private Vector3 角色在空中方向;
    private VariableJoystick 摇杆组件;
    private Vector2 摇杆方向;
    private float 重力= -20f;
    private float 跳跃力量 = 0.9f;
    private Vector3 角色重力的方向=new Vector3(0,-2f,0);
    private bool 角色接触地面;
    private Vector2 在空中移动的值;
    private float 在空中的速度 = 0.3f;

    [Header("奔跑设置")]
    public bool 启用奔跑 = true;
    public float 触发奔跑所需秒数 = 0.6f;
    public float 奔跑速度倍数 = 1.8f;
    [Range(0f, 1f)]
    public float 摇杆方向一致阈值 = 0.85f;

    [Header("奔跑体力（勾选启用）")]
    public bool 启用奔跑体力 = false;
    public float 奔跑持续秒数 = 5f;
    public float 奔跑冷却秒数 = 2f;

    [Header("下蹲设置")]
    public float 下蹲速度倍数 = 0.5f;
    [Range(0.3f, 1f)]
    public float 下蹲高度比例 = 0.6f;

    [Header("外部输入")]
    public Vector2 外部输入方向;

    private bool 是否在奔跑;
    private float 摇杆同向计时;
    private float 奔跑剩余时间;
    private float 冷却剩余时间;
    private Vector2 上一帧摇杆方向;

    private bool 是否在下蹲;
    private float 站立原始高度;
    private Vector3 站立原始中心;

    void Start()
    {
        角色控制器组件=GetComponent<CharacterController>();
        站立原始高度 = 角色控制器组件.height;
        站立原始中心 = 角色控制器组件.center;
    }

    public float 移动
    {
        get { return 移动速度; }
    }

    public bool 正在奔跑
    {
        get { return 是否在奔跑; }
    }

    public bool 正在下蹲
    {
        get { return 是否在下蹲; }
    }

    public VariableJoystick 摇杆配置
    {
        get { return 摇杆组件; }
        set { 摇杆组件 = value; }
    }

    public bool 角色地面检测
    {
        get { return 角色接触地面; }
    }

    void Update()
    {
        地面检测();
        奔跑状态更新();
        速度平滑();
        移动逻辑();
    }

    Vector2 当前输入方向()
    {
        Vector2 摇杆输入 = Vector2.zero;
        if (摇杆组件 != null)
        {
            摇杆输入.x = 摇杆组件.Horizontal;
            摇杆输入.y = 摇杆组件.Vertical;
        }

        Vector2 合并输入 = 摇杆输入.sqrMagnitude > 外部输入方向.sqrMagnitude ? 摇杆输入 : 外部输入方向;
        if (合并输入.magnitude > 1f) 合并输入.Normalize();
        return 合并输入;
    }

    void 地面检测()
    {
        角色接触地面 = 角色控制器组件.isGrounded;
        if (角色接触地面 && 角色重力的方向.y < -2f)
            角色重力的方向.y = -2f;
        if (角色接触地面 == false)
            角色重力的方向.y += 重力 * Time.deltaTime;
    }

    void 奔跑状态更新()
    {
        if (!启用奔跑)
        {
            if (是否在奔跑) 结束奔跑(false);
            return;
        }

        if (启用奔跑体力 && 冷却剩余时间 > 0)
        {
            冷却剩余时间 -= Time.deltaTime;
            if (冷却剩余时间 < 0) 冷却剩余时间 = 0;
        }

        摇杆方向 = 当前输入方向();

        bool 摇杆在用 = 摇杆方向.magnitude > 0.1f;

        if (!摇杆在用)
        {
            摇杆同向计时 = 0;
            if (是否在奔跑) 结束奔跑(false);
            上一帧摇杆方向 = Vector2.zero;
            return;
        }

        float 方向相似度 = Vector2.Dot(摇杆方向.normalized, 上一帧摇杆方向.normalized);
        bool 方向没变 = 上一帧摇杆方向.magnitude > 0.01f && 方向相似度 >= 摇杆方向一致阈值;

        if (方向没变)
            摇杆同向计时 += Time.deltaTime;
        else
            摇杆同向计时 = 0;

        上一帧摇杆方向 = 摇杆方向;

        if (是否在奔跑)
        {
            if (启用奔跑体力)
            {
                奔跑剩余时间 -= Time.deltaTime;
                if (奔跑剩余时间 <= 0)
                {
                    结束奔跑(true);
                    return;
                }
            }

            if (!方向没变) 结束奔跑(false);
        }
        else
        {
            bool 冷却通过 = !启用奔跑体力 || 冷却剩余时间 <= 0;
            if (冷却通过 && 角色接触地面 && !是否在下蹲 && 摇杆同向计时 >= 触发奔跑所需秒数)
            {
                开始奔跑();
            }
        }
    }

    void 开始奔跑()
    {
        是否在奔跑 = true;
        if (启用奔跑体力) 奔跑剩余时间 = 奔跑持续秒数;
    }

    void 结束奔跑(bool 触发冷却)
    {
        是否在奔跑 = false;
        if (启用奔跑体力 && 触发冷却) 冷却剩余时间 = 奔跑冷却秒数;
        摇杆同向计时 = 0;
    }

    public void 切换下蹲状态()
    {
        if (!是否在下蹲)
        {
            是否在下蹲 = true;
            if (是否在奔跑) 结束奔跑(false);
            角色控制器组件.height = 站立原始高度 * 下蹲高度比例;
            角色控制器组件.center = new Vector3(站立原始中心.x, 站立原始中心.y * 下蹲高度比例, 站立原始中心.z);
        }
        else
        {
            是否在下蹲 = false;
            角色控制器组件.height = 站立原始高度;
            角色控制器组件.center = 站立原始中心;
        }
    }

    public void 跳跃()
    {
        if(!角色接触地面)return;
        if(是否在下蹲)return;
        角色重力的方向.y = Mathf.Sqrt(跳跃力量 * -2f * 重力);
        在空中移动的值 = 当前输入方向();
        角色方向 = (transform.right * 在空中移动的值.x + transform.forward * 在空中移动的值.y).normalized;
        if (是否在奔跑) 结束奔跑(false);
    }

    void 移动逻辑()
    {
        if (角色接触地面) 角色方向 = (transform.right * 摇杆方向.x + transform.forward * 摇杆方向.y).normalized;
        else { 角色在空中方向 = (transform.right * 摇杆方向.x + transform.forward * 摇杆方向.y).normalized; 角色控制器组件.Move(角色在空中方向 * Time.deltaTime * 速度*在空中的速度); }
        角色控制器组件.Move(角色方向 * Time.deltaTime * 移动速度);
        角色控制器组件.Move(角色重力的方向*Time.deltaTime);
    }

    void 速度平滑()
    {
        float 奔跑倍数 = 是否在奔跑 ? 奔跑速度倍数 : 1f;
        float 下蹲倍数 = 是否在下蹲 ? 下蹲速度倍数 : 1f;
        移动速度 = Mathf.Lerp(移动速度, 速度 * 奔跑倍数 * 下蹲倍数 * 摇杆变化值(), 平滑度 * Time.deltaTime);
    }

    float 摇杆变化值()
    {
        Vector2 输入 = 当前输入方向();
        if (Math.Abs(输入.x) > Math.Abs(输入.y))
            return Math.Abs(输入.x);
        else return Math.Abs(输入.y);
    }

    public bool 检测角色拖动摇杆()
    {
        if(摇杆方向.x!=0|| 摇杆方向.y != 0)
            return true;
        else return false;
    }
}