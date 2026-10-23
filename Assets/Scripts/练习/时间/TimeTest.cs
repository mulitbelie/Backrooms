using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //时间流逝的标度
        Debug.Log("时间流逝的标度"+Time.timeScale);
        Time.timeScale = 0.5f;//速度 默认为1，大于1表示时间加速，小于1表示时间减速
        Debug.Log("时间流逝的标度"+Time.timeScale);

    }
    void Update()
    {
        //完成上一帧所用的时间如果 1秒100帧 1帧0.01秒
        Debug.Log("完成上一帧所用的时间"+ Time.deltaTime);
        Debug.Log("自游戏启动以来的时间(加载完成后执行物理或其它固定帧率更新的时间间隔)"+Time.fixedTime);
        Debug.Log("自游戏启动以来的时间(加载完成后执行Update方法的时间)"+Time.time);
        Debug.Log("自游戏启动以来的实际时间(包含启动加载时间)"+Time.realtimeSinceStartup);
        Debug.Log("经过平滑处理的Time.deltaTime"+Time.smoothDeltaTime);
        Debug.Log("自场景以来的时间"+Time.timeSinceLevelLoad);
    }
    void FixedUpdate()
    {
        //固定时间步长 
        //用于物理模拟
        Debug.Log("执行物理或其它固定帧率更新的时间间隔"+ Time.fixedDeltaTime);
    }



}
