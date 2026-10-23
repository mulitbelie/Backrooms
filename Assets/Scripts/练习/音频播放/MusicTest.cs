using UnityEngine;

public class MusicTest : MonoBehaviour
{
    //音频源组件
    private AudioSource audioSource;
    //音频文件
    public AudioClip audioClip,soundClip;
    // Start is called before the first frame update
    void Start()
    {
        //获取音频源组件
        audioSource = GetComponent<AudioSource>();
        //将音频文件赋值给音频源组件
        audioSource.clip = soundClip;
        //循环播放音频
        audioSource.loop = true;
        //播放音频
        audioSource.Play();
        //播放音频文件
        // PlayOneShot写法（点击音效）
        audioSource.PlayOneShot(audioClip, 0.8f);
        //设置音量
        audioSource.volume = 0.5f;//音量范围0-1 0表示静音 1表示最大音量


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //暂停音频
            audioSource.Pause();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //继续播放音频
            audioSource.Play();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //停止播放音频
            audioSource.Stop();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            //播放音频文件
            audioSource.PlayOneShot(audioClip);
        }

        
    }
}
