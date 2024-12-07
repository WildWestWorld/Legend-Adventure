using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //������UnityEngine.InputSystem
    public PlayerInputControl inputControl;

    //�������
    private Rigidbody2D rb;

    //������ײ��
    private CapsuleCollider2D coll;

    //��ײ���
    private PhysicsCheck physicsCheck;

    //����ͼ����Ⱦ
    private SpriteRenderer spriteRenderer;

    //���ﶯ��
    private PlayerAnimation playerAnimation;

    //�洢���������
    public Vector2 inputDirection;

    [Header("��������")]
    //������ж��ٶ�
    public float speed;

    private float runSpeed;
    
    //=>����Ĭ��ֵ
    private float walkSpeed => speed /2.5f;

    //��Ծ����
    public float jumpForce;


    //���˺���������
    public float hurtForce;


    //�Ƿ��¶�
    public bool isCrouch;


    //�¶�״̬ǰԭʼ����ײ�����ĵ����꣬�����¶�״̬�Ļ�ԭ
    private Vector2 originalOffset;

    //�¶�״̬ǰԭʼ����ײ���С�������¶�״̬�Ļ�ԭ
    private Vector2 originalSize;

    [Header("�������")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;



    [Header("״̬")]
    //�Ƿ�����
    public bool isHurt;
    //�Ƿ�����
    public bool isDead;
    //�Ƿ񹥻�
    public bool isAttack;

    //��������
    private void Awake()
    {
        //��ʼ������
        inputControl = new PlayerInputControl();
        //��ʼ���������
        rb = GetComponent<Rigidbody2D>();

        //��ȡ������ײ��
        coll = GetComponent<CapsuleCollider2D>();
        
        //���ﶯ��
        playerAnimation = GetComponent<PlayerAnimation>();
        
        //��ȡԭʼ����ײ�����ĵ�����
        originalOffset = coll.offset;
        //��ȡԭʼ����ײ���С
        originalSize = coll.size;

        //������ز���Ⱦ
        spriteRenderer = GetComponent<SpriteRenderer>();


        //�����ײ���
        physicsCheck = GetComponent<PhysicsCheck>();

        // +=ע���¼�
        // ���� ��Ծ�� �ᴥ����Ծ�¼�
        inputControl.Player.Jump.started += Jump;

        //����
        inputControl.Player.Attack.started += PlayerAttack;

        runSpeed = speed;

        #region ǿ����·
        //performed=��ס
        inputControl.Player.WalkButton.performed += context => {
            if (physicsCheck.isGround)
            {
                speed = walkSpeed;
            };
        };
        //canceled=ȡ������
        inputControl.Player.WalkButton.canceled += context =>
        {
            if (physicsCheck.isGround)
            {
                speed = runSpeed;
            }
        };
        #endregion


    }



    //��unity ��ѡ��������ʱ����
    private void OnEnable()
    {
        inputControl.Enable();
    }
    //��unity ȡ����ѡ��������ʱ����
    private void OnDisable()
    {
        inputControl.Disable();
    }

    //ʵʱ����
    private void Update()
    {
        //��ȡ ���¿������ƶ���ֵ
        inputDirection = inputControl.Player.Move.ReadValue<Vector2>();

        CheckState();
    }

    //�����ƶ���ʵʱ����
    private void FixedUpdate()
    {
        //������˺��Ͳ������ƶ�
        if (!isHurt&&!isAttack)
        {
            Move();
        }
    }


    //������
    //�ƶ�
    public void MoveFilp()
    {
        //����������������
        //�ٶ� = ����*�ٶ�* ʱ��������ʱ������ ���ò�ͬ�ĵ����ƶ���Ч��һ�£�
        //�����y Ӱ����������������������ֺ�ԭ�е�һ��
        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);

        //��ȡ��ǰ��ɫscale /����
        int faceDir = (int)transform.localScale.x;

        //����scale���� ��ɫ��ת��
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

        //�������ﳯ��
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



    //ע�ắ�� �й̶���ʽ���������������ɾ���
    private void Jump(InputAction.CallbackContext context)
    {
        //ֻ���ڵ����ʱ�������Ծ
        if (physicsCheck.isGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    //���﹥��
    private void PlayerAttack(InputAction.CallbackContext context)
    {
        if (physicsCheck.isGround) {
            playerAnimation.PLayerAttack();
            isAttack = true;

        }



    }


    #region UnityEvent
    //��ײ������� ���ﱻ������Ч��
    public void GetHurt(Transform attacker)
    {
        //�ı�����״̬
        isHurt = true;
        //�����ʵ�Ϊ0��ͣ�½�ɫ ��Ϊ����Ҫ�ѽ�ɫ����ȥ��
        rb.velocity = Vector2.zero;
        //���﷽�� ����λ�ò�ֵ���㷽�� =>����λ��-������λ�ã��������������� ����Ǹ��������ұߣ���һ�����Ƿ������߲�ֵ����
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized; 

        //ʩ����
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);


    }
    //��������
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
