using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal
{
    public string name;//初始化一个字符串类型的属性name，用于存储动物的名称
    public int age;
    public virtual void Speak()
    {
        
        Debug.Log("【动物】发出声音");
    }
}
