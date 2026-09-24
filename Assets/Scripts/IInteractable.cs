using UnityEngine;

public interface IInteractable
{
    string 交互提示 { get; }
    void OnInteract();
    bool 不需要射线命中 => false;
}