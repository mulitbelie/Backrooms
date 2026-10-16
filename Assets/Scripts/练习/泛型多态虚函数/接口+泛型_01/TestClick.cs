using UnityEngine;
public class TestClick : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("点击鼠标左键");
        }
    }
}
