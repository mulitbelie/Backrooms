using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //public Camera playerCamera;
    //射击
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;
    //Spread
    public float spreadIntensity;
    // 子弹预制体
    [Header("子弹预制体")]
    public GameObject bulletPrefab;
    // 子弹发射点
    [Header("子弹发射点")]
    public Transform bulletSpawn;
    // 子弹速度
    [Header("子弹速度")]
    public float bulletVelocity = 30f;
    public float bulletPrefabLifeTime = 2f;

    public GameObject muzzleEffect;

    public Animator animator;

    // 发射模式 【单发】一次一个有冷却时间 【连发】一次发多个 【自动】一次一个无冷却时间
    public enum ShootingMode
    {
        Siggle,
        Burst,
        Auto,
    }

    public ShootingMode currentShootingMode;

    [HideInInspector] public bool 外部持续射击;
    [HideInInspector] public bool 外部点击射击;

    private void Awake()
    {
       readyToShoot = true;
       burstBulletsLeft = bulletsPerBurst;
       animator = GetComponent<Animator>();
    }

    void Update()
    {
        bool 鼠标射击 = 平台工具.是移动端 ? false :
            (currentShootingMode == ShootingMode.Auto ? Input.GetKey(KeyCode.Mouse0) : Input.GetKeyDown(KeyCode.Mouse0));

        if(currentShootingMode == ShootingMode.Auto)
        {
            isShooting = 鼠标射击 || 外部持续射击;
        }
        else if(currentShootingMode == ShootingMode.Siggle||currentShootingMode == ShootingMode.Burst)
        {
            isShooting = 鼠标射击 || 外部点击射击;
            外部点击射击 = false;
        }
        if(readyToShoot && isShooting)
        {
            burstBulletsLeft = bulletsPerBurst;
            FireWeapon();
        }
    }    
    private void FireWeapon()
    {
        //播放 muzzleEffect
        muzzleEffect.GetComponent<ParticleSystem>().Play();
        //播放开枪动画
        animator.SetTrigger("RECOIL");
        //播放开枪音效
        全局音频.实例.播放手枪开火();
        readyToShoot = false;
        //计算子弹方向
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;
        //发射子弹
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        //设置子弹方向
        bullet.transform.forward = shootingDirection;
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        // 忽略子弹与武器碰撞体的碰撞，避免子弹出生瞬间卡在枪内
        Collider bulletCollider = bullet.GetComponent<Collider>();
        Collider[] weaponColliders = GetComponentsInChildren<Collider>();
        foreach (var col in weaponColliders)
        {
            Physics.IgnoreCollision(bulletCollider, col, true);
        }
        StartCoroutine(RestoreBulletCollision(bulletCollider, weaponColliders));

        //一段时间内毁灭子弹
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));
        //Checking if we are done shooting
        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }
        //Burst Mode
        if(currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)//we already shoot once before this call
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }

    }

    private IEnumerator RestoreBulletCollision(Collider bulletCollider, Collider[] weaponColliders)
    {
        yield return new WaitForSeconds(0.05f);
        if (bulletCollider != null && bulletCollider.gameObject.activeInHierarchy)
        {
            foreach (var col in weaponColliders)
            {
                if (col != null)
                    Physics.IgnoreCollision(bulletCollider, col, false);
            }
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        Vector3 baseDirection = Camera.main.transform.forward.normalized;

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        Vector3 right = Vector3.Cross(baseDirection, Vector3.up).normalized;
        Vector3 up = Vector3.Cross(right, baseDirection).normalized;

        return (baseDirection + right * x + up * y).normalized;
    } 

    //协程，定义多久时间后毁灭子弹
    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float lifeTime)//对象物 时间
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(bullet);
    }

}