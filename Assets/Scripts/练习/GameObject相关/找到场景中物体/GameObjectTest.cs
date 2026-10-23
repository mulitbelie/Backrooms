using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectTest : MonoBehaviour
{   
    public GameObject K;
    public GameObject cube;
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

        //创建游戏物体
        //1.场景中创建一个空游戏物体
        GameObject gameObject1 = new GameObject("新游戏物体");

        //2.通过场景里现有的游戏物体，游戏资源来克隆
        GameObject gameObject2 = Instantiate(gameObject1.gameObject);
        //场景中生成指定的预制体
        GameObject gameObject3 = Instantiate(cube.gameObject);

        //3.创建原始几何物体
        GameObject gameObject4 = GameObject.CreatePrimitive(PrimitiveType.Cube);

        //销毁游戏物体
        //1.直接销毁
        Destroy(gameObject1);
        //2.延迟销毁 x秒后销毁
        Destroy(gameObject2, 2f);
        //3.销毁指定游戏物体的子游戏物体
        Destroy(gameObject3.transform.Find("Cube").gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
