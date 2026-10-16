using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace 接口泛型{
public class TestDemo接口 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Player player = new Player();//实例化玩家2
        DamageInfo dmg = new DamageInfo();//实例化DamageInfo
        dmg.value = 10;//设置伤害值
        dmg.type = "魔法";//设置伤害类型
        player.Damage(dmg);//调用玩家的Damage方法
    }

}

}
