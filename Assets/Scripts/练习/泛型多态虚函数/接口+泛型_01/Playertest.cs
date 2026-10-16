using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playertest : MonoBehaviour
{
    public int takeDamage = 10;
    Goblin goblin;//定义一个哥布林对象方便所有方法调用，不需要每个方法调用都创建一个对象
    void Start()//玩家开始时 创建一个哥布林对象
    {
          goblin = new Goblin();//内存里创建一个哥布林对象
          
    }
    void Update()//玩家更新时 调用Damage方法 传入10点伤害
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("按了空格键");
            goblin.Damage(takeDamage);//调用哥布林的Damage方法 传入10点伤害
        }
    }

}

