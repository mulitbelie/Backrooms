using UnityEngine;

public class Note : MonoBehaviour, IInteractable
{
    [Header("笔记设置")]
    public Sprite noteImage;
    public string noteTitle = "笔记";
    public float triggerRadius = 2f;

    public string 交互提示 => $"{平台工具.按键提示} to see {noteTitle}";

    void Awake()
    {
        SphereCollider trigger = GetComponent<SphereCollider>();
        if (trigger == null)
            trigger = gameObject.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = triggerRadius;
    }

    public void OnInteract()
    {
        if (PlayerNote.Instance != null)
            PlayerNote.Instance.Show(noteImage, noteTitle);
    }
}