using System;
using UnityEngine;

public class 门铃广播者 : MonoBehaviour
{
    public string 访客名字 = "快递员";

    public event Action<string> 门铃响了;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"[门铃] {访客名字} 来了，正在按门铃...");//
            门铃响了?.Invoke(访客名字);
            //  ↑
            //  "门铃响了" 这个名单有没有东西？
            //    有 → 继续往下跑 Invoke
            //    没有(null) → 停止，什么都不做
        }
    }
}