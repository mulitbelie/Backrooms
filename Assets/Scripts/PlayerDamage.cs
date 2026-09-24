using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamage : MonoBehaviour
{
    public int HP = 100;
    public GameObject bloodyScreenCanvas;
    private Image _bloodyScreenImage;

    void Start()
    {
        // 在 Canvas 的子物体中查找 Image 组件
        _bloodyScreenImage = bloodyScreenCanvas.GetComponentInChildren<Image>();
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;
        if (HP <= 0)
        {
            print("玩家死亡");
        }
        else
        {
            print("玩家受到伤害");
            StartCoroutine(BloodyScreenEffect());
        }
    }

    /// <summary>
    /// 血腥屏幕效果协程：玩家受伤时显示全屏红色遮罩，4秒后自动隐藏。
    /// 使用协程而非普通方法，可以在等待期间暂停执行而不阻塞主线程。
    /// </summary>
    private IEnumerator BloodyScreenEffect()
    {
        if( bloodyScreenCanvas.activeInHierarchy == false)
        {
            bloodyScreenCanvas.SetActive(true);
        }

        Color startColor = Color.clear;
        startColor.a = 1f;
        _bloodyScreenImage.color = startColor;

        float duration = 4f;
        float elapsedTime = 0f;
        Color endColor = Color.red;
        endColor.a = 0f;
        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            
            Color newColor = _bloodyScreenImage.color;
            newColor.a = alpha;
            _bloodyScreenImage.color = newColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _bloodyScreenImage.color = endColor;

        if(bloodyScreenCanvas.activeInHierarchy == true)
        {
            bloodyScreenCanvas.SetActive(false);
        }
    }
}