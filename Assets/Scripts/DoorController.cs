using UnityEngine;

public class DoorController : LockableInteractable
{
    [Header("门设置")]
    public float openAngle = 90f;
    public float closeAngle = 0f;
    public float smoothSpeed = 5f;
    public float 动画完成阈值 = 0.5f;

    private Quaternion openRotation;
    private Quaternion closeRotation;
    private bool 初始化完成;

    protected override string 对象名 => "door";

    public override bool isFullyOpen
    {
        get
        {
            if (!isOpen || !初始化完成) return false;
            return Quaternion.Angle(transform.localRotation, openRotation) < 动画完成阈值;
        }
    }

    void Start()
    {
        openRotation = Quaternion.Euler(0f, openAngle, 0f);
        closeRotation = Quaternion.Euler(0f, closeAngle, 0f);

        if (!gameObject.CompareTag("Door"))
            gameObject.tag = "Door";

        初始化完成 = true;
    }

    protected override void Toggle()
    {
        base.Toggle();
        enabled = true;
    }

    void Update()
    {
        Quaternion 目标 = isOpen ? openRotation : closeRotation;
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation, 目标, smoothSpeed * Time.deltaTime);

        if (Quaternion.Angle(transform.localRotation, 目标) < 动画完成阈值)
            enabled = false;
    }
}