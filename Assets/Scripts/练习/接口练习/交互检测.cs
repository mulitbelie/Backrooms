using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 交互检测_练习 : MonoBehaviour
{
    private bool hasLoggedMiss;

    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 10f))
        {
            hasLoggedMiss = false;
            IInteractable_物品 interactTarget = hit.collider.GetComponentInParent<IInteractable_物品>();
            if (Input.GetKeyDown(KeyCode.E) && interactTarget != null)
            {
                interactTarget.Interact();
            }
        }
        else if (!hasLoggedMiss)//（重置标记，下次 miss 可以再打印）
        {
            hasLoggedMiss = true;
            Debug.Log("射线未命中任何物体");
        }
    }
}