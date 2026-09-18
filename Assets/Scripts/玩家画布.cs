using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 玩家画布 : MonoBehaviour
{   
    public 全局脚本 全局脚本;
    public VariableJoystick 摇杆;

    public 玩家画布 与画布绑定(全局脚本 全局脚本){
        
        this.全局脚本 = 全局脚本;
        return this;
    }



}
