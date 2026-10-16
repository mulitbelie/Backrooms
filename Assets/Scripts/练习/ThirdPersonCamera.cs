using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("相机设置")]
    public Transform target;
    public float mouseSensitivity = 100f;
    public float camDistance = 5f;
    public float camHeight = 1.5f;

    private float rotX;
    private float rotY;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rotY = target.eulerAngles.y;
    }

    void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        rotY += mouseX;
        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, -30f, 60f);

        Quaternion rot = Quaternion.Euler(rotX, rotY, 0);
        Vector3 camPos = target.position - rot * Vector3.forward * camDistance + Vector3.up * camHeight;
        transform.position = camPos;
        transform.LookAt(target);
    }
}
