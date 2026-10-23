using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //通过编号加载场景
        SceneManager.LoadScene(0);
        //通过场景名称加载场景
        SceneManager.LoadScene("场景1");
        //异步加载场景
        SceneManager.LoadSceneAsync("场景2");
       

    
    }
     //声明一个用来接收【异步加载操作对象】的变量
    private AsyncOperation ao;
    //通过协程加载场景
    private IEnumerator LoadSceneCoroutine()
    {
        //赋值
        ao = SceneManager.LoadSceneAsync(1);
        //加载好场景不自动跳转
        ao.allowSceneActivation = false;
        while (ao.progress/*加载进度*/<0.9f)
        {
            //当加载进度小于90%时，协程挂起
            yield return null;
        }
        //加载完成后执行的代码
        Debug.Log("场景加载完成");
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
