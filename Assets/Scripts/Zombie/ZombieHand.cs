using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHand : MonoBehaviour
{
    private Zombie zombie;

    //获取僵尸的伤害值，如果僵尸对象为空或僵尸已死亡则返回0
    public int damage => (zombie != null && !zombie.IsDead) ? zombie.zombieDamage : 0;

    private void Start()
    {
        zombie = GetComponentInParent<Zombie>();
    }
}

//三元运算符
//zombie != null → 如果僵尸对象不为空且僵尸未死亡
//!zombie.IsDead → 且僵尸未死亡
//→ 返回 zombie.zombieDamage
//否则（zombie 是 null）
//→ 返回 0
