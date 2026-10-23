using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClickToMove : MonoBehaviour
{
    //声明一个用来接收【导航代理组件】的变量
    private NavMeshAgent navMeshAgent;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //获取鼠标点击
        if (Input.GetMouseButtonDown(0))
        {
            //声明一个用来接收【相机射线】的变量
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //声明一个用来接收【射线与导航网格相交的信息】的变量
            RaycastHit hit;
            //如果射线与导航网格相交
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, NavMesh.AllAreas))
            {
                //设置目标位置
                navMeshAgent.SetDestination(hit.point);
            }
           
           
        }
    }
}
