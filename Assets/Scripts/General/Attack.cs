using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    //伤害
    public int damage;
    //伤害范围
    public float attackRange;
    //伤害比率
    public float attackRate;

    private void OnTriggerStay2D(Collider2D otherCollision)
    {

        //TryGetComponent 方法会：
        //尝试获取组件
        //如果获取成功，返回 true，并且将组件赋值给 out 参数
        //如果获取失败，返回 false，out 参数将为 null
        if (otherCollision.TryGetComponent<Character>(out Character character))
        {
            character.TakeDamage(this);
        }
    }

}
