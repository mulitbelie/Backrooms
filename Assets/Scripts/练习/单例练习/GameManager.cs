using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager 实例 { get; private set; }

    public int 当前分数 = 0;

    void Awake()
    {
        // ① 如果已经有一个了，我是多余的 → 自杀
        if (实例 != null && 实例 != this)
        {
            Destroy(gameObject);
            return;
        }

        // ② 我是第一个，我是唯一的 → 记录我
        实例 = this;
        // ③ 切场景不要销毁我
        DontDestroyOnLoad(gameObject);
    }

    public void 加分(int 分) // 加分方法
    {
        当前分数 += 分;
        Debug.Log($"[GameManager] 加了 {分} 分，当前总分：{当前分数}");
    }

    public void 重置分数() // 重置分数方法
    {
        当前分数 = 0;
        Debug.Log("[GameManager] 分数已重置");
    }
}