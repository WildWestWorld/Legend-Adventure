using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("��������")]
    //�������ֵ
    public float maxHealth;
    //��ǰ����ֵ
    public float currentHealth;


    [Header("�����޵�")]
    //�޵�ʱ��
    public float invulnerableDuration;
    //�޵�ʱ�䵹��ʱ
    public float invulnerableCounter;
    //�Ƿ�����
    public bool invulnerable;

    //�յ��˺����¼��ļ���
    public UnityEvent<Transform> onTakeDamage;

    //����������״̬
    public UnityEvent onDie;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        //���������״̬
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

        // �����ظ������˺�
        if (currentHealth <= 0)
            return;

        if (currentHealth - attacker.damage > 0)
        {
            currentHealth -= attacker.damage;
            TriggerInvulnerable();
            //ִ������
            onTakeDamage?.Invoke(attacker.transform);
        }
        else
        {
            currentHealth = 0;
            // ��������
            onDie?.Invoke();
        }

        Debug.Log(attacker.damage);
    }


    //����
    private void TriggerInvulnerable()
    {

        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }

}
