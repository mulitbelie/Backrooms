using UnityEngine;
using System;

public class EnumExample_03 : MonoBehaviour
{
    //【枚举】
    public enum MySkill{攻击,防御,治疗};
    // public enum MySkill{攻击=10,防御,治疗};//可以自定义枚举编号
    public MySkill mySkill;
    public int 转换成序枚举编号;

    void Update()
    {
        转换成序枚举编号 = (int)mySkill;
    }
}