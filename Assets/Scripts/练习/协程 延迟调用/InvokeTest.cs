using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvokeTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //单次调用
        if (Input.GetKeyDown(KeyCode.A))
        {
            Invoke(nameof(Test), 1f);//1秒后 调用Test方法
        }
        //重复调用
        if (Input.GetKeyDown(KeyCode.S))
        {
            InvokeRepeating(nameof(Test), 2f, 1f);//2秒后 调用Test方法，后每1秒调用一次
        }
        //停止重复调用
        if (Input.GetKeyDown(KeyCode.D))
        {
            CancelInvoke(nameof(Test));//取消重复调用
            Debug.Log("停止重复调用");
        }
        //停止调用所有方法
        if (Input.GetKeyDown(KeyCode.F))
        {
            CancelInvoke();//取消所有调用
            Debug.Log("停止调用所有方法");
        }
        //判断有没有正在延迟调用的方法
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("是否有正在延迟调用的方法：" + IsInvoking(nameof(Test)));//判断【Test方法】是否正在延迟调用
            Debug.Log("是否有正在延迟调用的方法：" + IsInvoking());//判断有没有正在延迟调用的方法
        }
        
    }
    private void Test()
    {
        Debug.Log("测试");
    }
}
