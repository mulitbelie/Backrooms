using UnityEngine;

public class 加分按钮 : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))// 按下1键加分10分
        {
            GameManager.实例.加分(10);//直接通过GameManager实例调用加分方法
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))// 按下2键加分50分
        {
            GameManager.实例.加分(50);
        }

        if (Input.GetKeyDown(KeyCode.R))// 按下R键重置分数
        {
            GameManager.实例.重置分数();//直接通过GameManager实例调用重置分数方法
        }
    }
}