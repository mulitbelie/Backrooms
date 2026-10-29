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
        // currentIndex：当前停在哪一页，比如滑到第2页 currentIndex=2
        for (int i = 0; i < dots.Length; i++)
        {
            // i 是指示器的序号：0、1、2... 依次遍历每一个点
            GameObject itemObj = dots[i];
            if(itemObj == null)continue;// 如果这个点是空的，直接跳过，不往下跑
            // 【1】找到小点物体上的Toggle组件
            Toggle toggle = itemObj.GetComponentInChildren<Toggle>();
            if (toggle != null)
            {
                // 如果当前页码=这个点的序号 → isOn=true（选中），否则false
                toggle.isOn = (i == currentIndex);
                // ⚠️ 这一句：把Toggle的过渡效果关掉，否则会有一个过渡动画
                toggle.transition = Selectable.Transition.None;
            }
            else
            {
                Debug.LogWarning($"索引{i} Item上没有Toggle组件", itemObj);
            }
            // 【2】找这个点下面的Image（圆点本体）
            Image dotImg = dots[i].GetComponentInChildren<Image>();
            if (dotImg != null)
            {
                
                //`i == currentIndex`：判断：这个指示器序号，是不是当前页面序号
                // 如果为真：dotImg.color = activeColor；否则：normalColor
                dotImg.color = i == currentIndex ? activeColor : normalColor;
            }
            else
            {
                Debug.LogWarning($"索引{i}的小点物体下没有找到Image！", dots[i]);
            }
        }
    }

}