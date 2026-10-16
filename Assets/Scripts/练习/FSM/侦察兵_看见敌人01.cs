using UnityEngine;

/// <summary>
/// 侦察兵【看见敌人】
/// </summary>
public class 侦察兵_看见敌人 : 侦察兵
{
    public GameObject 目标敌人;
    public GameObject 自身;

    public 侦察兵_看见敌人()
    {
        Debug.Log("侦察兵【看见敌人】：生成侦察兵实体");
    }
    // 重写Init：除了父类挂牌，额外绑定游戏物体
    public override void Init(触发ID id) //触发ID 工牌为 看见敌人
    {
        // 调用父类的Init方法，设置触发编号
        base.Init(id);
        // 绑定游戏物体
        //不再Find，司令官外部赋值
        //自身 = GameObject.Find("AI");//全局找物体名 名字不能错
        //目标敌人 = GameObject.Find("Player");//全局找物体名 名字不能错
        Debug.Log("看见敌人侦察兵，绑定自身和敌人对象");
    }
    // 重写检测条件：判断看见敌人的条件
    public override bool 检测条件()
    {
        float 距离 = Vector3.Distance(自身.transform.position, 目标敌人.transform.position);
        return 距离 < 10;
    }
}
