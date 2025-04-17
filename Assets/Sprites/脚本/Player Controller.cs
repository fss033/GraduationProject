using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb; // �������
    private BoxCollider2D coll; // ��ײ�����
    private SpriteRenderer sprite; // ������Ⱦ��
    private Animator anim; // ����������
    private int jumpCount; // ��Ծ����
    public CharacterHealth characterHealth; // ��ɫ����ֵ

    [SerializeField] private LayerMask jumpableGround; // ����Ծ����

    private float dirX = 0f; // ˮƽ��������
    [SerializeField] private float moveSpeed = 7f; // �ƶ��ٶ�
    [SerializeField] private float jumpForce = 7f; // ��Ծ����

    private enum MovementState { idle, running, jumping, falling } // ����״̬

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        jumpCount = 0;
    }

    void Update()
    {
        if (!enabled) return;
        dirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        if (Input.GetButtonDown("Jump"))
        {
            if (jumpCount < 1)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpCount++;
            }
        }
        if (IsGrounded())
        {
            jumpCount = 0;
        }

        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        MovementState state = MovementState.idle;

        if (dirX > 0f)
        {
            state = MovementState.running;
            sprite.flipX = false;
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            sprite.flipX = true;
        }

        if (rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -.3f)
        {
            state = MovementState.falling;
        }

        anim.SetInteger("state", (int)state);
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    void OnDisable()
    {
        // ���ű�������ʱ��ֹͣ��ɫ�ƶ�
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }
}