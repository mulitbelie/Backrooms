using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 柜子 : MonoBehaviour, IInteractable_物品//第一个是继承的类，逗号后面全部是接口
{
    public void Interact()// Interact ()功能  开柜子
    {
        Debug.Log("柜子：打开柜门");
        //柜门动画
    }
}

