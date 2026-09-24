using UnityEngine;

public class 平台设置 : MonoBehaviour
{
    [Header("强制平台")]
    public 平台工具.平台 强制平台 = 平台工具.平台.自动;

    void Awake()
    {
        应用();
    }

    void OnValidate()
    {
        应用();
    }

    void 应用()
    {
        平台工具.强制平台 = 强制平台;
    }
}