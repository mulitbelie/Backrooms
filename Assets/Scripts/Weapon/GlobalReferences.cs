using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GlobalReferences : MonoBehaviour
{
    // Start is called before the first frame update
    public static GlobalReferences instance {get; set; }
    // 【原有】默认兜底特效，完全不动
    public GameObject bulletImpactEffectPrefab;

     // 【新增】Layer → 通用特效映射（批量物体，比如水泥/金属/木头）
    [System.Serializable]
    public class LayerImpactEntry
    {
        [Tooltip("目标Layer索引（比如 7=Wall、8=Metal、9=Wood）")]
        public int layer;
        [Tooltip("这个Layer下所有物体的通用特效Prefab")]
        public GameObject effectPrefab;
    }

    [Header("Layer通用特效映射（批量物体）")]
    public List<LayerImpactEntry> impactsByLayer = new List<LayerImpactEntry>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }


    // ========== 新增 2 个查询方法 ==========
    /// <summary>
    /// 查 Layer 对应的通用特效，没配返回 null（Bullet 会用默认）
    /// </summary>
    public GameObject GetImpactByLayer(int layer)
    {
        foreach (var entry in impactsByLayer)
        {
            if (entry.layer == layer) return entry.effectPrefab;
        }
        return null;
    }

    /// <summary>
    /// 兜底特效（给 Bullet 用）
    /// </summary>
    public GameObject DefaultImpact => bulletImpactEffectPrefab;
}