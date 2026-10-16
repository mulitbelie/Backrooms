using UnityEngine;

public class 侦察兵_待机计时: 侦察兵
{
    public float 等待时长 = 3f;
    private float 计时器;
    public GameObject 自身;

    public override void Init(触发ID id)
    {
        base.Init(id);
        计时器 = 0;
    }

    public override bool 检测条件()
    {
        // 计时累加
        计时器 += Time.deltaTime;
        if (计时器 >= 等待时长)
        {
            计时器 = 0; //重置计时器
            return true;
        }
        return false;
    }
}

