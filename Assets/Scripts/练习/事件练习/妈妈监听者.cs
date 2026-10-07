using UnityEngine;

public class 妈妈监听者 : MonoBehaviour
{
    public 门铃广播者 门铃;//（class类名  你自己起的） 

    void Start()
    {
        查找门铃广播者();// 自动找场景里的那个门铃广播者组件

        if (门铃 != null)
        {
            门铃.门铃响了 += 妈妈开门;//订阅监听
            Debug.Log("[妈妈] 我订阅了门铃，听到响就去开门");
        }
    }

    void 查找门铃广播者(){
        门铃 = FindObjectOfType<门铃广播者>();// 自动找场景里的那个门铃广播者组件
    }

    void OnDestroy()
    {
        if (门铃 != null)
        {
            门铃.门铃响了 -= 妈妈开门;//取消订阅（防泄漏）
        }
    }

    void 妈妈开门(string 访客)//【访客】就是门铃广播者中的【访客名字】第1个参数，可以自己定义
    {
        Debug.Log($"[妈妈] 听到门铃了！来啦来啦~ 开门：{访客}");
    }
}