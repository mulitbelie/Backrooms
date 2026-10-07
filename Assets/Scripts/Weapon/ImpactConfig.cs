using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent] // 一个物体只能挂一个
public class ImpactConfig : MonoBehaviour
{
    [Header("专属命中特效（纯特效，不管伤害）")]
    [Tooltip("这个物体被击中时的专属特效Prefab（比如丧尸血花、玻璃碎片、木箱碎裂）")]
    public GameObject hitEffectPrefab;
}
