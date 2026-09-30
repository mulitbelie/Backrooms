using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalReferences : MonoBehaviour
{
    // Start is called before the first frame update
    public static GlobalReferences instance {get; set; }
    public GameObject bulletImpactEffectPrefab;
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
    }
}