using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 全局脚本 : MonoBehaviour
{
    public GameObject 玩家预制体;
    public Transform 出生点;
    public 玩家画布 玩家画布;
    
    public static 全局脚本 Instance { get; private set; }//全局脚本的单例实例，用于在其他脚本中调用

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }//如果已经存在一个实例，就销毁当前实例
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public GameObject 玩家 { get; private set; }

    void Start()
    {
        //玩家画布 = GameObject.Find("玩家画布").GetComponent<玩家画布>();
        玩家 = Instantiate(玩家预制体, new Vector3(出生点.position.x, 出生点.position.y + 1, 出生点.position.z), Quaternion.identity);//实例化玩家预制体，设置出生点位置，旋转角度为 0

        var 移动控制 = 玩家.GetComponentInChildren<基础移动控制>();
        if (移动控制 != null)
        {
            移动控制.摇杆配置 = 玩家画布.摇杆;
        }
        else
        {
            Debug.LogError("[全局脚本] 没找到 基础移动控制，摇杆配置失败");
        }

        玩家画布.初始化(this);
    }
}