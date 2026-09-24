using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class 玩家画布 : MonoBehaviour
{   
    public 全局脚本 全局脚本;
    public VariableJoystick 摇杆;

    [Header("背包UI")]
    public Image[] 背包槽位图片;
    public Image[] 背包槽位边框;
    public GameObject 拾取提示文本;

    [Header("移动端拾取按钮")]
    public GameObject 拾取按钮;

    [Header("准星")]
    public Image 准星;
    public Color 准星默认颜色 = Color.white;
    public Color 准星可拾取颜色 = new Color(1f, 0.9f, 0.3f, 1f);

    [Header("放下设置")]
    public Color 选中颜色 = new Color(1f, 0.9f, 0.3f, 1f);
    public Color 未选中颜色 = Color.white;

    private Inventory _背包;
    private 交互检测 交互;
    private bool 事件已订阅;
    private int _选中槽位 = -1;

    private GraphicRaycaster 射线;
    private EventSystem 事件系统;
    private Canvas 画布;
    public bool 拖拽中;
    public bool 屏蔽视角控制 => 拖拽中;
    private int 拖拽槽位索引 = -1;
    private Image 拖拽视觉;
    private CanvasGroup 拖拽视觉组;
    private Vector3 拖拽起点屏幕;

    public Inventory 背包 => _背包;

    public int 选中槽位
    {
        get => _选中槽位;
        set { _选中槽位 = value; 更新背包UI(); }
    }

    public 玩家画布 与画布绑定(全局脚本 全局脚本){
        
        this.全局脚本 = 全局脚本;
        return this;
    }

    public void 初始化玩家引用()
    {
        if (全局脚本 == null)
        {
            Debug.LogError("[玩家画布] 全局脚本为 null，无法初始化！");
            return;
        }

        _背包 = 全局脚本.玩家 != null ? 全局脚本.玩家.GetComponent<Inventory>() : null;
        if (_背包 == null)
        {
            _背包 = FindObjectOfType<Inventory>();
            Debug.LogWarning($"[玩家画布] 玩家身上没找到 Inventory，用 FindObjectOfType 兜底: {(_背包 != null ? "成功" : "失败")}");
        }

        交互 = 全局脚本.玩家 != null ? 全局脚本.玩家.GetComponent<交互检测>() : null;
        if (交互 == null)
        {
            交互 = FindObjectOfType<交互检测>();
            Debug.LogWarning($"[玩家画布] 玩家身上没找到 交互检测，用 FindObjectOfType 兜底: {(交互 != null ? "成功" : "失败")}");
        }

        订阅背包事件();
        更新背包UI();
        更新拾取提示();

        射线 = GetComponentInParent<GraphicRaycaster>();
        事件系统 = FindObjectOfType<EventSystem>();
        画布 = GetComponentInParent<Canvas>();

        if (射线 == null && 画布 != null)
        {
            射线 = 画布.gameObject.AddComponent<GraphicRaycaster>();
            Debug.LogWarning("[拖拽系统] Canvas 缺 GraphicRaycaster，已自动添加");
        }
        if (事件系统 == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
            事件系统 = es.GetComponent<EventSystem>();
            Debug.LogWarning("[拖拽系统] 场景缺 EventSystem，已自动创建");
        }

        if (准星 == null)
        {
            准星 = 自动创建准星();
        }

        if (准星 != null)
        {
            var rt = 准星.rectTransform;
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(28f, 28f);
            准星.color = 准星默认颜色;
            准星.raycastTarget = false;
            准星.gameObject.SetActive(true);
            准星.transform.SetAsLastSibling();
        }
    }

    Image 自动创建准星()
    {
        Canvas 动态画布 = 找到画布();
        if (动态画布 == null) { Debug.LogWarning("★★ [准星] 找不到Canvas，无法自动创建"); return null; }

        var go = new GameObject("Crosshair", typeof(RectTransform));
        go.transform.SetParent(动态画布.transform, false);
        var img = go.AddComponent<Image>();
        img.sprite = 生成准星Sprite();
        img.preserveAspect = true;
        return img;
    }

    Sprite 生成准星Sprite()
    {
        int size = 64;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bool 中心空洞 = Mathf.Abs(x - size / 2) <= 3 && Mathf.Abs(y - size / 2) <= 3;
                bool 横线 = Mathf.Abs(y - size / 2) <= 2 && Mathf.Abs(x - size / 2) > 6;
                bool 竖线 = Mathf.Abs(x - size / 2) <= 2 && Mathf.Abs(y - size / 2) > 6;

                Color c = (横线 || 竖线) && !中心空洞 ? Color.white : new Color(0, 0, 0, 0);
                tex.SetPixel(x, y, c);
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    void Update()
    {
        if (_背包 == null)
            尝试初始化();

        更新拾取提示();
        更新准星();
        处理拖拽();
    }

    void 尝试初始化()
    {
        if (全局脚本 == null)
            全局脚本 = FindObjectOfType<全局脚本>();

        if (全局脚本 != null && 全局脚本.玩家 != null)
        {
            初始化玩家引用();
        }
        else
        {
            _背包 = FindObjectOfType<Inventory>();
            交互 = FindObjectOfType<交互检测>();

            if (_背包 != null)
            {
                订阅背包事件();
                更新背包UI();
            }
        }
    }

    void 更新准星()
    {
        if (准星 == null) return;
        准星.color = (交互 != null && 交互.有可交互物体()) ? 准星可拾取颜色 : 准星默认颜色;
    }

    void 处理拖拽()
    {
        if (背包 == null) return;

        Vector3 当前屏幕点 = Input.mousePosition;
        bool 刚按下 = false;
        bool 仍按住 = false;

        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            当前屏幕点 = t.position;
            刚按下 = t.phase == TouchPhase.Began;
            仍按住 = t.phase != TouchPhase.Ended && t.phase != TouchPhase.Canceled;
        }
        else
        {
            刚按下 = Input.GetMouseButtonDown(0);
            仍按住 = Input.GetMouseButton(0);
        }

        if (!拖拽中 && 刚按下)
        {
            开始拖拽(当前屏幕点);
        }
        else if (拖拽中 && 仍按住)
        {
            更新拖拽视觉(当前屏幕点);
        }
        else if (拖拽中 && !仍按住)
        {
            结束拖拽(当前屏幕点);
        }
    }

    Canvas 找到画布()
    {
        Canvas c = FindObjectOfType<Canvas>();
        if (c != null) return c;
        c = 背包槽位图片?[0]?.GetComponentInParent<Canvas>();
        return c;
    }

    Image 射线检测背包槽位(Vector3 屏幕点)
    {
        Canvas 动态画布 = 找到画布();
        if (动态画布 == null) { Debug.LogWarning("★★ [拖拽] 场景中找不到任何Canvas"); return null; }

        GraphicRaycaster 动态射线 = 动态画布.GetComponent<GraphicRaycaster>();
        if (动态射线 == null) 动态射线 = 动态画布.gameObject.AddComponent<GraphicRaycaster>();

        EventSystem 动态事件系统 = FindObjectOfType<EventSystem>();
        if (动态事件系统 == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
            动态事件系统 = es.GetComponent<EventSystem>();
        }

        var 结果列表 = new System.Collections.Generic.List<RaycastResult>();
        var eventData = new PointerEventData(动态事件系统) { position = 屏幕点 };
        动态射线.Raycast(eventData, 结果列表);

        foreach (var r in 结果列表)
        {
            Image img = r.gameObject.GetComponent<Image>();
            if (img == null) continue;
            for (int i = 0; i < 背包槽位图片.Length; i++)
            {
                if (背包槽位图片[i] == img)
                {
                    return img;
                }
            }
        }

        return null;
    }

    void 开始拖拽(Vector3 屏幕点)
    {
        Image 点中的槽位 = 射线检测背包槽位(屏幕点);
        if (点中的槽位 == null) return;

        for (int i = 0; i < 背包槽位图片.Length; i++)
        {
            if (背包槽位图片[i] == 点中的槽位)
            {
                if (i >= 背包.Count) return;

                拖拽中 = true;
                拖拽槽位索引 = i;
                拖拽起点屏幕 = 屏幕点;

                选中槽位 = i;
                更新背包UI();

                创建拖拽视觉(点中的槽位);
                return;
            }
        }
    }

    void 创建拖拽视觉(Image 原图)
    {
        Canvas 动态画布 = 找到画布();
        if (动态画布 == null) return;
        GameObject go = new GameObject("拖拽视觉");
        go.transform.SetParent(动态画布.transform, false);
        拖拽视觉 = go.AddComponent<Image>();
        拖拽视觉.sprite = 原图.sprite;
        拖拽视觉.rectTransform.sizeDelta = 原图.rectTransform.sizeDelta;
        拖拽视觉组 = go.AddComponent<CanvasGroup>();
        拖拽视觉组.alpha = 0.7f;
        拖拽视觉组.blocksRaycasts = false;
    }

    void 更新拖拽视觉(Vector3 屏幕点)
    {
        if (拖拽视觉 == null) return;
        Canvas 动态画布 = 找到画布();
        if (动态画布 == null) return;
        Camera cam = 动态画布.renderMode == RenderMode.ScreenSpaceOverlay ? null : 动态画布.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            动态画布.GetComponent<RectTransform>(), 屏幕点, cam, out Vector2 p);
        拖拽视觉.rectTransform.anchoredPosition = p;
    }

    void 结束拖拽(Vector3 屏幕点)
    {
        if (!拖拽中) return;
        拖拽中 = false;

        float 移动距离 = Vector2.Distance(拖拽起点屏幕, 屏幕点);
        Image 释放位置 = 射线检测背包槽位(屏幕点);
        bool 松手在背包外 = 释放位置 == null;

        Destroy(拖拽视觉?.gameObject);
        拖拽视觉 = null;
        拖拽视觉组 = null;

        if (移动距离 > 10f && 松手在背包外 && 拖拽槽位索引 >= 0)
        {
            放下指定槽位(拖拽槽位索引);
        }

        拖拽槽位索引 = -1;
    }

    public void 放下指定槽位(int index)
    {
        if (背包 == null || 全局脚本?.玩家 == null) return;
        if (index < 0 || index >= 背包.Count) return;

        // 位置对齐和落地全部由 Inventory.RemoveItem 统一处理
        背包.RemoveItem(index);

        if (选中槽位 >= 背包.Count)
            选中槽位 = Mathf.Max(0, 背包.Count - 1);
    }

    void 订阅背包事件()
    {
        if (背包 == null || 事件已订阅) return;
        背包.ItemAdded += (s, e) => 更新背包UI();
        背包.ItemRemoved += (s, e) => 更新背包UI();
        事件已订阅 = true;
    }

    public void 更新背包UI()
    {
        if (背包槽位图片 == null) return;

        for (int i = 0; i < 背包槽位图片.Length; i++)
        {
            if (背包槽位图片[i] == null) continue;

            bool 是否选中 = (i == 选中槽位);
            Color 目标颜色 = 是否选中 ? 选中颜色 : 未选中颜色;

            if (i < 背包.Count)
            {
                var item = 背包.GetItem(i);
                背包槽位图片[i].sprite = item.Image;
                背包槽位图片[i].enabled = true;
                背包槽位图片[i].color = 目标颜色;
            }
            else
            {
                背包槽位图片[i].sprite = null;
                背包槽位图片[i].enabled = true;
                Color c = 目标颜色; c.a = 0f;
                背包槽位图片[i].color = c;
            }

            if (背包槽位边框 != null && i < 背包槽位边框.Length && 背包槽位边框[i] != null)
            {
                Color 目标边框色 = 是否选中 ? 选中颜色 : 未选中颜色;
                背包槽位边框[i].color = 目标边框色;
            }
        }
    }

    void 更新拾取提示()
    {
        bool 有物体 = 交互 != null && 交互.有可交互物体();

        if (拾取提示文本 != null)
        {
            拾取提示文本.SetActive(有物体);
            if (有物体)
            {
                拾取提示文本.GetComponentInChildren<TMP_Text>().text = 交互.获取交互提示();
            }
        }

        if (拾取按钮 != null)
        {
            拾取按钮.SetActive(有物体);
        }
    }

    public void 移动端拾取按钮点击()
    {
        if (交互 != null)
        {
            交互.执行交互();
        }
    }
}