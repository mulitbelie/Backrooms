using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;//鼠标锁定模式Confined在屏幕中心
        Cursor.lockState = CursorLockMode.Locked;//鼠标锁定模式Locked在屏幕中心并隐藏
        Cursor.lockState = CursorLockMode.None;//鼠标锁定模式None不锁定鼠标
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
