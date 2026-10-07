using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    [Header("玩家手上的手电模型（纯外观 prefab，不含 Light 组件）")]
    public GameObject 手上手电模型;

    [Header("手电筒灯光 GameObject（含 Light 组件）")]
    public GameObject fleshLightGo;

    [Header("手电筒道具的 Tag")]
    public string 手电筒Tag = "Flashlight";

    public event Action<bool> OnPickupStateChanged;
    public event Action<bool> OnLightStateChanged;

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

        背包 = GetComponent<Inventory>();
        if (背包 == null) 背包 = GetComponentInParent<Inventory>();
        if (背包 == null) 背包 = transform.root.GetComponentInChildren<Inventory>();
        if (背包 != null)
        {
            背包.ItemAdded += OnItemAdded;
            背包.ItemRemoved += OnItemRemoved;
        }

        移动控制 = GetComponentInParent<基础移动控制>();
        if (移动控制 == null) 移动控制 = transform.root.GetComponentInChildren<基础移动控制>();
    }

    void OnDestroy()
    {
        if (背包 != null)
        {
            背包.ItemAdded -= OnItemAdded;
            背包.ItemRemoved -= OnItemRemoved;
        }
    }

    void OnItemAdded(object sender, InventoryEventArgs e)
    {
        if (已拾取) return;

        var mb = e.Item as MonoBehaviour;
        if (mb != null && mb.CompareTag(手电筒Tag))
        {
            var invItem = e.Item as InventoryItem;
            if (invItem != null && !LockableInteractable.IsAccessible(invItem))
            {
                Debug.Log("[FleshLight] 手电筒在柜子里，先打开柜门才能拾取");
                return;
            }

            已拾取 = true;
            if (手上手电模型 != null)
                手上手电模型.SetActive(true);
            OnPickupStateChanged?.Invoke(true);
        }
    }

    void OnItemRemoved(object sender, InventoryEventArgs e)
    {
        if (!已拾取) return;

        var mb = e.Item as MonoBehaviour;
        if (mb != null && mb.CompareTag(手电筒Tag))
        {
            已拾取 = false;
            usingFLightGo = false;
            if (fleshLightGo != null)
                fleshLightGo.SetActive(false);
            if (手上手电模型 != null)
                手上手电模型.SetActive(false);
            OnPickupStateChanged?.Invoke(false);
            OnLightStateChanged?.Invoke(false);
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

    public void ToggleFlashlight()
    {
        if (!已拾取)
        {
            Debug.Log("[FleshLight] 还没有手电筒，先去拾取吧");
            return;
        }

        usingFLightGo = !usingFLightGo;
        if (fleshLightGo != null)
            fleshLightGo.SetActive(usingFLightGo);
        OnLightStateChanged?.Invoke(usingFLightGo);
    }

    public bool IsOn()
    {
        return usingFLightGo;
    }

    public bool HasFlashlight()
    {
        return 已拾取;
    }
}