using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    //�˺�
    public int damage;
    //�˺���Χ
    public float attackRange;
    //�˺�����
    public float attackRate;

    private void OnTriggerStay2D(Collider2D otherCollision)
    {

        //TryGetComponent �����᣺
        //���Ի�ȡ���
        //�����ȡ�ɹ������� true�����ҽ������ֵ�� out ����
        //�����ȡʧ�ܣ����� false��out ������Ϊ null
        if (otherCollision.TryGetComponent<Character>(out Character character))
        {
            character.TakeDamage(this);
        }
    }

}
