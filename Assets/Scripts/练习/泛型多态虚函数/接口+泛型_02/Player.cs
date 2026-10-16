using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace 接口泛型{
public class Player : IDamageable<DamageInfo>
{
    public int hp = 200;
    public void Damage(DamageInfo damageInfo)//实现接口方法
    {
        hp -= damageInfo.value;
        Debug.Log($"玩家受到{damageInfo.type}伤害，扣{damageInfo.value}点血量，当前血量为{hp}");
    }
}

}
