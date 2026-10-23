using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //rotation默认为四元数 eulerAngle默认为欧拉角       
        Debug.Log("随机出欧拉角"+ Random.rotation.eulerAngles);
        Debug.Log("把欧拉角转换成四元数"+ Quaternion.Euler(Random.rotation.eulerAngles));
        Debug.Log("随机出四元数"+ Random.rotation);
        Debug.Log("把四元数转为欧拉角"+ Random.rotation.eulerAngles);

        //在x轴（-1，1）和y轴（-1，1）之间随机出一个Vector2数
        Debug.Log(Random.insideUnitCircle);
        //你把这个 state 存起来，后面赋值回去：`Random.state = 保存的书签;`
        //随机就会从刚才这个位置继续往下出，**复刻一模一样的随机结果**
        Debug.Log("随机出一个状态"+ Random.state);
        
        Debug.Log(Random.Range(0,10));//整形重载随机出的数不包含10
        Debug.Log(Random.Range(0,10.0f));//浮点数重载随机出的数不包含10.0f
    }


}
