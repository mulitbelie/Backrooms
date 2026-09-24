using UnityEngine;

public class DrawerController : LockableInteractable
{
    [Header("抽屉设置")]
    public Vector3 openOffset = new Vector3(0f, 0f, 1f);
    public float smoothSpeed = 5f;
    public float 动画完成阈值 = 0.005f;

    private Vector3 初始位置;
    private Vector3 打开位置;
    private bool 初始化完成;

    protected override string 对象名 => "drawer";

    public override bool isFullyOpen
    {
        get
        {
            if (!isOpen || !初始化完成) return false;
            return Vector3.Distance(transform.localPosition, 打开位置) < 动画完成阈值;
        }
    }

    void Start()
    {
        初始位置 = transform.localPosition;
        打开位置 = 初始位置 + openOffset;
        初始化完成 = true;
    }

    protected override void Toggle()
    {
        base.Toggle();
        enabled = true;
    }

    void Update()
    {
        Vector3 目标 = isOpen ? 打开位置 : 初始位置;
        transform.localPosition = Vector3.Lerp(
            transform.localPosition, 目标, smoothSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.localPosition, 目标) < 动画完成阈值)
            enabled = false;
    }
}