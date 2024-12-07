using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    // 物体的碰撞体
    private CapsuleCollider2D coll;

    [Header("检测参数")]
    // 是否手动设置中心点
    public bool manual;

    // 人物中心位置的偏移值
    public Vector2 bottomOffset;
    // 左侧检测边缘偏移量
    public Vector2 leftOffset;
    // 右侧检测边缘偏移量
    public Vector2 rightOffset;

    // 检测范围
    public float checkRaduis;

    // 检测后过滤条件
    public LayerMask groundLayer;

    [Header("状态")]
    // 是否在地面
    public bool isGround;
    // 左侧人物边缘监测点
    public bool touchLeftWall;
    // 右侧人物边缘监测点
    public bool touchRightWall;

    private void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();

        if (!manual)
        {
            // 根据公式计算偏移值
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
        // 人物中心点检测
        Vector2 middlePointVec = (Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, 0);


        // 检测地面
        isGround = Physics2D.OverlapCircle(middlePointVec, checkRaduis, groundLayer);

        // 边缘检测
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, checkRaduis, groundLayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, checkRaduis, groundLayer);
    }

    // 根据 localScale.x 动态调整偏移
    private Vector2 AdjustOffset(Vector2 originalOffset)
    {
        // 如果角色翻转（localScale.x < 0），翻转 X 轴的偏移
        if (transform.localScale.x < 0)
        {
            return new Vector2(-originalOffset.x, originalOffset.y);
        }
        return originalOffset;
    }

    // 绘制检测范围辅助线（选中时）
    private void OnDrawGizmosSelected()
    {
        // 动态调整左右检测点偏移
        Vector2 adjustedLeftOffset = AdjustOffset(leftOffset);
        Vector2 adjustedRightOffset = AdjustOffset(rightOffset);

        // 绘制地面检测范围
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, 0), checkRaduis);

        // 绘制左侧检测范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + adjustedLeftOffset, checkRaduis);

        // 绘制右侧检测范围
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere((Vector2)transform.position + adjustedRightOffset, checkRaduis);
    }
}
