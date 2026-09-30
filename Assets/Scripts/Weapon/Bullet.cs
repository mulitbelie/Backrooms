using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision objectWeHit)
    {
        CreateBulletImpactEffect(objectWeHit);
        Destroy(gameObject);
    }

    void CreateBulletImpactEffect(Collision objectWeHit)
    {
        if (GlobalReferences.instance == null || GlobalReferences.instance.bulletImpactEffectPrefab == null)
        {
            Debug.LogWarning("bulletImpactEffectPrefab 未赋值，请在 GlobalReferences 的 Inspector 中配置！");
            return;
        }

        ContactPoint contact = objectWeHit.contacts[0];

        float randomAngle = Random.Range(0f, 360f);
        Quaternion randomRot = Quaternion.AngleAxis(randomAngle, contact.normal);

        GameObject impact = Instantiate(
            GlobalReferences.instance.bulletImpactEffectPrefab,
            contact.point,
            randomRot * Quaternion.LookRotation(contact.normal)
        );

        var follower = impact.AddComponent<BulletHoleFollower>();
        follower.Init(objectWeHit.transform, contact.point, contact.normal, randomAngle);
    }

    private class BulletHoleFollower : MonoBehaviour
    {
        private Transform target;
        private Vector3 localPoint;
        private Vector3 localNormal;
        private float randomAngle;

        public void Init(Transform target, Vector3 worldPoint, Vector3 worldNormal, float randomAngle)
        {
            this.target = target;
            this.randomAngle = randomAngle;
            localPoint = target.InverseTransformPoint(worldPoint);
            localNormal = target.InverseTransformDirection(worldNormal);
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 worldNormal = target.TransformDirection(localNormal);
            transform.position = target.TransformPoint(localPoint);
            transform.rotation = Quaternion.AngleAxis(randomAngle, worldNormal) * Quaternion.LookRotation(worldNormal);
        }
    }
}