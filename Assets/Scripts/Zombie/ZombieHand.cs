using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHand : MonoBehaviour
{
    private Zombie zombie;


    public int damage => zombie != null ? zombie.zombieDamage : 0;//获取僵尸的伤害值，如果僵尸对象为空则返回0

    private void Start()
    {
        zombie = GetComponentInParent<Zombie>();
    }
}

//三元运算符
//zombie != null → 如果僵尸对象不为空
//→ 返回 zombie.zombieDamage
//否则（zombie 是 null）
//→ 返回 0
