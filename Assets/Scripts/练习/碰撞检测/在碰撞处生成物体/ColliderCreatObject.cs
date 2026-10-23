using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCreatObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        //获取最近碰撞点 this.transform.position （自己物体中心点）最近的点
        Vector3 contactPoint = other.GetComponent<Collider>().ClosestPoint(this.transform.position);
        //生成空物体
        GameObject summonGameObject = new GameObject();
        //设置空物体位置为碰撞点
        summonGameObject.transform.position = contactPoint;
        Debug.Log("生成了新物体");
    }
    private void OnCollisionEnter(Collision collision)
    {
        Vector3 contactPoint = collision.collider.ClosestPoint (this.transform.position);
        Debug.Log("碰撞了");
    }

}