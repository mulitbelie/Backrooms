using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int bulletDamage;
/// <summary>
/// 子弹命中碰撞回调，分两步处理：先伤害、再特效，最后自毁。
/// 伤害与特效完全解耦，各自独立查询，互不依赖。
/// </summary>
/// //【子弹碰到物体自动执行】
    private void OnCollisionEnter(Collision collision)//撞击到的物体
    {
            // ========== 第一步：伤害查询 ==========
    // 查找【碰撞物体/它的父物体】身上有没有实现 IHitReceiver 接口
    // IHitReceiver 代表这个物体可以接收伤害（僵尸、可破坏木桶这类）
        IHitReceiver damageHandler = collision.gameObject.GetComponentInParent<IHitReceiver>();
        if (damageHandler != null)// 如果找到了可接收伤害的物体
        {
            // 调用统一的受击接口，传递碰撞信息、伤害值、子弹本体
            damageHandler.HandleHit(collision, bulletDamage, gameObject);
        }
            // ========== 第二步：特效生成 ==========
        // 根据被撞击物体，去查找应该使用哪一个撞击特效预制体
        GameObject effectPrefab = ResolveEffectPrefab(collision.gameObject);
        if (effectPrefab != null)// 如果找到了对应的特效
        {
            // 在碰撞位置生成撞击特效（火花、血花、木屑）
            SpawnImpactEffect(effectPrefab, collision);
        }
            // ========== 第三步：子弹撞击完成 子弹自毁 ==========
        Destroy(gameObject);
    }
//【查找撞击特效预制体的函数】
    private GameObject ResolveEffectPrefab(GameObject hitObject)
    {
        // 第一步：先看物体（或父物体）有没有ImpactConfig脚本     
        // ImpactConfig：物体专属撞击配置，用来放这个物体独有的特效（比如僵尸血花）
        ImpactConfig impactConfig = hitObject.GetComponentInParent<ImpactConfig>();
        if (impactConfig != null && impactConfig.hitEffectPrefab != null)
        {
            return impactConfig.hitEffectPrefab;// 如果有配置，并且配置里指定了特效，直接返回这个特效
        }
        // 第二步：物体没有专属特效，去全局配置里面按Layer找对应的特效（墙、铁门、木墙）
        if (GlobalReferences.instance != null)
        {
        // 根据物体的Layer，拿到该材质对应的撞击特效（墙、铁门、木墙）
        //GlobalReferences：全局引用类体，用来存储所有层对应撞击特效的预制体
            GameObject layerEffect = GlobalReferences.instance.GetImpactByLayer(hitObject.layer);
            if (layerEffect != null) return layerEffect;
        // 如果GlobalReferences里没有匹配Layer，返回全局默认撞击特效
            if (GlobalReferences.instance.DefaultImpact != null)
            {
                return GlobalReferences.instance.DefaultImpact;
            }
        }
        // 三层全空（ImpactConfig 没挂 + Layer 没配 + 默认也没赋值）才会走到这里，返回 null 让调用方跳过生成。
        // 实际跑起来几乎不会遇到，除非 GlobalReferences 物体丢了或 Inspector 里全空着。
        return null;
    }

    private void SpawnImpactEffect(GameObject prefab, Collision collision)//生成子弹孔洞特效
    {
        ContactPoint contact = collision.contacts[0];  //获取第一个碰撞点
        float randomAngle = Random.Range(0f, 360f);
        Quaternion randomRot = Quaternion.AngleAxis(randomAngle, contact.normal);

        GameObject impact = Instantiate(prefab, contact.point, randomRot * Quaternion.LookRotation(contact.normal));

        var follower = impact.AddComponent<BulletHoleFollower>();  
        follower.Init(collision.transform, contact.point, contact.normal, randomAngle);
    }

    private class BulletHoleFollower : MonoBehaviour//子弹孔洞特效跟随器
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