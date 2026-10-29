using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour, IHitReceiver
{
    public int zombieDamage;
    [SerializeField] private int HP = 100;
    public bool IsDead => HP <= 0;
    private Animator animator;

    [Header("僵尸受伤死亡延续时间")]
    public float dieDelayTime = 2f;
    public float getHitingDelayTime = 0.5f;
    //Renderer数组
    private Renderer[] zombieRenderers;
    //溶解材质的参数
    private string cutoffHeight= "_CutoffHeight";
    [Header("溶解时间")]
    public float dissolveTime = 1f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        zombieRenderers = GetComponentsInChildren<Renderer>();
        // 初始化材质数组
        for (int i = 0; i < zombieRenderers.Length; i++)
        {
            Material m = zombieRenderers[i].material;
           
        }
    }

    public void HandleHit(Collision collision, int damage, GameObject bullet)
    {
        ZombieState perception = GetComponent<ZombieState>();
        if (perception != null)
        {
            Transform attacker = bullet != null ? bullet.transform.root : perception.player;
            perception.ForceAware(attacker);
        }
        TakeDamage(damage);
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;
        if (HP <= 0)
        {

            int randomValue = Random.Range(0, 2);//0或1

            if (randomValue == 0)
            {
                animator.SetTrigger("IsDie1");
            }
            else
            {
                animator.SetTrigger("IsDie2");
            }
            
            StartCoroutine(DieCoroutine());//死亡协程
        }
        else
        {
            animator.SetTrigger("IsGetHiting");
            StartCoroutine(GetHitingCoroutine());
        }
    }

//3.设置溶解高度
    private void SetCutoffHeight(float value)
{
    for (int i = 0; i < zombieRenderers.Length; i++)
    {
        //zombieRenderers[i] →拿到数组里第i个Renderer组件
        //.material →从这个Renderer上获取材质组件
        Material m = zombieRenderers[i].material;
        //如果材质存在且有CutoffHeight参数
        if (m != null && m.HasProperty(cutoffHeight))
        {
            //设置材质参数（溶解高度 0-1）
            m.SetFloat(cutoffHeight, value);
        }
    }
}
//2.溶解协程  
    private IEnumerator DissolveCoroutine(float duration)
{
    // 先初始化：溶解参数直接设为1，模型完整显示
    SetCutoffHeight(1f);
    //已经过去的时间
    float elapsed = 0f;
    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        //t 从 0-1 变化
        float t = Mathf.Clamp01(elapsed / duration);
        // 1 - t：从1 → 0，传给材质的CutoffHeight
        SetCutoffHeight(1f - t);
        yield return null;
    }
    // 最后兜底，强制设置为0，保证完全消失
    SetCutoffHeight(0f);
}
    //1.死亡协程多少秒后销毁僵尸 调用溶解协程duration秒后销毁僵尸
    private IEnumerator DieCoroutine()
    {
        // 先播放死亡动画（dieDelayTime 秒）
        yield return new WaitForSeconds(dieDelayTime/2);
        // 1 秒的溶解时间
        yield return StartCoroutine(DissolveCoroutine(dissolveTime));
        //销毁僵尸
        Destroy(gameObject);
    }
    //受伤协程多少秒后恢复动画
    private IEnumerator GetHitingCoroutine()
    {
        yield return new WaitForSeconds(getHitingDelayTime);
    }

   
}