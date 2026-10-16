using UnityEngine;

namespace MyGame.Combat
{
    ///<summary>
    /// 组件：通用血量组件
    /// </summary>
    public class HealthComponent : MonoBehaviour, ITakeDamage, IDie//，实现接口：继承
    {
        public int maxHp = 100;
        public int _currentHp;
        public bool _isDead;

        private void Awake()//组件初始化时调用比Start早，所以这里初始化血量和是否死亡
        {
            _currentHp = maxHp;
            _isDead = false;
        }

        public void TakeDamage(int damage)
        {
            if (_isDead)return;//直接终止这个函数，后面代码不再执行

            _currentHp -= damage;
            Debug.Log($"{gameObject.name}受到{damage}点伤害，当前血量：{_currentHp}");
            if (_currentHp <= 0)
            {
                _isDead = true;
                Die();
            }
        }

        public void Die()
        {
            if(_isDead)return;//如果死了，直接退出
            _isDead = true;
            Debug.Log($"{gameObject.name}死亡");
            Destroy(gameObject, 2f);//2秒后销毁
        }
        //可选 回血方法，外部可调用
        public void Heal(int value)
        {
            if (_isDead)return;// 死了，直接退出，不能回血
            _currentHp = Mathf.Min(_currentHp + value, maxHp);//**取 a 和 b 里面更小的那个数**
        }
    }
}

