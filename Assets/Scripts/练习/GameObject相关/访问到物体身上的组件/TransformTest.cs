using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformTest : MonoBehaviour
{
    public Transform trans;
    // Start is called before the first frame update
    void Start()
    {
        //位置信息
        Vector3 position = trans.position;//获取trans物体的世界坐标
        Quaternion rotation = trans.rotation;//获取trans物体的旋转信息
        Vector3 roteV3 = rotation.eulerAngles;//获取trans物体的旋转角度
        Vector3 scale = trans.localScale;//获取trans物体的缩放比例

        //除了trans.position外，其它的都可以直接赋值 x y z
        trans.position = new Vector3(1, 1, 1);//将trans物体移动到(1,1,1)位置
        trans.eulerAngles = new Vector3(0, 90, 0);//设置trans物体的旋转角度为90度，x轴为0度，y轴为90度，z轴为0度
        trans.localScale = new Vector3(2, 2, 2);//设置trans物体的缩放比例为2，x轴为2，y轴为2，z轴为2
        trans.position = new Vector3(1, 1, 1);//将trans物体移动到(1,1,1)位置

        //trans可以直接代表整个物体
        this.transform.parent = trans;//将trans物体设置为this物体的父物体
        this.transform.GetChild(0).gameObject.SetActive(true);//将trans物体的子物体设置为激活状态
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
