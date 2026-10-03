using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon : MonoBehaviour
{
    [Header("枪的显示与UI")]
    public GameObject 手上枪模型;
    public string 枪Tag = "Weapon";

    //public Camera playerCamera;
    //射击
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    // 每次射击的子弹数
    public int bulletsPerBurst = 1;
    // 剩余连发子弹数
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

    //换弹时间
    public float reloadTime = 2f;
    // 弹夹容量剩余子弹数
    public int magazineSize, bulletsLeft;
    // 当前是否正在换弹
    public bool isReloading;

    //原始位置
    public Vector3 spawnPosition;
    public Vector3 spawnRotation;



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

    private bool 已拾取 = false;
    private Inventory 背包;
    private bool 已订阅背包 = false;

    private void Awake()
    {
       readyToShoot = true;
       burstBulletsLeft = bulletsPerBurst;

       if (手上枪模型 != null)
       {
           animator = 手上枪模型.GetComponent<Animator>();
           if (animator == null) animator = 手上枪模型.GetComponentInChildren<Animator>();
       }
       if (animator == null) animator = GetComponent<Animator>();

       bulletsLeft = magazineSize;

       if (手上枪模型 != null)
           手上枪模型.SetActive(false);
    }

    void Update()
    {
        if (!已订阅背包)
        {
            背包 = GetComponentInParent<Inventory>();
            if (背包 == null) 背包 = FindObjectOfType<Inventory>();
            if (背包 != null)
            {
                背包.ItemAdded += OnItemAdded;
                背包.ItemRemoved += OnItemRemoved;
                已订阅背包 = true;
                Debug.Log($"[Weapon] Update 订阅背包成功: {背包.name}");
            }
            else if (Time.frameCount % 120 == 0)
            {
                Debug.LogWarning("[Weapon] 还没找到 Inventory...");
            }
        }

        if (!已拾取) return;

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

        
        if(Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloading == false)
        {
            Reload();
        }
        // 当子弹用完时，自动换弹
        //**不在换弹状态 isReloading == false  也可以写成 !isReloading **//
        if(readyToShoot && !isShooting  && !isReloading && bulletsLeft <= 0)
        {
            Reload();
        }
        
        // 玩家尝试射击
        if(readyToShoot && isShooting)
        {
            if(bulletsLeft > 0)
            {
                burstBulletsLeft = bulletsPerBurst;
                FireWeapon();
            }
            // 当子弹用完时，播放空弹音音效
            else if(!isReloading)
            {
                全局音频.实例.播放手枪空弹夹();
                readyToShoot = false;
                Invoke("ResetShot", shootingDelay);
            }
        }
        // 更新子弹显示ui
        if(AmmoManager.Instance.ammoDisplay != null)
        {
            //BulletsLeft 剩余子弹数     MagazineSize 总子弹数     bulletsPerBurst 每次射击的子弹数 除法应为有子弹是连发
            AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft/bulletsPerBurst}/{magazineSize/bulletsPerBurst}";
        }

    }    
    private void FireWeapon()
    {
        bulletsLeft--;
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

    private void Reload()
    {
        // 换弹时，播放换弹夹音效
         全局音频.实例.播放手枪换弹夹();
        // 换弹时，如果当前正在换弹，isReloading = true;
        isReloading = true;
        Invoke("ReloadComplete", reloadTime);
    }

    private void ReloadComplete()
    {
        // 换弹完成，重置子弹容量 为最大容量 isReloading 为 false
        bulletsLeft = magazineSize;
        isReloading = false;
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

    void OnDestroy()
    {
        if (背包 != null)
        {
            背包.ItemAdded -= OnItemAdded;
            背包.ItemRemoved -= OnItemRemoved;
        }
    }

    void OnItemAdded(object sender, InventoryEventArgs e)
    {
        if (已拾取) return;

        var mb = e.Item as MonoBehaviour;
        if (mb == null)
        {
            Debug.Log("[Weapon] OnItemAdded: e.Item 不是 MonoBehaviour");
            return;
        }

        Debug.Log($"[Weapon] OnItemAdded: item={mb.name}, Tag={mb.tag}, 期望Tag={枪Tag}, 匹配={mb.CompareTag(枪Tag)}");

        if (mb.CompareTag(枪Tag))
        {
            已拾取 = true;
            if (手上枪模型 != null)
                手上枪模型.SetActive(true);
            Debug.Log("[Weapon] 成功拾取枪！已拾取=true");
        }
    }

    void OnItemRemoved(object sender, InventoryEventArgs e)
    {
        if (!已拾取) return;

        var mb = e.Item as MonoBehaviour;
        if (mb != null && mb.CompareTag(枪Tag))
        {
            已拾取 = false;
            isReloading = false;
            readyToShoot = true;
            bulletsLeft = magazineSize;
            if (手上枪模型 != null)
                手上枪模型.SetActive(false);
            Debug.Log("[Weapon] 成功丢弃枪！已拾取=false");
        }
    }

    public bool HasWeapon()
    {
        return 已拾取;
    }
}