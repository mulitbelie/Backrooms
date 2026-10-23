using UnityEngine;

public class 区域音效触发器 : MonoBehaviour
{
    [Header("区域音效")]
    public AudioClip 音效;
    [Range(0f, 1f)] public float 音量 = 0.5f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if(全局音频.实例==null || 音效==null) return;

        全局音频.实例.播放一次(音效, 音量);
        Destroy(gameObject);
    }
}