using UnityEngine;
using UnityEngine.UI;

public class ReticleT : MonoBehaviour
{
    [Header("移动")]
    public float speed = 15f;

    [Header("UI元素")]
    public RectTransform reticle, Top, Bottom, Left, Right;

    [Header("扩散")]
    public float spreadPerShot = 8f;
    public float spreadMax = 40f;
    public float spreadRecoverSpeed = 12f;
    
    //是否持有武器bool变量
    private bool weaponHeld;
    private float baseMargin;
    private float shotMargin;
    private Vector2 topInitial, bottomInitial, leftInitial, rightInitial;
    private Vector2 topTarget, bottomTarget, leftTarget, rightTarget;

    private void Start()
    {
        topInitial = Top.anchoredPosition;
        bottomInitial = Bottom.anchoredPosition;
        leftInitial = Left.anchoredPosition;
        rightInitial = Right.anchoredPosition;

        SetWeaponHeld(false);
    }

    private void Update()
    {
        if (!weaponHeld) return;

        UpdateTargetPositions();

        Top.anchoredPosition    = Vector2.Lerp(Top.anchoredPosition,    topTarget,    speed * Time.deltaTime);
        Bottom.anchoredPosition = Vector2.Lerp(Bottom.anchoredPosition, bottomTarget, speed * Time.deltaTime);
        Left.anchoredPosition   = Vector2.Lerp(Left.anchoredPosition,   leftTarget,   speed * Time.deltaTime);
        Right.anchoredPosition  = Vector2.Lerp(Right.anchoredPosition,  rightTarget,  speed * Time.deltaTime);

        shotMargin = Mathf.Lerp(shotMargin, 0f, spreadRecoverSpeed * Time.deltaTime);
    }

    //设置是否持有武器方法 我调用
    public void SetWeaponHeld(bool held)
    {
        weaponHeld = held;
        if (Top != null) Top.gameObject.SetActive(held);
        if (Bottom != null) Bottom.gameObject.SetActive(held);
        if (Left != null) Left.gameObject.SetActive(held);
        if (Right != null) Right.gameObject.SetActive(held);
    }

    //设置基础扩散值
    public void SetBaseSpread(float margin)
    {
        baseMargin = Mathf.Clamp(margin, 0f, spreadMax);
    }

    //增加开火扩散值
    public void AddSpread()
    {
        shotMargin = Mathf.Min(shotMargin + spreadPerShot, spreadMax - baseMargin);
    }

    //更新目标位置
    private void UpdateTargetPositions()
    {
        float m = Mathf.Min(baseMargin + shotMargin, spreadMax);
        topTarget    = topInitial    + new Vector2(0, m);
        bottomTarget = bottomInitial + new Vector2(0, -m);
        leftTarget   = leftInitial   + new Vector2(-m, 0);
        rightTarget  = rightInitial  + new Vector2(m, 0);
    }
}