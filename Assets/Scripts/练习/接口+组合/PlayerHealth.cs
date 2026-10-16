using UnityEngine;

namespace MyGame.Combat
{
    ///<summary>
    /// 组件：通用血量组件
    /// </summary>
    public class PlayerHealth : MonoBehaviour, ITakeDamage, IDie//，实现接口：继承
    {
        [Header("玩家血量")]
        public int maxHp = 100;
        public float invincibleTime = 1f; //这是"无敌时长"（暴露参数）
        private int _currentHp;
        private bool _isDead;
        private float _invincibleTime;//这是"当前剩余无敌时间0"（从时长倒减到 0）   

        private void Awake()//组件初始化时调用比Start早，所以这里初始化血量和是否死亡
        {
            _currentHp = maxHp;
            _isDead = false;
        }
        private void Update()
        {
            if (_invincibleTime > 0)//如果无敌无敌时间大于0
            {
                _invincibleTime -= Time.deltaTime;
                // _invincibleTime 赋初值，TakeDamage 受伤时也没触发赋值，
                // 所以 _invincibleTime 永远是 0，倒计时根本不启动。
                if(_invincibleTime <= 0)
                {
                Debug.Log($"玩家无敌时间结束");
                 }
            }
            
            
                
            
        }


        public void TakeDamage(int damage)
        {
            if (_isDead || _invincibleTime > 0)return;//如果死了，或者无敌无敌时间大于0，直接退出
            //因为无敌时间内，不能受到伤害，所以直接退出
            _currentHp -= damage;
            Debug.Log($"玩家受到{damage}点伤害，当前血量：{_currentHp}");

            // 玩家特有逻辑：无敌帧、受伤动画、屏幕后效
            _invincibleTime = invincibleTime;// 受伤瞬间，把"时长"赋给"剩余无敌时间"
            // 伤动画、屏幕后效等
            Debug.Log($"玩家受伤，触发无敌帧");

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
            Debug.Log($"玩家死亡，触发游戏失败");
            // 玩家死亡逻辑：关闭输入、死亡动画、UI弹窗等
        }
        //可选 回血方法，外部可调用
        public void Heal(int value)
        {
            if (_isDead)return;// 死了，直接退出，不能回血
            _currentHp = Mathf.Min(_currentHp + value, maxHp);//**取 a 和 b 里面更小的那个数**
        }
    }
}
