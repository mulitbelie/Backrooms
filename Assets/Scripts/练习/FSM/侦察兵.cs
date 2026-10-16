using UnityEngine;//抽象父类 + 两个侦察兵子类（带构造 + Init）

/// <summary>
/// 侦察兵【抽象父类】
/// </summary>
 public abstract class 侦察兵
{
    public 触发ID 触发编号;//永久的工牌卡槽：**侦察兵实例身上的成员变量，外部（司令官那边）可以读取使用，永久存在**

    public 侦察兵()
    {
        Debug.Log("侦察兵【构造函数】：实体出场，空白工牌");
    }

    //=========================================
    public virtual void Init(触发ID id)
    {
        Debug.Log("侦察兵【Init】：初始化工牌，设置触发编号");
        触发编号 = id;//临时纸条（拷贝，只在这个方法内部有效）只在 Init 这个函数内部临时使用，只在这一次函数执行期间有效，不属于侦察兵。
    }

    public abstract bool 检测条件();

}
