using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class 分数显示UI : MonoBehaviour
{
    public TextMeshProUGUI 文本组件; // 分数显示文本组件

    void Update()
    {   
        // 检查GameManager实例是否存在
        if (GameManager.实例 != null && 文本组件 != null)
        {
            // 更新分数显示
            文本组件.text = $"分数：{GameManager.实例.当前分数}";
        }
    }
}