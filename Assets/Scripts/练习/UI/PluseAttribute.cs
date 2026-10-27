using UnityEngine;
using TMPro;    

public class PluseAttribute : MonoBehaviour
{
    public TextMeshProUGUI tmpText;

    void Start()
    {
        var numObj = transform.Find("Num");
        if(numObj != null)
        {
            tmpText = numObj.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError("Num对象不存在");
        }
    }
    public void AddOne()
    {
        string str = tmpText.text;
        // 尝试将字符串转换为整数
        if(int.TryParse(str, out int num))
        {
            // 如果转换成功+1
            num+=1;

        }
        else
        {
            // 如果转换失败，将num设为1
            num = 1;
        }
        // 将整数转换为字符串并赋值给tmpText
        tmpText.text=num.ToString();
        
    }

}
