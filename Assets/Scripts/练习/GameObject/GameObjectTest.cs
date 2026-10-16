using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectTest : MonoBehaviour
{   
    public GameObject K;
    // Start is called before the first frame update
    void Start()
    {
        //获取A游戏物体的组件
        if (K != null)
        {
            GameObject k = K;
            Debug.Log("自身游戏物体的层："+ k.tag);
        }
        //this可以省略，直接使用gameObject
        Debug.Log("自身游戏物体："+ this.gameObject);
        Debug.Log("自身游戏物体的名称："+ this.gameObject.name);
        Debug.Log("自身游戏物体的标签："+ this.gameObject.tag);
        
        Debug.Log("自身游戏物体的父游戏物体："+ this.gameObject.layer);
        Debug.Log("自身游戏物体的组件："+ this.gameObject.GetComponent<Collider>());

        Debug.Log("自身游戏物体的变换："+ this.gameObject.transform);
        //判断是否有父游戏物体
        if(gameObject.transform.parent != null)
        {
        Debug.Log("自身游戏物体的父游戏物体："+ this.gameObject.transform.parent);
        Debug.Log("自身游戏物体的父游戏物体的变换："+ this.gameObject.transform.parent.transform);
        Debug.Log("自身游戏物体的父游戏物体的父游戏物体："+ this.gameObject.transform.parent.transform.parent);
        Debug.Log("自身游戏物体的父游戏物体的父游戏物体的变换："+ this.gameObject.transform.parent.transform.parent.transform);
        }
        else
        {
            Debug.Log("自身游戏物体没有父游戏物体");
        }
        //关于游戏物体查找方法
        Debug.Log("通过名称查找："+ GameObject.Find("Key"));
        //找到后保存到变量
        GameObject key = GameObject.Find("Key");
        Debug.Log(key);
        //通过标签查找游戏物体
        Debug.Log("通过标签查找："+ GameObject.FindWithTag("Key"));
        GameObject key1 = GameObject.FindWithTag("Key");
        Debug.Log(key1);
        //通过组件查找被激活的游戏物体
        Debug.Log("通过组件查找："+ GameObject.FindObjectOfType<Transform>(true));
        Transform key3 = GameObject.FindObjectOfType<Transform>();
        Debug.Log(key3);
        //标签查找多数物体 FindGameObjects
        GameObject[] tags = GameObject.FindGameObjectsWithTag("Key");
        Debug.Log(tags);
        //组件查找多数物体 FindObjectsOfType
        Component[] components = FindObjectsOfType<Collider>();
        Debug.Log(components);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
