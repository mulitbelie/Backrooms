using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coroutione : MonoBehaviour
{

    void Start()
    {
        float fps = 1.0f / Time.deltaTime;
        Debug.Log("当前FPS："+fps);
        //调用协程(方法1)
        StartCoroutine(nameof(coroutine));
        StartCoroutine("coroutine");
        //调用协程(方法2)
        StartCoroutine(coroutine());

        //调用协程2传参
        StartCoroutine(coroutine2(100));
    }

    // Update is called once per frame
    void Update()
    {
        //调用协程3 循环
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(coroutine3());
        }
        
    }
    //协程1 基础
    IEnumerator coroutine()
    {
        // 等待 1 秒（以秒为单位）
        yield return new WaitForSeconds(1f);
        Debug.Log("等待 1 秒");
        // 等待 1 帧
        yield return null;
        // 等待 1 帧 后面加任何值都可以，都是等待 1 帧
        Debug.Log("等待 1 帧");
        yield return 0;
        //等待到当前帧结束
        yield return new WaitForEndOfFrame();
        Debug.Log("等待到当前帧结束");
        

    }
        //协程2传参
    IEnumerator coroutine2(int a)
    {
        yield return null;
        Debug.Log(a);
    }
    //协程3 循环  
    IEnumerator coroutine3()
    {
        while (true)
        {
            yield return null;
            Debug.Log("循环");
            if (Input.GetKeyDown(KeyCode.B))
            {
                Debug.Log("退出协程3");
                yield break;//break 退出while yield break 退出协程
            }

        }
    }
}
