using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : Animal
{
    public override void Speak()
    {
        Debug.Log("【狗】汪汪汪");
    }
    public void ShowName()
    {
        Debug.Log($"我是一个{name}");//name是父类Animal的属性，用于存储动物的名称,Dog类继承了Animal的属性所以可以使用
    }
        public void ShowAge()
    {
        Debug.Log($"我今年{age}岁");
    }

}
