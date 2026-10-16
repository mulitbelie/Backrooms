using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : IDamageable<int>//继承实现接口
{
    public int hp = 100;
    public void Damage(int damageTaken)//继承实现接口IDamageable必须有 实现Damage方法 否则会报错
    {
        if (hp > 0)
        {
           hp -= damageTaken;
           Debug.Log($"哥布林受到 {damageTaken} 伤害, 剩余 {hp} 点生命值");
        }
        else
        {
            Debug.Log($"哥布林已死亡");
        }

    }

}
