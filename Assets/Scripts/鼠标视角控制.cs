using UnityEngine;

public class 鼠标视角控制 : MonoBehaviour
{
    public Transform 玩家模块;
    public Transform 上下滑动模块;

    [Header("灵敏度")]
    public float 水平灵敏度 = 10f;
    public float 垂直灵敏度 = 8f;

    [Header("衣柜中限制（WardrobeController 自动设置）")]
    [Range(0f, 180f)] public float 衣柜水平半角 = 60f;
    [Range(0f, 90f)]  public float 衣柜垂直半角 = 60f;

    public bool 锁定光标 = true;

    private float 水平旋转;
    private float 垂直旋转;
    private float 水平半角 = float.MaxValue;
    private float 垂直半角 = float.MaxValue;

    public void 收紧为衣柜(Transform 根节点)
    {
        水平半角 = 衣柜水平半角;
        垂直半角 = 衣柜垂直半角;
        重置旋转计数(根节点);
    }

    public void 恢复默认(Transform 根节点)
    {
        水平半角 = float.MaxValue;
        垂直半角 = float.MaxValue;
        重置旋转计数(根节点);
    }

    public void 重置旋转计数(Transform 根节点)
    {
        水平旋转 = 0f;
        垂直旋转 = 0f;
        if (玩家模块 != null && 玩家模块 != 根节点)
            玩家模块.localRotation = Quaternion.identity;
        if (上下滑动模块 != null && 上下滑动模块 != 根节点)
            上下滑动模块.localRotation = Quaternion.identity;
    }

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

        float 实际水平增量 = 截断增量(水平旋转, 水平增量, 水平半角);
        float 实际垂直增量 = 截断增量(垂直旋转, 垂直增量, 垂直半角);

        水平旋转 += 实际水平增量;
        垂直旋转 += 实际垂直增量;

        if (上下滑动模块 != null)
            上下滑动模块.localRotation = Quaternion.Euler(垂直旋转, 0f, 0f);

        if (玩家模块 != null)
            玩家模块.Rotate(Vector3.up, 实际水平增量);
    }

    float 截断增量(float 当前值, float 增量, float 半角)
    {
        float 目标值 = 当前值 + 增量;
        if (目标值 > 半角) return 半角 - 当前值;
        if (目标值 < -半角) return -半角 - 当前值;
        return 增量;
    }
}