using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    // �������ײ��
    private CapsuleCollider2D coll;

    [Header("������")]
    // �Ƿ��ֶ��������ĵ�
    public bool manual;

    // ��������λ�õ�ƫ��ֵ
    public Vector2 bottomOffset;
    // ������Եƫ����
    public Vector2 leftOffset;
    // �Ҳ����Եƫ����
    public Vector2 rightOffset;

    // ��ⷶΧ
    public float checkRaduis;

    // �����������
    public LayerMask groundLayer;

    [Header("״̬")]
    // �Ƿ��ڵ���
    public bool isGround;
    // ��������Ե����
    public bool touchLeftWall;
    // �Ҳ������Ե����
    public bool touchRightWall;

    private void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();

        if (!manual)
        {
            // ���ݹ�ʽ����ƫ��ֵ
            rightOffset = new Vector2((coll.bounds.size.x) / 2 + coll.offset.x, coll.bounds.size.y / 2);
            leftOffset = new Vector2(-(coll.bounds.size.x) / 2 + coll.offset.x, rightOffset.y);
         
        }
    }

    private void Update()
    {
        Check();
    }

    public void Check()
    {
        // �������ĵ���
        Vector2 middlePointVec = (Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, 0);


        // ������
        isGround = Physics2D.OverlapCircle(middlePointVec, checkRaduis, groundLayer);

        // ��Ե���
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, checkRaduis, groundLayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, checkRaduis, groundLayer);
    }

    // ���� localScale.x ��̬����ƫ��
    private Vector2 AdjustOffset(Vector2 originalOffset)
    {
        // �����ɫ��ת��localScale.x < 0������ת X ���ƫ��
        if (transform.localScale.x < 0)
        {
            return new Vector2(-originalOffset.x, originalOffset.y);
        }
        return originalOffset;
    }

    // ���Ƽ�ⷶΧ�����ߣ�ѡ��ʱ��
    private void OnDrawGizmosSelected()
    {
        // ��̬�������Ҽ���ƫ��
        Vector2 adjustedLeftOffset = AdjustOffset(leftOffset);
        Vector2 adjustedRightOffset = AdjustOffset(rightOffset);

        // ���Ƶ����ⷶΧ
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, 0), checkRaduis);

        // ��������ⷶΧ
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + adjustedLeftOffset, checkRaduis);

        // �����Ҳ��ⷶΧ
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere((Vector2)transform.position + adjustedRightOffset, checkRaduis);
    }
}
