using UnityEngine;

public class CameraBob : MonoBehaviour
{
    [Header("脚步Bob参数")]
    [Tooltip("弹跳频率，数值越快脚步越密")]
    public float bobFrequency = 6f;
    [Tooltip("上下幅度，FPS一般0.05~0.08")]
    public float verticalAmplitude = 0.07f;
    [Tooltip("左右摇摆幅度，比上下小很多，0.02~0.04")]
    public float horizontalAmplitude = 0.03f;
    [Tooltip("归位平滑速度")]
    public float smoothSpeed = 10f;

    [Header("参考")]
    public 基础移动控制 playerMove;

    private Vector3 originLocalPos;
    private float bobTimer;

    void Start()
    {
        // 记录初始本地位置
        originLocalPos = transform.localPosition;
    }

    void Update()
    {
        bool isMoving = playerMove.IsMoving;

        if (isMoving)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            // 上下：Sin，抬脚-落下
            float y = Mathf.Sin(bobTimer) * verticalAmplitude;
            // 左右：Cos错开相位，左脚右、右脚左，自然交替
            float x = Mathf.Cos(bobTimer) * horizontalAmplitude;

            Vector3 targetPos = originLocalPos + new Vector3(x, y, 0);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, smoothSpeed * Time.deltaTime);
        }
        else
        {
            // 停止移动，平滑回到基准位置，计时器清零
            transform.localPosition = Vector3.Lerp(transform.localPosition, originLocalPos, smoothSpeed * Time.deltaTime);
            bobTimer = 0;
        }
    }
}
