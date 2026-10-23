using UnityEngine;

public class ResourceTest : MonoBehaviour
{
    public GameObject cube;

    // Start is called before the first frame update
    void Start()
    {
        //必须有Resource文件夹
        //加载资源必须在Resource文件夹下
        //`Resources.Load<T>()`：**把硬盘上的预制体资源读到内存里（拿到模板）**
        //`Instantiate()`：**拿着这个模板，在场景里生成一个真实物体（克隆一份出来）**
        cube = Instantiate(Resources.Load<GameObject>("Cube"));
        if(cube == null)
        {
            Debug.LogError("加载预制体失败");
            return;
        }
        Debug.Log(cube);
        cube.transform.position = new Vector3(1,1,1);
        cube.transform.rotation = Random.rotation;
        //cube.transform.eulerAngles = new Vector3(Random.Range(0,360),Random.Range(0,360),Random.Range(0,360));
        cube.transform.localScale = new Vector3(Random.Range(0.5f,2),Random.Range(0.5f,2),Random.Range(0.5f,2));
        

        //加载音频素材
        AudioClip sound = Resources.Load<AudioClip>("BGM");
        if(sound == null)
        {
            Debug.LogError("加载音频素材失败");
            return;
        }
        // 拿到AudioSource组件（播放器）
        AudioSource audioSource = GetComponent<AudioSource>();
        // 把素材交给播放器
        audioSource.clip = sound;
        // 播放器播放
        audioSource.Play();

    }
    void Update()
    {
        //让物体沿着世界空间y轴旋转
        cube.transform.Rotate(Vector3.up, 10*Time.deltaTime,Space.World);
    }


}
