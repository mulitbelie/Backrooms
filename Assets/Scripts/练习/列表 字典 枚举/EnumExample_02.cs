using UnityEngine;
using System;

public class EnumExample_02 : MonoBehaviour
{
    //【枚举】
    public enum elementType{火,水,风};
    //1.火 2.水 3.风
    //elementType（枚举类型）`myElementType` 是变量
    public elementType myElementType;

    void Start()
    {
        switch(myElementType)// switch( 变量 )看【选择框】现在选了什么
        {
            case elementType.火:// case 枚举类型.枚举值:  如果选择框 == 菜单里的“火”
                Debug.Log("火");
                break;
            case elementType.水:// 如果选择框 == 菜单里的“水”
                Debug.Log("水");
                break;
            case elementType.风:
                Debug.Log("风");
                break;
        }
    }
}