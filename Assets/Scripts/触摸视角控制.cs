using System.Collections.Generic;
using UnityEngine;

public class 触摸视角控制 : MonoBehaviour
{
    public Transform 玩家模块;
    public Transform 上下滑动模块;

    [Header("隔离UI交互的画布（自动查找）")]
    public 玩家画布 画布;

    [Header("水平旋转（像素 → 度）")]
    public float 水平灵敏度 = 2f;
    [Header("垂直旋转（像素 → 度）")]
    public float 垂直灵敏度 = 2f;

    [Header("衣柜中限制（WardrobeController 自动设置）")]
    [Range(0f, 180f)] public float 衣柜水平半角 = 60f;
    [Range(0f, 90f)]  public float 衣柜垂直半角 = 60f;

    private float 水平旋转;
    private float 垂直旋转;
    private float 水平半角 = float.MaxValue;
    private float 垂直半角 = float.MaxValue;
    private int 当前触摸索引 = -1;
    private readonly HashSet<int> 禁用触摸点 = new HashSet<int>();

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
        if (画布 == null)
            画布 = FindObjectOfType<玩家画布>();
    }

    void LateUpdate()
    {
        if (画布 != null && 画布.屏蔽视角控制) return;

        if (Input.touchCount == 0)
        {
            当前触摸索引 = -1;
            禁用触摸点.Clear();
            return;
        }

        Touch 当前触摸;

        if (当前触摸索引 < 0)
        {
            当前触摸 = 找到右侧触摸点();
            if (当前触摸.fingerId < 0) return;
            当前触摸索引 = 当前触摸.fingerId;
        }
        else
        {
            当前触摸 = 按FingerId取触摸(当前触摸索引);

            if (当前触摸.fingerId < 0 || 当前触摸.phase == TouchPhase.Ended || 当前触摸.phase == TouchPhase.Canceled)
            {
                当前触摸 = 找到右侧触摸点();
                当前触摸索引 = 当前触摸.fingerId;
                if (当前触摸索引 < 0) return;
            }
        }

        Vector2 delta = 当前触摸.deltaPosition;
        if (Mathf.Abs(delta.x) < 0.01f && Mathf.Abs(delta.y) < 0.01f) return;

        float 水平增量 = delta.x * 水平灵敏度;
        float 垂直增量 = -delta.y * 垂直灵敏度;

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

    Touch 找到右侧触摸点()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            var t = Input.GetTouch(i);
            if (禁用触摸点.Contains(t.fingerId)) continue;
            if (t.position.x < Screen.width * 0.5f) continue;
            return t;
        }
        return default;
    }

    Touch 按FingerId取触摸(int fingerId)
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            var t = Input.GetTouch(i);
            if (t.fingerId == fingerId) return t;
        }
        return default;
    }
}