using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door : MonoBehaviour
{
    private DoorController dc;

    void Update()//更新
    {
        ToggleDoor();//玩家开关门
    }
    private void OnTriggerEnter(Collider other) //other门对象
    {

        if (other.CompareTag("Door"))
        {
            dc = other.GetComponent<DoorController>();//获取门的组件DoorController (切换门状态 (打开/关闭))
        }
       
    }
    //退出门的范围
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            dc = null;//滞空 将dc设置为null
        }
    }
        //玩家开关门
    private void ToggleDoor()
    {
        if (Input.GetKeyDown(KeyCode.E) && dc != null)
        {
            dc.ToogleDoor();//切换门状态 (打开/关闭)
        }
    }       
}