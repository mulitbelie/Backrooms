using UnityEngine;

public class 区域BGM切换器 : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        全局音频.实例?.进入安全屋BGM();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        全局音频.实例?.离开安全屋BGM();
    }
}
