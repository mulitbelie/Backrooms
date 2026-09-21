using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float openAngle = 90f;//门打开的角度(欧拉角度y)
    public float closeAngle = 0f;//门关闭的角度(欧拉角度y)
    public float smoothSpeed = 0.5f;//平滑速度3D

    private Quaternion openRotation;//门打开的旋转角度(四元数)
    private Quaternion closeRotation;//门关闭的旋转角度(四元数)
    public bool isOpen = false;//是否打开
   
    // Start is called before the first frame update
    void Start()
    {
        openRotation = Quaternion.Euler(0f, openAngle, 0f);//将开门欧拉角度转换为四元数
        closeRotation = Quaternion.Euler(0f, closeAngle, 0f);//将关门欧拉角度转换为四元数
        //添加碰撞器并设置为触发器
        gameObject.AddComponent<SphereCollider>().isTrigger = true;
        gameObject.tag = "Door";
    }
    void Update()//改变门的旋转角度
    {
        if (isOpen&&Quaternion.Angle(transform.localRotation,openRotation)>0.1f)//如果当前门是开门状态 并且没有完全打开，执行开门动画
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, openRotation, smoothSpeed * Time.deltaTime);//平滑旋转到开门角度
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, closeRotation, smoothSpeed * Time.deltaTime);//平滑旋转到关门角度
        }
    }

    // Update is called once per frame
    public void ToogleDoor()//切换门状态 (打开/关闭)
    {
        isOpen = !isOpen;//取非 切换门状态
    }

}
