using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathfTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("正无穷大"+Mathf.Infinity);
        Debug.Log("负无穷大"+Mathf.NegativeInfinity);
        Debug.Log("圆周率"+Mathf.PI);

        //方法
        Debug.Log("绝对值"+Mathf.Abs(-10f));
        Debug.Log("取整"+Mathf.Round(10.5f));
        Debug.Log("向下取整"+Mathf.FloorToInt(10.5f));
        Debug.Log("向上取整"+Mathf.CeilToInt(10.5f));
        Debug.Log("随机数"+Random.Range(0,10));
        Debug.Log("以度为单位的正弦"+Mathf.Sin(45f));
        Debug.Log("以度为单位的余弦"+Mathf.Cos(45f));
        Debug.Log("线性插值"+Mathf.Lerp(0,1,0.5f));
        Debug.Log("向目标值移动"+Mathf.MoveTowards(0,1,0.5f));
        Debug.Log("平滑移动"+Mathf.SmoothStep(0,1,0.5f));

        
        int a = -1;
        //钳制：将一个值限制在指定的范围内0-10之间
        a = Mathf.Clamp(a,0,10);
        Debug.Log("钳制"+a);

        
    }

}
