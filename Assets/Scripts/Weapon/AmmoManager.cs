using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static AmmoManager Instance {get; private set; }

    //UI
    public TextMeshProUGUI ammoDisplay;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else{
            Instance = this;
            transform.SetParent(null);//在调用前加 transform.SetParent(null)，强制把物体从父级上摘下来变成根节点
            DontDestroyOnLoad(gameObject);//有个硬性要求——这个物体必须是层级里的根节点（直接挂在 Scene 下，没有父物体）。
        }
    }
}