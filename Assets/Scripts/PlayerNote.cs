using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerNote : MonoBehaviour
{
    public static PlayerNote Instance { get; private set; }
    private GameObject notePanel;
    private Image noteImageComponent;
    private TextMeshProUGUI titleText;
    private TextMeshProUGUI hintText;
    [Header("关闭提示词")]
    [TextArea] public string 桌面端关闭提示 = "Press E or ESC to close";
    [TextArea] public string 移动端关闭提示 = "Tap to close";
    private bool isShowing;
    void Awake()
    {
        enabled = true;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        查找预制笔记();
    }
    void 查找预制笔记()
    {
        Canvas[] 所有画布 = FindObjectsOfType<Canvas>(true);
        foreach (var c in 所有画布)
        {
            var 笔记子节点 = c.transform.Find("笔记");
            if (笔记子节点 != null)
            {
                notePanel = 笔记子节点.gameObject;
                break;
            }
        }
        if (notePanel == null)
        {
            return;
        }
        noteImageComponent = notePanel.transform.Find("NoteImage")?.GetComponent<Image>();
        titleText = notePanel.transform.Find("Title")?.GetComponent<TextMeshProUGUI>();
        hintText = notePanel.transform.Find("Hint")?.GetComponent<TextMeshProUGUI>();
        if (hintText != null)
            hintText.text = 平台工具.是移动端 ? 移动端关闭提示 : 桌面端关闭提示;
        notePanel.SetActive(false);
    }
    void Update()
    {
        if (!isShowing) return;
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
            return;
        }
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                Close();
                return;
            }
        }
    }
    public void Show(Sprite image, string title)
    {
        enabled = true;
        if (isShowing) return;
        if (notePanel == null) return;
        isShowing = true;
        if (noteImageComponent != null) noteImageComponent.sprite = image;
        if (titleText != null) titleText.text = string.IsNullOrEmpty(title) ? "笔记" : title;
        notePanel.SetActive(true);
        if (Cursor.lockState == CursorLockMode.Locked)
            Cursor.lockState = CursorLockMode.None;
        设置玩家控制(false);
        设置准星显隐(false);
    }
    public void Close()
    {
        if (!isShowing) return;
        isShowing = false;
        if (notePanel != null) notePanel.SetActive(false);
        设置玩家控制(true);
        设置准星显隐(true);
        if (!平台工具.是移动端)
            Cursor.lockState = CursorLockMode.Locked;
    }
    void 设置玩家控制(bool 启用)
    {
        禁用<基础移动控制>(启用);
        禁用<基础音效>(启用);
        禁用<鼠标视角控制>(启用);
        禁用<触摸视角控制>(启用);
        禁用<交互检测>(启用);
        禁用<PC交互触发>(启用);
    }
    void 禁用<T>(bool 启用) where T : MonoBehaviour
    {
        var comp = FindObjectOfType<T>();
        if (comp != null) comp.enabled = 启用;
    }
    void 设置准星显隐(bool 显示)
    {
        var 画布 = FindObjectOfType<玩家画布>();
        if (画布 != null && 画布.准星 != null)
            画布.准星.gameObject.SetActive(显示);
    }
    public bool IsShowing => isShowing;
}