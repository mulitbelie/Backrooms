using System;
using UnityEngine;

public class RayCastTest : MonoBehaviour
{
    public LayerMask layer;
    //获取相机方法二：定义相机槽位
    public Camera cam;
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            // 获取相机方法一：从tag为MainCamera的相机屏幕坐标转换为射线坐标
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // 获取相机方法二：从自定义相机屏幕坐标转换为射线坐标
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            // **它是一个 “信息盒子”，用来存这次检测命中之后返回的全部碰撞信息**。
            RaycastHit hit;
            //Raycast(射线, 输出碰撞信息, 最大距离, layer层)
            // if(Physics.Raycast(ray,out hit, Mathf.Infinity,layer))
            // {  
            //     Debug.Log("射到物体"+ hit.transform.gameObject);
            //     Debug.Log("射到物体名称"+ hit.transform.gameObject.name);
            //     Debug.Log("射到的点"+ hit.point);
            //     Debug.Log("射到的点的transform"+ hit.transform.position);
            // }

            
            //球形射线检测：检测射线是否与球体碰撞
            //SphereCast(射线, 球半径, 输出碰撞信息, 最大距离, layer层掩码)
            if(Physics.SphereCast(ray, 1f, out hit ,Mathf.Infinity,layer))
            {
                Debug.Log("射到物体名称"+ hit.transform.gameObject.name);
                if(hit.transform != null)
                {
                    //隐藏物体
                    hit.transform.gameObject.SetActive(false);
                }
                
            }
        }
        
    }
}
