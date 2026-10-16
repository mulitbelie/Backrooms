using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonPlayer : MonoBehaviour
{
    [Header("移动参数")]
    public float moveSpeed = 5f;
    public float rotateSmooth = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.2f;

    [Tooltip("把相机Transform拖进来")]
    public Transform mainCam;

    private CharacterController cc;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = cc.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // WASD输入
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        if (inputDir.magnitude > 0.1f)
        {
            // 获取相机水平朝向（去掉Y轴旋转，防止角色跟着相机上下俯仰倾斜）
            Vector3 camForward = mainCam.forward;
            Vector3 camRight = mainCam.right;
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            // 基于相机方向计算移动方向
            Vector3 moveDir = (camForward * v + camRight * h).normalized;

            // 平滑旋转角色朝向移动方向
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotateSmooth * Time.deltaTime);

            cc.Move(moveDir * moveSpeed * Time.deltaTime);
        }

        // 跳跃
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 重力
        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }
}
