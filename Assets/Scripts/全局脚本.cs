using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 全局脚本 : MonoBehaviour
{
    static 全局脚本 instance;
    public GameObject 玩家;
    public Transform 出生点;
    public 玩家画布 玩家画布;
    
    private void Awake(){
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;


    }
    // Start is called before the first frame update
    void Start()
    {
        绑定画布();
        生成玩家();
    }

    void 绑定画布(){
        玩家画布 = GameObject.Find("玩家画布").GetComponent<玩家画布>().与画布绑定(this);

    }
    void 生成玩家(){
        Instantiate(玩家, new Vector3(出生点.position.x,出生点.position.y+1, 出生点.position.z), Quaternion.identity);

    }

    public void 请求绑定玩家(GameObject 生成的玩家){

        玩家 = 生成的玩家;
        生成的玩家.GetComponent<基础移动控制>().摇杆配置=玩家画布.摇杆;
        玩家画布.初始化玩家引用();
    }

    // Update is called once per frame


    void Update()
    {
        
    }
}