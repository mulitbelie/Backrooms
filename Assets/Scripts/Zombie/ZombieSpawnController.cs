using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// 僵尸波次生成控制器：负责按波次生成僵尸、检测全部死亡、触发波次间冷却
public class ZombieSpawnController : MonoBehaviour
{
    [Header("波次配置")]
    public int initialZombiesPerWave = 5;  // 第一波的僵尸数量
    public int currentZombiesPerWave;      // 当前波次应生成的僵尸数量（每波翻倍）
    public float spawnDelay = 0.5f;        // 每只僵尸之间的生成间隔（秒）

    [Header("波次状态")]
    public int currentWave = 0;            // 当前波数（从1开始）
    public float waveCooldown = 10.0f;     // 两波之间的冷却时间（秒）

    [Header("冷却状态")]
    public bool inCooldown;                // 当前是否处于波间冷却中
    public float cooldownCounter = 0.0f;   // 冷却剩余时间（用于UI显示）

    [Header("僵尸管理")]
    public List<Zombie> currentZombniesAlive;  // 当前存活（未死亡且未销毁）的僵尸列表
    public GameObject zombiePrefab;             // 僵尸预制体
    [Header("多个出生点")]
    [SerializeField] public List<Transform> spawnPoints;  // 多个出生点（用于随机生成僵尸）

    [Header("UI引用")]
    public TextMeshProUGUI waveOverUI;        // "波次结束"提示UI
    public TextMeshProUGUI cooldownCouterUI;  // 冷却倒计时UI
    public TextMeshProUGUI currentWaveUI;     // 当前波数UI
    public GameObject InputButtonsUI;  // 输入按钮UI

    private void Start()
    {
        // 初始化 当前波生成的僵尸数量（动态更新）=初始僵尸数量5(固定值)，后面会根据波数翻倍生成
        currentZombiesPerWave = initialZombiesPerWave;
        // 初始化 冷却时间（动态更新）= 当前波次的冷却时间是10秒(固定)
        cooldownCounter = waveCooldown;
        // 初始化僵尸列表为空
        currentZombniesAlive = new List<Zombie>();
        // 初始化出生点（自动从场景中获取所有子体组件）
        spawnPoints = new List<Transform>(GetComponentsInChildren<Transform>());
        spawnPoints.Remove(transform);

        // 直接调用 StartNextWave ()，开局立刻启动第一波
        StartNextWave();
    }

    private void Update()
    {
        // 1.[清理名单]：把死掉、已经删掉的僵尸从存活列表踢出去
        //条件：僵尸被销毁`null` 或者 `IsDead=true`
        currentZombniesAlive.RemoveAll(z => z == null || z.IsDead);

        // 2. 判断：场上僵尸全部死光了吗？1所有僵尸都死了 & 2没在冷却中 & 3已经至少打过第 1 波
        //如果满足所有条件，[开启波次冷却]
        if (currentZombniesAlive.Count == 0 && !inCooldown && currentWave > 0)
        {
            // 触发波间冷却协程
            StartCoroutine(WaveCooldown());
        }

        // 3. 如果现在正在冷却，倒计时减时间
        if (inCooldown)
        {
            //冷却状态下倒计时递减；
            cooldownCounter -= Time.deltaTime;
        }
        // 4. 更新UI显示倒计时
        // 用Mathf.Max取最大值，确保倒计时显示为非负数所以始终会>0
        //  F0 表示保留整数部分
        cooldownCouterUI.text = Mathf.Max(0, cooldownCounter).ToString("F0");
    }

    // [方法]开始下一波：增加波数、更新UI、启动生成协程
    private void StartNextWave()
    {
        // 增加波数
        currentWave++;
        // 更新UI显示当前波数
        currentWaveUI.text = "Wave:" + currentWave.ToString();
        // 启动生成协程，开始生成当前波次的僵尸
        StartCoroutine(SpawnWave());
    }

    // [生成协程]逐只生成当前波次所需的僵尸
    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < currentZombiesPerWave; i++)
        {
            //防止出生点列表为空报错
            if (spawnPoints.Count == 0)
            {
                Debug.LogError("没有出生点");
                yield break;
            }
            // 随机选择一个出生点
            Transform randomPoint =  spawnPoints[Random.Range(0, spawnPoints.Count)];
            // 在生成点周围随机偏移，避免僵尸全部重叠
            Vector3 spawnOffset = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f));
            //Vector3 spawnRandomRotation = new Vector3(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
            // 计算僵尸生成位置：当前对象位置 + 随机偏移
            Vector3 spawnPosition = randomPoint.position + spawnOffset;
            // 计算僵尸生成方向：随机角度
            Quaternion spawnRotation = Random.rotation;

            // 实例化僵尸预制体，设置位置和方向
            var zombie = Instantiate(zombiePrefab, spawnPosition, spawnRotation);

            //`enemyScript` = 临时拿到**这一只新刷出来僵尸的 Zombie 脚本**，拿到之后存入列表，用来后续判断生死。
            Zombie enemyScript = zombie.GetComponent<Zombie>();
            // 获取僵尸组件，Add() 添加到存活列表 [currentZombniesAlive]
            currentZombniesAlive.Add(enemyScript);

            // 等待间隔时间后再生成下一只
            //`yield return xxx`：等待真实时间(spawnDelay)
            // yield return new WaitForSeconds(1); 
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    // 【波间冷却协程】：显示"波次结束"UI，等待冷却时间，然后扩大僵尸数量并开始下一波
    private IEnumerator WaveCooldown()
    {
        inCooldown = true;                  // 进入冷却状态
        cooldownCounter = waveCooldown;     // 重置倒计时
        waveOverUI.gameObject.SetActive(true); // 显示"波次结束"UI
        InputButtonsUI.gameObject.SetActive(false); // 隐藏输入按钮UI

        // 等待完整的冷却时长
        // 等待真实时间waveCooldown秒
        yield return new WaitForSeconds(waveCooldown); 

        inCooldown = false;                  // 退出冷却状态
        waveOverUI.gameObject.SetActive(false); // 隐藏"波次结束"UI
        InputButtonsUI.gameObject.SetActive(true); // 显示输入按钮UI

        // 每波僵尸数量翻倍
        currentZombiesPerWave *= 2;

        // 开始下一波
        StartNextWave();
    }
}