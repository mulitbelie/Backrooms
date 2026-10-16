using Unity.VisualScripting;
using UnityEngine;

public class Box<T>//类型参数（类型占位符）
{
    public T item;
    public void Show()
    {
        Debug.Log($"盒子里的内容：{item}");
    }
}



