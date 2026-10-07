using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoManager : MonoBehaviour
{
    public static AmmoManager Instance {get; private set; }

    [Header("UI")]
    public TextMeshProUGUI ammoDisplay;
    public TextMeshProUGUI magazineDisplay;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else{
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
    }
}