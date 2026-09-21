using UnityEngine;

public interface IInteractable
{
    string 交互提示 { get; }
    void OnInteract();
}