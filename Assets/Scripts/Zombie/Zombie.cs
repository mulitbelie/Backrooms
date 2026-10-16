using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour, IHitReceiver
{
    public int zombieDamage;
    [SerializeField] private int HP = 100;
    private Animator animator;

    [Header("僵尸受伤死亡延续时间")]
    public float dieDelayTime = 2f;
    public float getHitingDelayTime = 0.5f;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void HandleHit(Collision collision, int damage, GameObject bullet)
    {
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

    private IEnumerator DieCoroutine()
    {
        yield return new WaitForSeconds(dieDelayTime);
        Destroy(gameObject);
    }

    private IEnumerator GetHitingCoroutine()
    {
        yield return new WaitForSeconds(getHitingDelayTime);
    }
}