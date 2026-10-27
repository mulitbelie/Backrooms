using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ToggleTextColor : MonoBehaviour
{
    private Toggle toggle;
    private TextMeshProUGUI tmpText;

    public Color selectedColor = Color.white;//选中时文字的颜色
    public Color unselectedColor = Color.grey;//未选中时文字的颜色

    void Awake()
    {
        toggle = GetComponent<Toggle>();
        tmpText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        // 初始化文字颜色
        // `toggle.isOn` 就是 **Toggle 组件原生自带的公开属性**
        SetTextColor(toggle.isOn);
        // 启动自动绑定监听，不用手动在面板拖事件
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }
    // toggle切换事件处理
    void OnToggleChanged(bool isOn)//isOn为true时，调用SetTextColor方法
    {
        SetTextColor(isOn);
    }
    // 设置文字颜色
    void SetTextColor(bool isOn)//isOn为true时，选中时文字的颜色，否则未选中时文字的颜色
    {
        if(tmpText == null)return;
        tmpText.color = isOn ? selectedColor : unselectedColor;
    }
}
