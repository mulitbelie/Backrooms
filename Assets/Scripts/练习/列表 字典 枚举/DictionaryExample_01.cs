using UnityEngine;
using System.Collections.Generic;

public class DictionaryExample_01 : MonoBehaviour
{
    //【字典变量】
    public Dictionary<string,float> 字典变量 = new Dictionary<string,float>();//使用前需要先初始化字典
    //Dictionary<string,float>  <索引类型,值类型>
    void Start()
    {
        //字典索引操作
        //【添加元素】
        字典变量.Add("字典索引1",1.1f);//添加元素1.1f(float)到字典索引1(string)
        字典变量.Add("字典索引2",2.2f);
        字典变量.Add("字典索引3",3.3f);
    }
    void Update()
    {
        //字典索引操作输出
        print(字典变量["字典索引1"]);//打印【字典索引1】对应的值(float)
    }

}

