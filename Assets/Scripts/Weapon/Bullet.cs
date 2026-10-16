using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int bulletDamage;

    private void OnCollisionEnter(Collision collision)
    {
        IHitReceiver damageHandler = collision.gameObject.GetComponentInParent<IHitReceiver>();
        if (damageHandler != null)
        {
            damageHandler.HandleHit(collision, bulletDamage, gameObject);
        }

        if (ImpactEffectProvider.Instance != null)
        {
            ImpactEffectProvider.Instance.HandleImpact(collision);
        }

        Destroy(gameObject);
    }
}