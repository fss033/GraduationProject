using UnityEngine;
using System.Collections;

public class CharacterHealth : MonoBehaviour
{
    public int maxHealth = 60; // 最大血量
    public int currentHealth; // 当前血量
    public bool isInvincible = false; // 是否处于无敌状态
    public float invincibilityDuration = 1.0f; // 无敌时间

    private PlayerLife playerLife; // 引用玩家生命管理组件

    void Start()
    {
        currentHealth = maxHealth; // 初始化血量
        playerLife = GetComponent<PlayerLife>(); // 获取玩家生命管理组件
    }

    // 受到伤害
    public void TakeDamage(int damage)
    {
        if (isInvincible) return; // 如果处于无敌状态，则不受伤害

        currentHealth -= damage; // 减少血量
        if (currentHealth <= 0)
        {
            Die(); // 如果血量小于等于零，触发死亡
        }
        else
        {
            playerLife.OnDamageTaken(); // 调用受伤逻辑
        }
        StartCoroutine(InvincibilityCoroutine()); // 进入无敌状态
    }

    // 治疗
    public void Heal(int amount)
    {
        currentHealth += amount; // 增加血量
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth; // 血量不超过最大值
        }
    }

    // 无敌状态协程
    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true; // 设置为无敌状态
        yield return new WaitForSeconds(invincibilityDuration); // 等待无敌时间结束
        isInvincible = false; // 取消无敌状态
    }

    // 死亡逻辑
    private void Die()
    {
        Debug.Log("角色死亡");
        // 在这里可以添加更多死亡逻辑，比如播放死亡动画、销毁对象等
    }
}