using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //来自于UnityEngine.InputSystem
    public PlayerInputControl inputControl;

    //人物刚体
    private Rigidbody2D rb;

    //人物碰撞体
    private CapsuleCollider2D coll;

    //碰撞检测
    private PhysicsCheck physicsCheck;

    //人物图像渲染
    private SpriteRenderer spriteRenderer;

    //人物动画
    private PlayerAnimation playerAnimation;

    //存储人物的向量
    public Vector2 inputDirection;

    [Header("基础参数")]
    //人物的行动速度
    public float speed;

    private float runSpeed;
    
    //=>设置默认值
    private float walkSpeed => speed /2.5f;

    //跳跃力度
    public float jumpForce;


    //受伤后向后的推力
    public float hurtForce;


    //是否下蹲
    public bool isCrouch;


    //下蹲状态前原始的碰撞体中心点坐标，用于下蹲状态的还原
    private Vector2 originalOffset;

    //下蹲状态前原始的碰撞体大小，用于下蹲状态的还原
    private Vector2 originalSize;

    [Header("物理材质")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;



    [Header("状态")]
    //是否受伤
    public bool isHurt;
    //是否死亡
    public bool isDead;
    //是否攻击
    public bool isAttack;

    //生命周期
    private void Awake()
    {
        //初始化人物
        inputControl = new PlayerInputControl();
        //初始化人物刚体
        rb = GetComponent<Rigidbody2D>();

        //获取人物碰撞体
        coll = GetComponent<CapsuleCollider2D>();
        
        //人物动画
        playerAnimation = GetComponent<PlayerAnimation>();
        
        //获取原始的碰撞体中心点坐标
        originalOffset = coll.offset;
        //获取原始的碰撞体大小
        originalSize = coll.size;

        //人物的素材渲染
        spriteRenderer = GetComponent<SpriteRenderer>();


        //获得碰撞检测
        physicsCheck = GetComponent<PhysicsCheck>();

        // +=注册事件
        // 按下 跳跃键 会触发跳跃事件
        inputControl.Player.Jump.started += Jump;

        //攻击
        inputControl.Player.Attack.started += PlayerAttack;

        runSpeed = speed;

        #region 强制走路
        //performed=按住
        inputControl.Player.WalkButton.performed += context => {
            if (physicsCheck.isGround)
            {
                speed = walkSpeed;
            };
        };
        //canceled=取消按下
        inputControl.Player.WalkButton.canceled += context =>
        {
            if (physicsCheck.isGround)
            {
                speed = runSpeed;
            }
        };
        #endregion


    }



    //在unity 勾选启用人物时触发
    private void OnEnable()
    {
        inputControl.Enable();
    }
    //在unity 取消勾选启用人物时触发
    private void OnDisable()
    {
        inputControl.Disable();
    }

    //实时更新
    private void Update()
    {
        //读取 按下控制器移动的值
        inputDirection = inputControl.Player.Move.ReadValue<Vector2>();

        CheckState();
    }

    //物理移动的实时更新
    private void FixedUpdate()
    {
        //如果受伤害就不让他移动
        if (!isHurt&&!isAttack)
        {
            Move();
        }
    }


    //方法区
    //移动
    public void MoveFilp()
    {
        //更改人物刚体的速率
        //速度 = 方向*速度* 时间修正（时间修正 能让不同的电脑移动的效果一致）
        //这里的y 影响的是重力，我们让他保持和原有的一致
        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);

        //获取当前角色scale /朝向
        int faceDir = (int)transform.localScale.x;

        //利用scale进行 角色的转向
        if (inputDirection.x > 0)
        {
            //faceDir = 1;
            spriteRenderer.flipX = false;
        }

        if (inputDirection.x < 0)
        {
            //faceDir = -1;
            spriteRenderer.flipX = true;

        }

        //设置人物朝向
        //transform.localScale = new Vector3(faceDir, 1, 1);
    }


    // private void OnTriggerStay2D(Collider2D otherCollision)
    // {
    //   Debug.Log(otherCollision.name);
    //}


    public void Move()
    {
        // Check if the character is crouching
        isCrouch = inputDirection.y < -0.5f && physicsCheck.isGround;

        // If crouching, prevent horizontal movement by setting speed to zero temporarily
        float currentSpeed = isCrouch ? 0 : speed;

        // Apply velocity with the adjusted speed
        rb.velocity = new Vector2(inputDirection.x * currentSpeed * Time.deltaTime, rb.velocity.y);

        // Determine character's facing direction
        int faceDir = (int)transform.localScale.x;

        if (inputDirection.x > 0)
        {
            faceDir = 1;
        }
        else if (inputDirection.x < 0)
        {
            faceDir = -1;
        }

        // Set character's facing direction
        transform.localScale = new Vector3(faceDir, 1, 1);

        // If crouching, adjust collider properties
        if (isCrouch)
        {
            coll.offset = new Vector2(-0.05f, 0.85f);
            coll.size = new Vector2(0.7f, 1.7f);
        }
        else
        {
            coll.size = originalSize;
            coll.offset = originalOffset;
        }
    }



    //注册函数 有固定格式，让他帮我们生成就行
    private void Jump(InputAction.CallbackContext context)
    {
        //只有在地面的时候才能跳跃
        if (physicsCheck.isGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    //人物攻击
    private void PlayerAttack(InputAction.CallbackContext context)
    {
        if (physicsCheck.isGround) {
            playerAnimation.PLayerAttack();
            isAttack = true;

        }



    }


    #region UnityEvent
    //碰撞到怪物后 人物被反弹的效果
    public void GetHurt(Transform attacker)
    {
        //改变受伤状态
        isHurt = true;
        //将速率调为0，停下角色 因为我们要把角色弹回去了
        rb.velocity = Vector2.zero;
        //人物方向 利用位置差值计算方向 =>人物位置-攻击者位置，如果正数就是左边 如果是负数就是右边，归一化就是放着两者差值过大
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized; 

        //施加力
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);


    }
    //人物死亡
    public void PlayerDead()
    {
        isDead = true;
        inputControl.Player.Disable();
    }
    #endregion

    private void CheckState()
    {
        coll.sharedMaterial = physicsCheck.isGround ? normal : wall;
    }
}
