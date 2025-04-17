using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb; // 刚体组件
    private BoxCollider2D coll; // 碰撞体组件
    private SpriteRenderer sprite; // 精灵渲染器
    private Animator anim; // 动画控制器
    private int jumpCount; // 跳跃计数
    public CharacterHealth characterHealth; // 角色生命值

    [SerializeField] private LayerMask jumpableGround; // 可跳跃地面

    private float dirX = 0f; // 水平方向输入
    [SerializeField] private float moveSpeed = 7f; // 移动速度
    [SerializeField] private float jumpForce = 7f; // 跳跃力度

    private enum MovementState { idle, running, jumping, falling } // 控制状态

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
        // 当脚本被禁用时，停止角色移动
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }
}