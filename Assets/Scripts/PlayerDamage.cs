using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerDamage : MonoBehaviour
{
    public int HP = 100;
    public int maxHP = 100;

    [Header("免伤设置")]
    public float 受击后免伤时间 = 1.5f;
    private bool isInvincible;

    [Header("血腥屏幕（自动查找）")]
    public GameObject bloodyScreen;

    public TextMeshProUGUI playerHealthUI;
    private bool ui已初始化;


    void Start()
    {
        尝试查找UI();
        if (playerHealthUI != null)
            playerHealthUI.text = $"{maxHP}/{HP}";

        if (bloodyScreen != null)
            bloodyScreen.SetActive(false);
    }

    void Update()
    {
        if (!ui已初始化 || bloodyScreen == null)
        {
            尝试查找UI();

            if (playerHealthUI != null && !ui已初始化)
            {
                ui已初始化 = true;
                playerHealthUI.text = $"{maxHP}/{HP}";
            }

            if (bloodyScreen != null && bloodyScreen.activeSelf)
                bloodyScreen.SetActive(false);
        }
    }

    void 尝试查找UI()
    {
        if (playerHealthUI != null && bloodyScreen != null) return;

        var allCanvases = FindObjectsOfType<Canvas>(true);
        foreach (var canvas in allCanvases)
        {
            foreach (var t in canvas.transform.GetComponentsInChildren<Transform>(true))
            {
                if (playerHealthUI == null)
                {
                    if (t.name.IndexOf("Health", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        t.name.IndexOf("HP", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        var tmp = t.GetComponent<TextMeshProUGUI>();
                        if (tmp != null) playerHealthUI = tmp;
                    }
                }

                if (bloodyScreen == null)
                {
                    if (string.Equals(t.name, "BloodyScreen", StringComparison.OrdinalIgnoreCase))
                        bloodyScreen = t.gameObject;
                }

                if (playerHealthUI != null && bloodyScreen != null) return;
            }
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isInvincible) return;

        HP -= damageAmount;
        if (HP <= 0)
        {
            HP = 0;
            print("玩家死亡");
            if (playerHealthUI != null)
                playerHealthUI.text = $"{maxHP}/{HP}";
            PlayerDead();
        }
        else
        {
            print("玩家受到伤害");
            StopAllCoroutines();
            StartCoroutine(BloodyScreenEffect());
            StartCoroutine(受击免伤());
            if (playerHealthUI != null)
                playerHealthUI.text = $"{maxHP}/{HP}";
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

        var allBehaviours = transform.root.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (var c in allBehaviours)
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