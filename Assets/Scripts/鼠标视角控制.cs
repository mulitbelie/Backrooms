using UnityEngine;

public class 鼠标视角控制 : MonoBehaviour
{
    public Transform 玩家模块;
    public Transform 上下滑动模块;

    [Header("水平旋转（鼠标移动 → 度）")]
    public float 水平灵敏度 = 10f;
    [Header("垂直旋转（鼠标移动 → 度）")]
    public float 垂直灵敏度 = 8f;

    public bool 锁定光标 = true;

    private float 垂直旋转;

    void Start()
    {
        if (锁定光标) Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Input.GetMouseButtonDown(0) && 锁定光标)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (Cursor.lockState != CursorLockMode.Locked) return;

        float 鼠标X = Input.GetAxis("Mouse X");
        float 鼠标Y = Input.GetAxis("Mouse Y");
        if (Mathf.Abs(鼠标X) < 0.001f && Mathf.Abs(鼠标Y) < 0.001f) return;

        float 水平增量 = 鼠标X * 水平灵敏度;
        float 垂直增量 = -鼠标Y * 垂直灵敏度;

        垂直旋转 += 垂直增量;
        垂直旋转 = Mathf.Clamp(垂直旋转, -60f, 60f);

        if (上下滑动模块 != null)
            上下滑动模块.localRotation = Quaternion.Euler(垂直旋转, 0f, 0f);

        if (玩家模块 != null)
            玩家模块.Rotate(Vector3.up, 水平增量);
    }
}