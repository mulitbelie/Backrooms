using UnityEngine;
using UnityEngine.UI;

public class MyToggle : MonoBehaviour
{
    public GameObject onGameObject;
    public GameObject offGameObject;
    private Toggle toggle;
    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        OnValueChanged(toggle.isOn);
    }
    //传入isOn=true/false ,onGameObject.SetActive(true)下面取反!isOn offGameObject.SetActive(false)
    public void OnValueChanged(bool isOn)
    {
        // 开关勾选（`isOn = true`）→ `onGameObject` 显示，`offGameObject` 显示取反也就是隐藏
        onGameObject.SetActive(isOn);
        // 开关取消勾选（`isOn = false`）→ `onGameObject` 隐藏，`offGameObject` 显示
        offGameObject.SetActive(!isOn);
    }
}
