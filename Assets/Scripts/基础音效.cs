using UnityEngine;

public class 基础音效 : MonoBehaviour
{
    private 角色状态管理 状态管理;
    private 基础移动控制 移动控制;

    [Header("音频源")]
    public AudioSource 步态音效源;
    public AudioSource 单次音效源;

    [Header("步态音效")]
    public AudioClip 走路脚步声;
    public AudioClip 跑步脚步声;
    public AudioClip 下蹲脚步声;

    [Header("动作音效")]
    public AudioClip 跳跃音效;
    public AudioClip 落地音效;

    [Header("步态间隔（秒）")]
    public float 走路步间隔 = 0.5f;
    public float 跑步步间隔 = 0.3f;
    public float 下蹲步间隔 = 0.7f;

    [Header("音量")]
    [Range(0f, 1f)] public float 走路音量 = 0.6f;
    [Range(0f, 1f)] public float 跑步音量 = 0.8f;
    [Range(0f, 1f)] public float 下蹲音量 = 0.4f;
    [Range(0f, 1f)] public float 跳跃音量 = 0.7f;
    [Range(0f, 1f)] public float 落地音量 = 0.5f;

    private float 步计时器;
    private 角色状态管理.角色状态 上一帧步态状态;

    void Start()
    {
        状态管理 = GetComponent<角色状态管理>();
        移动控制 = GetComponent<基础移动控制>();
        if (步态音效源 == null)
            步态音效源 = gameObject.AddComponent<AudioSource>();
        if (单次音效源 == null)
            单次音效源 = gameObject.AddComponent<AudioSource>();

        步态音效源.loop = false;
        步态音效源.playOnAwake = false;
        单次音效源.playOnAwake = false;

        上一帧步态状态 = 状态管理.当前状态;
    }

    void Update()
    {
        检测动作音效();
        播放步态音();
    }

    void 检测动作音效()
    {
        if (状态管理.刚进入(角色状态管理.角色状态.跳跃) && 跳跃音效 != null)
        {
            单次音效源.PlayOneShot(跳跃音效, 跳跃音量);
        }

        if (状态管理.刚离开(角色状态管理.角色状态.跳跃) && 落地音效 != null)
        {
            单次音效源.PlayOneShot(落地音效, 落地音量);
        }
    }

    void 播放步态音()
    {
        bool 摇杆在用 = 移动控制 != null && 移动控制.检测角色拖动摇杆();
        bool 在地面 = 移动控制 != null && 移动控制.角色地面检测;

        if (!摇杆在用 || !在地面)
        {
            步态音效源.Stop();
            步计时器 = 0;
            上一帧步态状态 = 状态管理.当前状态;
            return;
        }

        角色状态管理.角色状态 当前状态 = 状态管理.当前状态;

        bool 状态变了 = 当前状态 != 上一帧步态状态;
        if (状态变了)
        {
            步态音效源.Stop();
            步计时器 = 0;
            上一帧步态状态 = 当前状态;
        }

        AudioClip 要播的clip = null;
        float 音量 = 0f;
        float 间隔 = 0.5f;

        switch (当前状态)
        {
            case 角色状态管理.角色状态.走路:
                要播的clip = 走路脚步声;
                音量 = 走路音量;
                间隔 = 走路步间隔;
                break;
            case 角色状态管理.角色状态.跑步:
                要播的clip = 跑步脚步声;
                音量 = 跑步音量;
                间隔 = 跑步步间隔;
                break;
            case 角色状态管理.角色状态.下蹲走路:
                要播的clip = 下蹲脚步声;
                音量 = 下蹲音量;
                间隔 = 下蹲步间隔;
                break;
            default:
                步态音效源.Stop();
                步计时器 = 0;
                return;
        }

        if (要播的clip == null) return;

        步计时器 += Time.deltaTime;
        if (步计时器 >= 间隔)
        {
            步计时器 = 0;
            步态音效源.Stop();
            步态音效源.clip = 要播的clip;
            步态音效源.volume = 音量;
            步态音效源.Play();
        }
    }
}