using UnityEngine;

public class 狗狗监听者 : MonoBehaviour
{
    public 门铃广播者 门铃;

    void Start()
    {
        if (门铃 != null)
        {
            门铃.门铃响了 += 狗狗叫;
        }
    }

    void OnDestroy()
    {
        if (门铃 != null)
        {
            门铃.门铃响了 -= 狗狗叫;
        }
    }

    void 狗狗叫(string 访客)
    {
        Debug.Log($"[狗狗] 汪！汪！有{访客}！");
    }
}