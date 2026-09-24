using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class 物品飞向玩家 : MonoBehaviour, IInteractable
{
    [Header("飞行设置")]
    public float 飞行速度 = 8f;
    public float 停留距离 = 1.5f;
    public Vector3 偏移量 = new Vector3(0f, -0.3f, 0f);

    [Header("旋转控制")]
    public float 旋转灵敏度 = 1.2f;
    public float 惯性阻尼 = 0.95f;
    public float 自动旋转速度 = 30f;
    public float 自动旋转恢复延迟 = 0.4f;

    [Header("交互提示")]
    public string 查看提示 = "examine";
    public string 收回提示 = "put back";

    public string 交互提示 =>
        正在查看
            ? $"{平台工具.按键提示} to {收回提示}"
            : $"{平台工具.按键提示} to {查看提示}";

    private Vector3 原始位置;
    private Quaternion 原始旋转;
    private bool 正在查看;
    private Coroutine 当前协程;

    private Vector3 观察位置;
    private Transform 观察相机;
    private Quaternion 手动总旋转;
    private float 手动角速度Y;
    private float 手动角速度X;
    private Vector2 上一触摸位置;
    private float 最后手动输入时间;

    void Awake()
    {
        原始位置 = transform.position;
        原始旋转 = transform.rotation;
    }

    void Update()
    {
        if (!正在查看) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            放回物品();
            return;
        }

        if (Input.GetMouseButton(0))
        {
            手动角速度Y += Input.GetAxis("Mouse X") * 旋转灵敏度 * 60f;
            手动角速度X += Input.GetAxis("Mouse Y") * 旋转灵敏度 * 60f;
            最后手动输入时间 = Time.time;
        }

        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                上一触摸位置 = t.position;
            }
            else if (t.phase == TouchPhase.Moved)
            {
                Vector2 delta = t.position - 上一触摸位置;
                手动角速度Y += delta.x * 旋转灵敏度;
                手动角速度X += delta.y * 旋转灵敏度;
                上一触摸位置 = t.position;
                最后手动输入时间 = Time.time;
            }
        }

        bool 自动旋转启用 = (Time.time - 最后手动输入时间) > 自动旋转恢复延迟;
        float yaw总速度 = (自动旋转启用 ? 自动旋转速度 : 0f) - 手动角速度Y;
        float pitch总速度 = 手动角速度X;

        Vector3 yaw轴 = Vector3.up;
        Vector3 pitch轴 = 观察相机.right;

        Quaternion 旋转增量 =
            Quaternion.AngleAxis(yaw总速度 * Time.deltaTime, yaw轴) *
            Quaternion.AngleAxis(pitch总速度 * Time.deltaTime, pitch轴);

        手动总旋转 = 旋转增量 * 手动总旋转;

        手动角速度Y *= 惯性阻尼;
        手动角速度X *= 惯性阻尼;

        if (Mathf.Abs(手动角速度Y) < 0.01f) 手动角速度Y = 0f;
        if (Mathf.Abs(手动角速度X) < 0.01f) 手动角速度X = 0f;

        transform.position = 观察位置;
        transform.rotation = 手动总旋转;
    }

    public void OnInteract()
    {
        if (正在查看)
        {
            放回物品();
            return;
        }

        if (当前协程 != null) StopCoroutine(当前协程);
        当前协程 = StartCoroutine(飞向玩家());
    }

    void 放回物品()
    {
        if (!正在查看) return;
        正在查看 = false;
        恢复玩家控制();

        手动角速度Y = 0f;
        手动角速度X = 0f;

        if (当前协程 != null) StopCoroutine(当前协程);
        当前协程 = StartCoroutine(飞回原点());
    }

    void 锁定玩家控制()
    {
        禁用<基础移动控制>(false);
        禁用<基础音效>(false);
        禁用<鼠标视角控制>(false);
        禁用<触摸视角控制>(false);
        if (Cursor.lockState == CursorLockMode.Locked)
            Cursor.lockState = CursorLockMode.None;
        设置准星显隐(false);
    }

    void 恢复玩家控制()
    {
        禁用<基础移动控制>(true);
        禁用<基础音效>(true);
        禁用<鼠标视角控制>(true);
        禁用<触摸视角控制>(true);
        if (!平台工具.是移动端)
            Cursor.lockState = CursorLockMode.Locked;
        设置准星显隐(true);
    }

    void 设置准星显隐(bool 显示)
    {
        var 画布 = FindObjectOfType<玩家画布>();
        if (画布 != null && 画布.准星 != null)
            画布.准星.gameObject.SetActive(显示);
    }

    void 禁用<T>(bool 启用) where T : MonoBehaviour
    {
        var comp = FindObjectOfType<T>();
        if (comp != null) comp.enabled = 启用;
    }

    IEnumerator 飞向玩家()
    {
        正在查看 = true;
        锁定玩家控制();

        观察相机 = Camera.main != null ? Camera.main.transform : transform;
        观察位置 = 观察相机.position + 观察相机.forward * 停留距离 + 观察相机.TransformDirection(偏移量);

        手动总旋转 = Quaternion.LookRotation(观察相机.forward, Vector3.up);
        手动角速度Y = 0f;
        手动角速度X = 0f;

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, 观察位置, 飞行速度 * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, 手动总旋转, 飞行速度 * Time.deltaTime);

            if (Vector3.Distance(transform.position, 观察位置) < 0.01f &&
                Quaternion.Angle(transform.rotation, 手动总旋转) < 1f)
                break;

            yield return null;
        }

        transform.position = 观察位置;
        transform.rotation = 手动总旋转;
    }

    IEnumerator 飞回原点()
    {
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, 原始位置, 飞行速度 * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, 原始旋转, 飞行速度 * Time.deltaTime);

            if (Vector3.Distance(transform.position, 原始位置) < 0.05f &&
                Quaternion.Angle(transform.rotation, 原始旋转) < 2f)
            {
                transform.position = 原始位置;
                transform.rotation = 原始旋转;
                当前协程 = null;
                yield break;
            }

            yield return null;
        }
    }
}