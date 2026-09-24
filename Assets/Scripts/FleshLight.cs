using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleshLight : MonoBehaviour
{
    [Header("玩家手上的手电模型（纯外观 prefab，不含 Light 组件）")]
    public GameObject 手上手电模型;

    [Header("手电筒灯光 GameObject（含 Light 组件）")]
    public GameObject fleshLightGo;

    [Header("手电筒道具的 Tag")]
    public string 手电筒Tag = "Flashlight";

    private bool 已拾取 = false;
    private bool usingFLightGo = false;
    private Inventory 背包;
    private 基础移动控制 移动控制;

    void Start()
    {
        已拾取 = false;
        usingFLightGo = false;
        if (手上手电模型 != null)
            手上手电模型.SetActive(false);
        if (fleshLightGo != null)
            fleshLightGo.SetActive(false);

        背包 = GetComponentInParent<Inventory>();
        if (背包 == null) 背包 = FindObjectOfType<Inventory>();
        if (背包 != null)
        {
            背包.ItemAdded += OnItemAdded;
            背包.ItemRemoved += OnItemRemoved;
        }

        移动控制 = GetComponentInParent<基础移动控制>();
        if (移动控制 == null) 移动控制 = FindObjectOfType<基础移动控制>();
    }

    void OnDestroy()
    {
        if (背包 != null)
        {
            背包.ItemAdded -= OnItemAdded;
            背包.ItemRemoved -= OnItemRemoved;
        }
    }

    /// <summary>
    /// 背包添加物品时触发 —— 通过 Tag 检测是否拾取了手电筒道具
    /// </summary>
    void OnItemAdded(object sender, InventoryEventArgs e)
    {
        if (已拾取) return;

        var mb = e.Item as MonoBehaviour;
        if (mb != null && mb.CompareTag(手电筒Tag))
        {
            已拾取 = true;
            // 拾取后：显示手上的手电模型，但灯光保持关闭
            if (手上手电模型 != null)
                手上手电模型.SetActive(true);
        }
    }

    /// <summary>
    /// 背包移除物品时触发 —— 通过 Tag 检测是否放下了手电筒
    /// </summary>
    void OnItemRemoved(object sender, InventoryEventArgs e)
    {
        if (!已拾取) return;

        var mb = e.Item as MonoBehaviour;
        if (mb != null && mb.CompareTag(手电筒Tag))
        {
            已拾取 = false;
            // 放下后：先关灯，再隐藏手上的手电模型
            usingFLightGo = false;
            if (fleshLightGo != null)
                fleshLightGo.SetActive(false);
            if (手上手电模型 != null)
                手上手电模型.SetActive(false);
        }
    }

    void Update()
    {
        UseFlashLight();
    }

    private void UseFlashLight()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFlashlight();
        }
    }

    /// <summary>
    /// 切换手电筒开关状态（外部 UI 按钮调用入口）
    /// </summary>
    public void ToggleFlashlight()
    {
        // 未拾取手电筒时不允许开关
        if (!已拾取)
        {
            Debug.Log("[FleshLight] 还没有手电筒，先去拾取吧");
            return;
        }

        usingFLightGo = !usingFLightGo;
        if (fleshLightGo != null)
            fleshLightGo.SetActive(usingFLightGo);
    }

    /// <summary>
    /// 获取手电筒当前是否处于开启状态
    /// </summary>
    /// <returns>true=已开启，false=已关闭</returns>
    public bool IsOn()
    {
        return usingFLightGo;
    }

    /// <summary>
    /// 是否已拾取手电筒（UI 可据此决定是否显示手电筒按钮）
    /// </summary>
    public bool HasFlashlight()
    {
        return 已拾取;
    }
}