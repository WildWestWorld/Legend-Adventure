using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//必填参数
[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(PhysicsCheck))]
public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb;

    // HideInInspector 可以隐藏该变量在 Unity Inspector 中显示，但仍然是 public 的
    [HideInInspector] public Animator anim;
    [HideInInspector] public PhysicsCheck physicsCheck; // 引用 PhysicsCheck 组件，用于碰撞检测

    [Header("移动设置")] public float normalSpeed; // 敌人正常的巡逻速度
    public float chaseSpeed; // 敌人追击时的速度
    public float currentSpeed; // 当前的移动速度，依据状态变化（巡逻或追击）
    public Vector3 faceDir; // 敌人面朝的方向（用于移动和攻击）

    public Transform attacker; // 记录攻击者的引用（用于受伤处理）

    public float hurtForce; // 受伤时的击退力

    [Header("检测设置")]
    // 中心偏移量，用于计算检测区域的位置
    public Vector2 centerOffset;

    // 检测区域的大小
    public Vector2 checkSize;

    // 检测距离
    public float checkDistance;

    // 攻击层的 LayerMask，用于检测敌人能被攻击的目标
    public LayerMask attackLayer;

    [Header("AI状态")] public float waitTime; // 等待时间，敌人在巡逻中需要等待的时间
    public float waitTimeCounter; // 等待时间计时器
    public bool wait; // 是否处于等待状态

    // 失去目标时间，敌人丢失玩家后会进入该状态一段时间
    public float lostTime;

    // 失去目标计时器
    public float lostTimeCounter;

    [Header("健康和状态")] public bool isHurt; // 是否正在受伤
    public bool isDead; // 是否已经死亡

    private BaseState currentState;
    protected BaseState patrolState;
    protected BaseState chaseState;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>(); // 获取 PhysicsCheck 组件

        currentSpeed = normalSpeed;
        waitTimeCounter = waitTime;
    }

    private void OnEnable()
    {
        currentState = patrolState;
        currentState.OnEnter(this);
    }

    private void Update()
    {
        faceDir = new Vector3(-transform.localScale.x, 0, 0); // 更新敌人朝向（通过本地缩放判断）
        currentState.LogicUpdate();

        TimeCounter(); // 更新时间

        // 检查是否需要转向（比如碰到墙壁）
        // CheckTurn(); // 如果需要敌人碰到墙壁转向，可以启用此方法
    }

    private void FixedUpdate()
    {
        // 只有在没有受伤、没有死亡、且没有在等待时才进行移动
        if (!isHurt && !isDead && !wait)
        {
            Move();
        }
    }

    private void OnDisable()
    {
        currentState.OnExit();
    }

    public virtual void Move()
    {
        // 根据当前速度和面朝方向来移动敌人
        rb.velocity = new Vector2(currentSpeed * faceDir.x * Time.deltaTime, rb.velocity.y);
    }

    private void CheckTurn()
    {
        // 检查敌人是否碰到墙壁，需要转向
        if (physicsCheck.touchLeftWall)
        {
            TurnAround(-1); // 碰到左墙时，转向左边
        }
        else if (physicsCheck.touchRightWall)
        {
            TurnAround(1); // 碰到右墙时，转向右边
        }
    }

    private void TurnAround(int direction)
    {
        // 通过修改本地缩放的 x 值来翻转敌人的朝向
        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Abs(localScale.x) * direction;
        transform.localScale = localScale;
    }

    public void TimeCounter()
    {
        if (wait)
        {
            waitTimeCounter -= Time.deltaTime; // 等待时间倒计时
            if (waitTimeCounter <= 0)
            {
                wait = false; // 结束等待状态
                waitTimeCounter = waitTime; // 重置等待时间计时器
                transform.localScale = new Vector3(faceDir.x, 1, 1); // 重置朝向
            }
        }

        // 如果敌人没有找到玩家且丢失目标时间还剩余，减小丢失时间计时器
        if (!FoundPlayer() && lostTimeCounter > 0)
        {
            lostTimeCounter -= Time.deltaTime;
        }
    }

    // 检测玩家是否在敌人的视野范围内
    public bool FoundPlayer()
    {
        return Physics2D.BoxCast(transform.position + (Vector3)centerOffset, checkSize, 0, faceDir, checkDistance,
            attackLayer);
    }

    // 切换状态（巡逻或追击）
    public void SwitchState(NPCState state)
    {
        var newState = state switch
        {
            NPCState.Patrol => patrolState,
            NPCState.Chase => chaseState,
            _ => null
        };
        currentState.OnExit(); // 退出当前状态
        currentState = newState;
        currentState.OnEnter(this); // 进入新状态
    }

    #region 伤害处理

    // 处理敌人受到攻击
    public void OnTackDamage(Transform attackTrans)
    {
        attacker = attackTrans;

        // 判断攻击者的位置，决定敌人翻转方向
        if (attacker.position.x - transform.position.x > 0)
        {
            // 攻击者在右边，敌人转向右边
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            // 攻击者在左边，敌人转向左边
            transform.localScale = new Vector3(1, 1, 1);
        }

        // 触发受伤动画，应用击退效果
        isHurt = true;
        anim.SetTrigger("hurt");

        // 计算击退的方向
        Vector2 dir = new Vector2(transform.position.x - attackTrans.position.x, 0).normalized;
        rb.velocity = new Vector2(0, rb.velocity.y); // 暂停敌人的水平移动
        StartCoroutine(OnHurt(dir));
    }

    private IEnumerator OnHurt(Vector2 dir)
    {
        // 在受伤时应用击退力
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f); // 等待0.5秒再结束受伤状态
        isHurt = false;
    }

    // 处理敌人死亡
    public void onDie()
    {
        // 将敌人设置为 "死亡" 图层（可以用于视觉效果或碰撞处理）
        gameObject.layer = 2;

        anim.SetBool("dead", true); // 触发死亡动画
        isDead = true; // 设置敌人已死亡
    }

    // 在死亡动画结束后销毁敌人对象
    public void DestoryAfterAnimation()
    {
        Destroy(this.gameObject);
    }

    #endregion

    // 在 Unity 编辑器中显示敌人的检测区域，用于调试
    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position + (Vector3)centerOffset + new Vector3(checkDistance * -transform.localScale.x, 0), 0.2f);
    }
}