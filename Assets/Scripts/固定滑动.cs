using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 安卓控制器 + 鼠标调试支持
/// </summary>

public class 固定滑动 : MonoBehaviour
{
    public float 灵敏度设置;
    private float xo;
    public Transform 玩家模块;
    public bool 是否滑动;
    public Transform 上下滑动模块;
    public float 速度;
    public Touch ac;
    private int 获取的触摸点;
    public bool 是否触摸;
    public List<int> 禁用触摸点;

    [Header("鼠标调试设置")]
    public bool 启用鼠标控制 = true;
    public bool 锁定光标 = true;
    public float 鼠标灵敏度 = 0.2f;

    private void Start()
    {
        是否滑动 = true;
        是否触摸 = false;
        if (锁定光标) Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (是否滑动 == false)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Input.GetMouseButtonDown(0) && 锁定光标)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (启用鼠标控制 && Cursor.lockState == CursorLockMode.Locked)
        {
            float 鼠标X = Input.GetAxis("Mouse X") * 鼠标灵敏度;
            float 鼠标Y = Input.GetAxis("Mouse Y") * 鼠标灵敏度;
            if (Mathf.Abs(鼠标X) > 0.001f || Mathf.Abs(鼠标Y) > 0.001f)
            {
                float a = 鼠标X * -1 * 灵敏度设置;
                float b = 鼠标Y * -1 * 灵敏度设置;
                xo += b;
                xo = Mathf.Clamp(xo, -60, 60);
                上下滑动模块.localRotation = Quaternion.Euler(xo, 0, 0);
                玩家模块.transform.Rotate(Vector3.down, a);
            }
        }

        if (Input.touchCount > 0 && 是否触摸 == false)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                bool 是否计算 = true;
                if (禁用触摸点 != null)
                {
                    for (int l = 0; l < 禁用触摸点.Count; l++)
                    {
                        if (i == 禁用触摸点[l]) 是否计算 = false;
                    }
                }


                if ((Input.GetTouch(i).position.x >= (float)Screen.width / 2) && 是否计算)
                {
                    ac = Input.GetTouch(i);
                    获取的触摸点 = i;
                    是否触摸 = true;
                    break;
                }
            }
        }
        if (Input.touchCount > 0 && 是否触摸 == false)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Input.GetTouch(i).position.x < (float)Screen.width / 2)
                {
                    禁用触摸点.Clear();
                    禁用触摸点.Add(i);
                }
            }
        }
        if (Input.touchCount == 1 && 是否触摸 == true && 禁用触摸点.Count == 1)
        {
            禁用触摸点.Clear(); ac = Input.GetTouch(0); 获取的触摸点 = 0;
        }
        if (Input.touchCount == 0 && 是否触摸 == true) 是否触摸 = false;
        if (Input.touchCount == 0 && 禁用触摸点.Count != 0) 禁用触摸点.Clear();
        if (Input.touchCount == 2 && ((Input.GetTouch(0).position.x >= (float)Screen.width / 2) && (Input.GetTouch(1).position.x < (float)Screen.width / 2)) && 获取的触摸点 == 1)
        {
            禁用触摸点.Clear();
            禁用触摸点.Add(1);
            ac = Input.GetTouch(0); 获取的触摸点 = 0;
            是否触摸 = true;
        }
        else if (Input.touchCount == 2 && ((Input.GetTouch(0).position.x < (float)Screen.width / 2) && (Input.GetTouch(1).position.x >= (float)Screen.width / 2)) && 获取的触摸点 == 0)
        {
            禁用触摸点.Clear();
            禁用触摸点.Add(0);
            ac = Input.GetTouch(1); 获取的触摸点 = 1;
            是否触摸 = true;
        }
        if (是否触摸 == true)
        {
            ac = Input.GetTouch(获取的触摸点);
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).position != ac.position)
                {
                    if (Input.touchCount > 1)
                    {
                        判断取消();
                        设置滑动();
                    }
                }
                else
                {
                    判断取消();
                    设置滑动();
                }

            }
        }
    }

    void 判断取消()
    {
        switch (ac.phase)
        {
            case TouchPhase.Ended:
                是否触摸 = false;
                禁用触摸点.Clear();
                break;
        }
    }

    void 设置滑动()
    {
        Vector2 _DeltaPos = ac.deltaPosition;
        float a = _DeltaPos.x * -1 * 灵敏度设置 * Time.deltaTime;
        float b = _DeltaPos.y * -1 * 灵敏度设置 * Time.deltaTime;
        xo += b;
        xo = Mathf.Clamp(xo, -60, 60);
        上下滑动模块.localRotation = Quaternion.Euler(xo, 0, 0);
        玩家模块.transform.Rotate(Vector3.down, a);
    }
}