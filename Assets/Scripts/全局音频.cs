using UnityEngine;

public class 全局音频 : MonoBehaviour
{
    public static 全局音频 实例 { get; private set; }

    [Header("BGM（真循环，场景氛围）")]
    public AudioClip BGM音频;
    [Range(0f, 1f)] public float BGM音量 = 0.3f;

    [Header("环境音（真循环，背景杂音）")]
    public AudioClip 环境音音频;
    [Range(0f, 1f)] public float 环境音音量 = 0.15f;

    [Header("开枪 SFX（枪声、撞击等，叠加播放）")]
    public AudioClip 手枪开火;
    [Range(0f, 1f)] public float 开火音量 = 0.15f;

    [Header("换弹夹 （枪声、撞击等，叠加播放）")]
    public AudioClip 手枪换弹夹;
    [Range(0f, 1f)] public float 手枪换弹夹音量 = 0.15f;

    [Header("空弹夹 （枪声、撞击等，叠加播放）")]
    public AudioClip 手枪空弹夹;
    [Range(0f, 1f)] public float 手枪空弹夹音量 = 0.15f;

    private AudioSource BGM源;
    private AudioSource 环境音源;
    private AudioSource 手枪开火音源;//内部调用
    private AudioSource 手枪换弹夹音源;//内部调用
    private AudioSource 手枪空弹夹音源;//内部调用

    void Awake()
    {
        if (实例 != null && 实例 != this)
        {
            Destroy(gameObject);
            return;
        }
        实例 = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        初始化音源();
        if (BGM音频 != null) 切换BGM(BGM音频);
        if (环境音音频 != null) 切换环境音(环境音音频);
    }

    void 初始化音源()
    {
        BGM源 = gameObject.AddComponent<AudioSource>();
        BGM源.loop = true;
        BGM源.playOnAwake = false;
        BGM源.volume = BGM音量;
        BGM源.spatialBlend = 0;
        BGM源.priority = 128;

        环境音源 = gameObject.AddComponent<AudioSource>();
        环境音源.loop = true;
        环境音源.playOnAwake = false;
        环境音源.volume = 环境音音量;
        环境音源.spatialBlend = 0;
        环境音源.priority = 128;

        手枪开火音源 = gameObject.AddComponent<AudioSource>();//添加音源组件
        手枪开火音源.playOnAwake = false;
        手枪开火音源.spatialBlend = 0;
        手枪开火音源.priority = 128;

        手枪换弹夹音源 = gameObject.AddComponent<AudioSource>();//添加音源组件
        手枪换弹夹音源.playOnAwake = false;
        手枪换弹夹音源.spatialBlend = 0;
        手枪换弹夹音源.priority = 128;

        手枪空弹夹音源 = gameObject.AddComponent<AudioSource>();//添加音源组件
        手枪空弹夹音源.playOnAwake = false;
        手枪空弹夹音源.spatialBlend = 0;
        手枪空弹夹音源.priority = 128;        
    }

    public void 切换BGM(AudioClip clip)
    {
        if (clip == null) return;
        if (BGM源 == null) 初始化音源();
        BGM源.Stop();
        BGM源.clip = clip;
        BGM源.Play();
    }

    public void 切换环境音(AudioClip clip)
    {
        if (clip == null) return;
        if (环境音源 == null) 初始化音源();
        环境音源.Stop();
        环境音源.clip = clip;
        环境音源.Play();
    }

    public void 播放手枪开火()//播放手枪开火播放方法 ，支持叠加播放方便Weapon调用
    {
        if (手枪开火音源 != null && 手枪开火 != null)//如果没有条件 手枪开火音源 为 null时候 → 直接崩
            手枪开火音源.PlayOneShot(手枪开火, 开火音量);//播放方式PlayOneShot(音频片段  音量) 播放一次 
    }

    public void 播放手枪换弹夹()//播放手枪换弹夹播放方法 ，支持叠加播放方便Weapon调用
    {
        if (手枪换弹夹音源 != null && 手枪换弹夹 != null)//如果没有条件 手枪开火音源 为 null时候 → 直接崩
            手枪换弹夹音源.PlayOneShot(手枪换弹夹, 手枪换弹夹音量);//播放方式PlayOneShot(音频片段  音量) 播放一次 
    }

    public void 播放手枪空弹夹()//播放手枪开火播放方法 ，支持叠加播放方便Weapon调用
    {
        if (手枪空弹夹音源 != null && 手枪空弹夹 != null)//如果没有条件 手枪开火音源 为 null时候 → 直接崩
            手枪空弹夹音源.PlayOneShot(手枪空弹夹, 手枪空弹夹音量);//播放方式PlayOneShot(音频片段  音量) 播放一次 
    }

    public void 停BGM() { if (BGM源 != null) BGM源.Stop(); }
    public void 停环境音() { if (环境音源 != null) 环境音源.Stop(); }
    public void 停全部() { 停BGM(); 停环境音(); }

    public void 设BGM音量(float v) { if (BGM源 != null) BGM源.volume = Mathf.Clamp01(v); }
    public void 设环境音音量(float v) { if (环境音源 != null) 环境音源.volume = Mathf.Clamp01(v); }
}