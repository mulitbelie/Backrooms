using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 部队状态【状态机类】
/// </summary>
//抽象类+构造函数 父类
public abstract class 部队
{
    public 状态ID 本部队ID;//这支部队自己的工牌编号 类型是状态ID 枚举类型
    //战斗手册，当前状态的跳转规则
    public Dictionary<触发ID, 状态ID> 战斗手册 = new Dictionary<触发ID, 状态ID>();

    //AI本地引用，所有部队状态共用
    protected GameObject aiObj;//AI对象引用
    protected Transform aiTrans;//AI对象引用的Transform组件
    protected NavMeshAgent agent;//AI对象引用的NavMeshAgent组件

    // Init：注入AI引用，上岗初始化
    public virtual void Init(GameObject aiGameObject)
    {
        aiObj = aiGameObject;//注入AI对象引用，上岗初始化
        aiTrans = aiObj.transform;
        agent = aiObj.GetComponent<NavMeshAgent>();
        Debug.Log("部队【Init】：拿到AI导航组件");
    }
    //构造函数：实体部队出厂
    public 部队()
    {
        Debug.Log("部队状态【实体部队出厂】：生成部队状态实体");
    }

    public abstract void 进入状态();//为方法体，子类必须实现
    public abstract void 状态更新();//为方法体，子类必须实现
    public abstract void 离开状态();//为方法体，子类必须实现

}
//=========子类 【待机部队】待机状态 =========
public class 部队_待机 : 部队//继承部队类
{
    public override void 进入状态()
    {
        Debug.Log("【进入状态】原地不动");
        agent.isStopped = true;
    }
    public override void 状态更新()
    {
        //Debug.Log("待机：停止移动");
        agent.isStopped = true;
    }
    public override void 离开状态()
    {
        Debug.Log("【离开待机】");
    }
}
// =========子类 【巡逻部队】巡逻状态 =========
public class 部队_巡逻 : 部队//继承部队类
{
    // 巡逻路点列表，在司令官那边传进来
    public Transform[] 巡逻路点;
    private int 当前路点索引;
    public override void 进入状态()
    {
        Debug.Log("【进入状态】开始来回走");
        当前路点索引 = 0;
        agent.isStopped = false;
        if(巡逻路点 != null && 巡逻路点.Length > 0)
        {
        agent.SetDestination(巡逻路点[当前路点索引].position);//设置目标为第一个路点
        }
    }
    public override void 状态更新()
    {
        //Debug.Log("巡逻中....");
        //判断到达当前路点，切换一下
        if(!agent.pathPending && agent.remainingDistance < 0.5f)//如果路径完成，且距离小于0.5f
        {
            当前路点索引 = (当前路点索引 + 1) % 巡逻路点.Length;//切换到下一个路点，如果到头了，就从头开始循环

            agent.SetDestination(巡逻路点[当前路点索引].position);//设置目标为下一个路点
        }
    }
    public override void 离开状态()
    {
        Debug.Log("【离开巡逻】");
    }
}
//=========子类 【追击部队】追击状态 =========
public class 部队_追击 : 部队//继承部队类
{
    public Transform 玩家目标;
    public override void 进入状态()
    {
        Debug.Log("【进入状态】冲向目标");
        agent.isStopped = false;
    }
    public override void 状态更新()
    {
        
        //Debug.Log("追击中....");
        if(玩家目标 != null)
        {
            agent.SetDestination(玩家目标.transform.position);//设置目标为玩家位置
        }
    }
    public override void 离开状态()
    {
        Debug.Log("【离开追击】");
    }

}