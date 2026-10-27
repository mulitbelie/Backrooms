using UnityEngine;
using UnityEngine.UI;

public class PageDotController : MonoBehaviour
{
    [Header("小点数组，把DotGroup下的所有子对象都赋值给数组中的元素")]
    public GameObject[] dots;
    [Header("选中颜色/默认颜色")]
    public Color activeColor;
    public Color normalColor;
    [Header("关联的Scroll脚本")]
    public LevelScrollRect scrollScript;

    void Start()
    {
        // 先判空，防止没赋值脚本直接报错
        if (scrollScript != null)
        {
            scrollScript.OnPageChanged += UpdateDot;
            UpdateDot(0);
        }
        else
        {
            Debug.LogWarning("没有绑定 LevelScrollRect！", this);
        }
        // 自动给所有Toggle绑定点击跳转
        for(int i = 0; i<dots.Length; i++)
        {
            int idx = i;
            Toggle toggle = dots[i].GetComponentInChildren<Toggle>();
            if(toggle != null)
            {
                toggle.onValueChanged.AddListener((bool isOn) =>
                {
                    if(isOn && scrollScript != null)
                    {
                        scrollScript.MoveToPage(idx);
                    }
                });
            }
        }
    }

    void UpdateDot(int currentIndex)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            GameObject itemObj = dots[i];
            if(itemObj == null)continue;
            // 1. 获取身上的Image，设置颜色
            Image dotImg = dots[i].GetComponentInChildren<Image>();
            if (dotImg != null)
            {
                dotImg.color = i == currentIndex ? activeColor : normalColor;
            }
            else
            {
                Debug.LogWarning($"索引{i}的小点物体下没有找到Image！", dots[i]);
            }
            // 2. 获取身上的Toggle，设置isOn，控制叶子leaf
            Toggle toggle = itemObj.GetComponentInChildren<Toggle>();
            if (toggle != null)
            {
                toggle.isOn = (i == currentIndex);
            }
            else
            {
                Debug.LogWarning($"索引{i} Item上没有Toggle组件", itemObj);
            }
        }
    }

}
