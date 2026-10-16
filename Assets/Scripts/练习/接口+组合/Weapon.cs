using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Combat;

namespace MyGame.Weapon
{
    public class Weapon : MonoBehaviour
    {
        public int damage = 15;
        private void OnTriggerEnter(Collider other)//other是个碰撞对象，当碰撞对象进入触发器时调用
        {
            if(other.TryGetComponent(out ITakeDamage target))
            //如果碰撞对象有ITakeDamage组件，就把它塞到target变量里out输出参数
            {
                
                Debug.Log($"{gameObject.name}攻击{other.name}，{damage}点伤害");
                target.TakeDamage(damage);

            }
        }
    }
   

}
