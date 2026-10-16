using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 特效提供器
/// 用于根据碰撞体的 Layer 映射到对应的特效
/// 如果没有对应的 Layer 映射，使用默认兜底特效
/// </summary>
public class ImpactEffectProvider : MonoBehaviour
{
    public static ImpactEffectProvider Instance { get; private set; }

    [Header("默认兜底特效")]
    public GameObject defaultEffectPrefab;

    [System.Serializable]
    public class LayerImpactEntry
    {
        public int layer;
        public GameObject effectPrefab;
    }

    [Header("Layer 通用特效映射")]
    // 自定义列表，用于存储 Layer 映射到的特效
    public List<LayerImpactEntry> impactsByLayer = new List<LayerImpactEntry>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void HandleImpact(Collision collision)
    {
        GameObject effectPrefab = ResolveEffectPrefab(collision.gameObject);
        if (effectPrefab != null)
        {
            SpawnImpactEffect(effectPrefab, collision);
        }
    }
    // 解析特效预制体
    private GameObject ResolveEffectPrefab(GameObject hitObject)
    {
        ImpactConfig impactConfig = hitObject.GetComponentInParent<ImpactConfig>();
        if (impactConfig != null && impactConfig.hitEffectPrefab != null)
        {
            return impactConfig.hitEffectPrefab;
        }
        // 检查 Layer 映射
        // 如果没有对应的 Layer 映射，使用默认兜底特效
        foreach (var entry in impactsByLayer)
        {
            if (entry.layer == hitObject.layer && entry.effectPrefab != null)
            {
                return entry.effectPrefab;
            }
        }

        return defaultEffectPrefab;
    }
    // 生成特效
    private void SpawnImpactEffect(GameObject prefab, Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        float randomAngle = Random.Range(0f, 360f);
        Quaternion randomRot = Quaternion.AngleAxis(randomAngle, contact.normal);

        GameObject impact = Instantiate(prefab, contact.point, randomRot * Quaternion.LookRotation(contact.normal));

        Rigidbody hitRigidbody = collision.collider.GetComponentInParent<Rigidbody>();
        Transform followTarget = hitRigidbody != null ? hitRigidbody.transform : collision.transform;

        var follower = impact.AddComponent<BulletHoleFollower>();
        follower.Init(followTarget, contact.point, contact.normal, randomAngle);
    }
    // 子弹孔洞特效跟随器
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
        // 特效跟随器更新
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