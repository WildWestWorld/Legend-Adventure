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

    private SpriteRenderer spriteRenderer;

    //存储人物的向量
    public Vector2 inputDirection;

    [Header("基础参数")]
    //人物的行动速度
    public float speed;

    //跳跃力度
    public float jumpForce;

    //生命周期
    private void Awake()
    {
        //初始化人物
        inputControl = new PlayerInputControl();
        //初始化人物刚体
        rb = GetComponent<Rigidbody2D>();
        //人物的素材渲染
        spriteRenderer = GetComponent<SpriteRenderer>();


        // +=注册事件
        // 按下 跳跃键 会触发跳跃事件
        inputControl.Player.Jump.started += Jump;
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
    }

    //物理移动的实时更新
    private void FixedUpdate()
    {
        Move();
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

    public void Move()
    {

        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);

        //获取当前角色scale /朝向
        int faceDir = (int)transform.localScale.x;

        //利用scale进行 角色的转向
        if (inputDirection.x > 0)
        {
            faceDir = 1;
        }

        if (inputDirection.x < 0)
        {
            faceDir = -1;
        }

        //设置人物朝向
        transform.localScale = new Vector3(faceDir, 1, 1);
    }


    //注册函数 有固定格式，让他帮我们生成就行
    private void Jump(InputAction.CallbackContext context)
    {
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

}
