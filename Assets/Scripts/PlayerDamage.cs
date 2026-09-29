using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerDamage : MonoBehaviour
{
    public int HP = 100;

    [Header("免伤设置")]
    public float 受击后免伤时间 = 1.5f;
    private bool isInvincible;

    [Header("血腥屏幕（自动查找）")]
    public GameObject bloodyScreen;

    public TextMeshProUGUI playerHealthUI;


    void Start()
    {
        if (playerHealthUI == null)
        {
            var 画布 = FindObjectOfType<玩家画布>();
            if (画布 != null)
            {
                var canvas = 画布.GetComponentInChildren<Canvas>();
                if (canvas != null)
                {
                    var healthObj = canvas.transform.Find("PlayerHealth");
                    if (healthObj != null)
                        playerHealthUI = healthObj.GetComponent<TextMeshProUGUI>();
                }
            }
        }

        if (playerHealthUI != null)
            playerHealthUI.text = $"Health: {HP}";

        if (bloodyScreen == null)
        {
            var 画布 = FindObjectOfType<玩家画布>();
            if (画布 != null)
            {
                var canvas = 画布.GetComponentInChildren<Canvas>();
                if (canvas != null)
                {
                    foreach (var t in canvas.transform.GetComponentsInChildren<Transform>(true))
                    {
                        if (string.Equals(t.name, "BloodyScreen", StringComparison.OrdinalIgnoreCase))
                        {
                            bloodyScreen = t.gameObject;
                            break;
                        }
                    }
                }
            }
        }

        if (bloodyScreen != null)
            bloodyScreen.SetActive(false);
    }

    public void TakeDamage(int damageAmount)
    {
        if (isInvincible) return;

        HP -= damageAmount;
        if (HP <= 0)
        {
            HP = 0;
            print("玩家死亡");
            PlayerDead();
        }
        else
        {
            print("玩家受到伤害");
            StopAllCoroutines();
            StartCoroutine(BloodyScreenEffect());
            StartCoroutine(受击免伤());
            if (playerHealthUI != null)
                playerHealthUI.text = $"Health: {HP}";
        }
    }

    private IEnumerator 受击免伤()
    {
        isInvincible = true;
        yield return new WaitForSeconds(受击后免伤时间);
        isInvincible = false;
    }

    private void PlayerDead()
    {
        StopBloodyScreen();

        var controls = GetComponentsInChildren<MonoBehaviour>();
        foreach (var c in controls)
        {
            if (c is 基础移动控制 || c is 鼠标视角控制 || c is 触摸视角控制 || c is 键盘控制 || c is 跳跃逻辑 || c is 下蹲ui控制)
            {
                c.enabled = false;
            }
        }
        GetComponentInChildren<Animator>().enabled = true;
        if (playerHealthUI != null)
            playerHealthUI.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ZombieHand"))
        {
            TakeDamage(other.gameObject.GetComponent<ZombieHand>().damage);
        }
    }

    private void StopBloodyScreen()
    {
        StopAllCoroutines();
        if (bloodyScreen != null)
        {
            var image = bloodyScreen.GetComponentInChildren<Image>();
            if (image != null)
            {
                var c = image.color;
                c.a = 0f;
                image.color = c;
            }
            bloodyScreen.SetActive(false);
        }
    }

    private IEnumerator BloodyScreenEffect()
    {
        if (bloodyScreen == null) yield break;

        if (bloodyScreen.activeInHierarchy == false)
            bloodyScreen.SetActive(true);

        var image = bloodyScreen.GetComponentInChildren<Image>();
        if (image == null) yield break;

        Color startColor = new Color(1f, 0f, 0f, 1f);
        image.color = startColor;

        float duration = 4f;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (bloodyScreen.activeInHierarchy == true)
            bloodyScreen.SetActive(false);
    }
}