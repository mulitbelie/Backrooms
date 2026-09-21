using UnityEngine;

public class 键盘UI控制 : MonoBehaviour
{
    public 玩家画布 画布;
    public KeyCode 放下键 = KeyCode.Q;

    void Start()
    {
        if (画布 == null)
        {
            画布 = GetComponent<玩家画布>();
            if (画布 == null)
                画布 = FindObjectOfType<玩家画布>();
        }
    }

    void Update()
    {
        if (画布 == null || 画布.背包 == null) return;

        处理槽位切换();
        处理放下();
    }

    void 处理槽位切换()
    {
        for (int i = 0; i < 画布.背包.Capacity; i++)
        {
            KeyCode k = KeyCode.Alpha1 + i;
            if (Input.GetKeyDown(k))
            {
                画布.选中槽位 = i;
                return;
            }
        }
    }

    void 处理放下()
    {
        if (画布.选中槽位 < 0 || 画布.选中槽位 >= 画布.背包.Count) return;
        if (!Input.GetKeyDown(放下键)) return;

        画布.放下指定槽位(画布.选中槽位);
    }
}