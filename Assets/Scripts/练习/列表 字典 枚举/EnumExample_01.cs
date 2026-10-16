using UnityEngine;
using System;

public class EnumExample_01 : MonoBehaviour
{
    //【不使用枚举】
    //1.火 2.水 3.风
    public int elementTypeId;//元素类型ID 需要手动输入值123

    void Start()
    {
        switch(elementTypeId)
        {
            case 1:
                Debug.Log("火");
                break;
            case 2:
                Debug.Log("水");
                break;
            case 3:
                Debug.Log("风");
                break;
        }
    }
}