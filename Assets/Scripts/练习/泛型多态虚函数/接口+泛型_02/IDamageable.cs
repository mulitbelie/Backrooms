using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace 接口泛型{
public interface IDamageable<T>
{
    void Damage(T damageInfo);
}
}
