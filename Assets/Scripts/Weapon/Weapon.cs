using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon : MonoBehaviour
{
    [Header("枪的显示与UI")]
    public GameObject 手上枪模型;
    public string 枪Tag = "Weapon";
    public int weaponDamage;

    //public Camera playerCamera;
    //射击
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    // 每次射击的子弹数
    public int bulletsPerBurst = 1;
    // 剩余连发子弹数
    public int burstBulletsLeft;
    // 射击散射强度
    [Header("射击散射强度")]
    public float hipSpreadIntensity;
    // ADS模式下射击散射强度
    public float adsSpreadIntensity;
    private float spreadIntensity;
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

    // 是否是ADS模式
    bool isADS;
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

    [Header("弹夹设置")]
    public string magTag = "Magazine";

    public event Action<bool> OnPickupStateChanged;//定义事件：这个事件可以接收一个bool参数

    [Header("瞄准镜")]
    public ReticleT reticleT;

    private bool 已拾取 = false;
    private Inventory 背包;
    private bool 已订阅背包 = false;
    private Magazine 当前消耗的弹夹;

    private void Awake()
    {
       if (reticleT == null) reticleT = FindObjectOfType<ReticleT>();

       // 初始化是否可以射击
       readyToShoot = true;
       // 初始化连发子弹数
       burstBulletsLeft = bulletsPerBurst;

       if (手上枪模型 != null)
       {
           animator = 手上枪模型.GetComponent<Animator>();
           if (animator == null) animator = 手上枪模型.GetComponentInChildren<Animator>();
       }
       if (animator == null) animator = GetComponent<Animator>();

       bulletsLeft = magazineSize;
       spreadIntensity = hipSpreadIntensity;

       if (手上枪模型 != null)
           手上枪模型.SetActive(false);
    }

    void Update()
    {
        if (!已订阅背包)
        {
            背包 = GetComponent<Inventory>();
            if (背包 == null) 背包 = GetComponentInParent<Inventory>();
            if (背包 == null) 背包 = transform.root.GetComponentInChildren<Inventory>();
            if (背包 != null)
            {
                背包.ItemAdded += OnItemAdded;
                背包.ItemRemoved += OnItemRemoved;
                已订阅背包 = true;
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
        // ADS模式
        if(已拾取){ 
            if(Input.GetMouseButtonDown(1))
            {
                EnterADS();
            }
            if(Input.GetMouseButtonUp(1))
            {
                ExitADS();
            }
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
            AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft/bulletsPerBurst}/{magazineSize/bulletsPerBurst}";
        }
        if(AmmoManager.Instance.magazineDisplay != null)
        {
            AmmoManager.Instance.magazineDisplay.text = CountMagazinesInInventory().ToString();
        }

        if (reticleT != null)
        {
            reticleT.SetBaseSpread(spreadIntensity * 2f);
        }

    }    

    private void EnterADS()
        {
            animator.SetTrigger("enterADS");//触发 进入ADS模式动画
            isADS = true;
            spreadIntensity = adsSpreadIntensity;//ADS模式下射击散射强度增加
        }
    private void ExitADS()
        {
            animator.SetTrigger("exitADS");//触发 退出ADS模式动画
            isADS = false;
            spreadIntensity = hipSpreadIntensity;//非ADS模式下射击散射强度减少
        }

    private void FireWeapon()
    {
        bulletsLeft--;
        //播放 muzzleEffect
        muzzleEffect.GetComponent<ParticleSystem>().Play();
        //播放开枪动画
        if(isADS)
        {
            //ADS模式下开枪动画
            animator.SetTrigger("RECOIL_ADS");
        }
        else
        {
            //非ADS模式下开枪动画
            animator.SetTrigger("RECOIL");
        }
        
        //播放开枪音效
        全局音频.实例.播放手枪开火();
        readyToShoot = false;

        //开火准星张开
        reticleT?.AddSpread();
        //计算子弹方向
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;
        //发射子弹
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        //bulletDamage设置子弹伤害 传给子弹组件的bulletDamage属性
        Bullet bul = bullet.GetComponent<Bullet>();//获取子弹组件
        bul.bulletDamage = weaponDamage;//设置子弹伤害为武器伤害


        //设置子弹方向
        bullet.transform.forward = shootingDirection;
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        // 忽略子弹与武器碰撞体的碰撞，避免子弹出生瞬间卡在枪内
        Collider bulletCollider = bullet.GetComponent<Collider>();
        Collider[] weaponColliders = 手上枪模型 != null
            ? 手上枪模型.GetComponentsInChildren<Collider>()
            : GetComponentsInChildren<Collider>();
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

    private Magazine FindMagazineInInventory()
    {
        if (背包 == null) return null;
        for (int i = 0; i < 背包.Count; i++)
        {
            var item = 背包[i];
            var mb = item as MonoBehaviour;
            if (mb != null && mb.CompareTag(magTag))
            {
                var mag = mb.GetComponent<Magazine>();
                if (mag != null) return mag;
            }
        }
        return null;
    }

    private int CountMagazinesInInventory()
    {
        if (背包 == null) return 0;
        int count = 0;
        for (int i = 0; i < 背包.Count; i++)
        {
            var item = 背包[i];
            var mb = item as MonoBehaviour;
            if (mb != null && mb.CompareTag(magTag) && mb.GetComponent<Magazine>() != null)
                count++;
        }
        return count;
    }

    private void Reload()
    {
        当前消耗的弹夹 = FindMagazineInInventory();
        if (当前消耗的弹夹 == null) return;
        全局音频.实例.播放手枪换弹夹();
        isReloading = true;
        Invoke("ReloadComplete", reloadTime);
    }

    private void ReloadComplete()
    {
        if (当前消耗的弹夹 != null && 背包 != null)
        {
            bulletsLeft = Mathf.Min(当前消耗的弹夹.ammoCount, magazineSize);
            背包.DestroyItem(当前消耗的弹夹);
            当前消耗的弹夹 = null;
        }
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

    //根据相机方向和随机射线，计算子弹方向
    public Vector3 CalculateDirectionAndSpread()
    {
        Vector3 baseDirection = Camera.main.transform.forward.normalized;

        float radius = UnityEngine.Random.Range(0f, spreadIntensity);
        float angle = UnityEngine.Random.Range(0f, 360f);
        float offsetX = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        float offsetY = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

        Vector3 right = Vector3.Cross(baseDirection, Vector3.up).normalized;
        Vector3 up = Vector3.Cross(right, baseDirection).normalized;

        return (baseDirection + right * offsetX + up * offsetY).normalized;
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

    //拾取武器
    void OnItemAdded(object sender, InventoryEventArgs e)
    {
        if (已拾取) return; //如果已经拾取了武器，就不执行了
        //做类型过滤 + 安全转换，只在 e.Item 是 MonoBehaviour 脚本组件时，才继续跑后面的代码；不是就直接退出，防止后面代码报错。
        var mb = e.Item as MonoBehaviour; //拿到 e.Item，尝试转成 MonoBehaviour 脚本；如果失败，就直接返回
        if (mb == null) return;

        if (mb.CompareTag(枪Tag))
        {
            已拾取 = true;
            if (手上枪模型 != null)
                手上枪模型.SetActive(true);
            reticleT?.SetWeaponHeld(true); //调用准星方法 SetWeaponHeld 
            OnPickupStateChanged?.Invoke(true);//Invoke()：按下电台的广播按钮，把信号发出去，通知所有监听（订阅）的脚本 ：拾取状态变成 true（可以拾取）
        }
    }
    
    //放下武器
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
            reticleT?.SetWeaponHeld(false); //不调用准星方法 SetWeaponHeld 
            OnPickupStateChanged?.Invoke(false);
        }
    }

    public bool HasWeapon()
    {
        return 已拾取;
    }
}