using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTest : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)//触发器进入 XX对象
    {
        Debug.Log("触发器进入：" + other.name);
    }
}
