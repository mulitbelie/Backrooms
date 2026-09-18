using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class 跳跃逻辑 : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Sprite[] 图片ui;

    基础移动控制 获取移动控制()
    {
        var 全局脚本物体 = GameObject.FindGameObjectsWithTag("全局脚本");
        if (全局脚本物体.Length == 0) return null;
        var 全局脚本组件 = 全局脚本物体[0].GetComponent<全局脚本>();
        if (全局脚本组件 == null || 全局脚本组件.玩家 == null) return null;
        return 全局脚本组件.玩家.GetComponent<基础移动控制>();
    }

    void 更新图片(bool 按下中)
    {
        var image = transform.GetComponent<Image>();
        if (按下中)
        {
            if (图片ui.Length > 1) image.sprite = 图片ui[1];
        }
        else
        {
            if (图片ui.Length > 0) image.sprite = 图片ui[0];
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var 移动控制 = 获取移动控制();
        if (移动控制 == null) return;

        移动控制.跳跃();
        更新图片(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        更新图片(false);
    }
}