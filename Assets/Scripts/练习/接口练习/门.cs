using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 门 : MonoBehaviour, IInteractable_物品//第一个是继承的类，逗号后面全部是接口
{
    private bool isOpen;
    // 遵守接口，必须写这个函数
    public void Interact()// Interact ()功能 开门
    {
        isOpen = !isOpen;
        Debug.Log("门：开关门");
        // 开门/关门动画
    }
}