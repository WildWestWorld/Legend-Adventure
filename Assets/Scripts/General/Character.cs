using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("基本属性")]
    //最大生命值
    public float maxHealth;
    //当前生命值
    public float currentHealth;


    [Header("受伤无敌")]
    //无敌时间
    public float invulnerableDuration;
    //无敌时间倒计时
    private float invulnerableCounter;
    //是否受伤
    public bool invulnerable;

    //收到伤害后事件的集合
    public UnityEvent<Transform> onTakeDamage;

    //人物死亡的状态
    public UnityEvent onDie;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        //如果是受伤状态
        if (invulnerable)
        {
            invulnerableCounter = invulnerableCounter - Time.deltaTime;
            if(invulnerableCounter <= 0)
            {
                invulnerable = false;
            }

        }
    }




    public void TakeDamage(Attack attacker)
    {
        if (invulnerable)
            return;

        // 避免重复触发伤害
        if (currentHealth <= 0)
            return;

        if (currentHealth - attacker.damage > 0)
        {
            currentHealth -= attacker.damage;
            TriggerInvulnerable();
            //执行受伤
            onTakeDamage?.Invoke(attacker.transform);
        }
        else
        {
            currentHealth = 0;
            // 触发死亡
            onDie?.Invoke();
        }

        Debug.Log(attacker.damage);
    }


    //触发
    private void TriggerInvulnerable()
    {

        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }

}
