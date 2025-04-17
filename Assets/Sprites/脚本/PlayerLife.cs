using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{
    private Rigidbody2D rb; // 刚体组件，用于控制物理运动
    private Animator anim; // 动画控制器，用于播放动画
    private CharacterHealth characterHealth; // 引用血量管理组件
    private bool isHit = false; // 是否处于受伤状态
    private float hitDirection; // 受伤时的移动方向
    private float hitTime = 0.5f; // 受伤后移动的时间
    private float hitTimer = 0f; // 受伤状态计时器

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        characterHealth = GetComponent<CharacterHealth>(); // 获取血量管理组件
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("陷阱"))
        {
            characterHealth.TakeDamage(10); // 受到陷阱伤害，假设每次陷阱造成 10 点伤害
        }
    }

    void Update()
    {
        // 检查血量是否为零
        if (characterHealth.currentHealth <= 0)
        {
            Die();
        }

        // 如果处于受伤状态，执行受伤后的移动逻辑
        if (isHit)
        {
            hitTimer += Time.deltaTime;
            if (hitTimer < hitTime)
            {
                // 向相反方向移动
                rb.velocity = new Vector2(hitDirection * 5f, rb.velocity.y); // 5f 是移动速度
            }
            else
            {
                // 受伤状态结束
                isHit = false;
                rb.velocity = Vector2.zero; // 停止移动
            }
        }
    }

    // 当玩家受到伤害时调用
    public void OnDamageTaken()
    {
        anim.SetTrigger("Hit"); // 播放受伤动画
        isHit = true; // 设置为受伤状态
        hitTimer = 0f; // 重置计时器

        // 获取玩家当前的移动方向并取反
        float moveDirection = Input.GetAxisRaw("Horizontal"); // 假设使用水平轴控制移动
        hitDirection = -Mathf.Sign(moveDirection); // 取反方向
    }

    // 玩家死亡逻辑
    private void Die()
    {
        rb.bodyType = RigidbodyType2D.Static; // 停止物理运动
        anim.SetTrigger("death"); // 播放死亡动画
        StartCoroutine(RestartLevelAfterDelay(2.0f)); // 2 秒后重启关卡
    }

    // 延迟重启关卡
    private IEnumerator RestartLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 等待指定时间
        RestartLevel();
    }

    // 重启当前关卡
    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}