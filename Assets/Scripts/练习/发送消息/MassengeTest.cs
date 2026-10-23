using UnityEngine;

public class MassengeTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //发送消息给自己
        gameObject.SendMessage("Hello");
        //不报错发送消息
        SendMessage("BBB",SendMessageOptions.DontRequireReceiver);

        //发送消息给所有子物体
        gameObject.BroadcastMessage ("Hello");
        //发送消息给父级物体
        gameObject.SendMessageUpwards("Hello");

    }

    private void Hello()
    {
        Debug.Log("Hello");
    }
}
