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

    private SpriteRenderer spriteRenderer;

    //�洢���������
    public Vector2 inputDirection;

    [Header("��������")]
    //������ж��ٶ�
    public float speed;

    //��Ծ����
    public float jumpForce;

    //��������
    private void Awake()
    {
        //��ʼ������
        inputControl = new PlayerInputControl();
        //��ʼ���������
        rb = GetComponent<Rigidbody2D>();
        //������ز���Ⱦ
        spriteRenderer = GetComponent<SpriteRenderer>();


        // +=ע���¼�
        // ���� ��Ծ�� �ᴥ����Ծ�¼�
        inputControl.Player.Jump.started += Jump;
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
    }

    //�����ƶ���ʵʱ����
    private void FixedUpdate()
    {
        Move();
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

    public void Move()
    {

        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);

        //��ȡ��ǰ��ɫscale /����
        int faceDir = (int)transform.localScale.x;

        //����scale���� ��ɫ��ת��
        if (inputDirection.x > 0)
        {
            faceDir = 1;
        }

        if (inputDirection.x < 0)
        {
            faceDir = -1;
        }

        //�������ﳯ��
        transform.localScale = new Vector3(faceDir, 1, 1);
    }


    //ע�ắ�� �й̶���ʽ���������������ɾ���
    private void Jump(InputAction.CallbackContext context)
    {
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

}
