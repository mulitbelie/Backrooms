using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 与玩家绑定 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        绑定玩家();
    }

    void 绑定玩家()
    {
        GameObject.FindGameObjectWithTag("全局脚本").GetComponent<全局脚本>().请求绑定玩家(gameObject);
    }
}
