using UnityEngine;
using System.Collections;

public class CharacterHealth : MonoBehaviour
{
    public int maxHealth = 60; // ���Ѫ��
    public int currentHealth; // ��ǰѪ��
    public bool isInvincible = false; // �Ƿ����޵�״̬
    public float invincibilityDuration = 1.0f; // �޵�ʱ��

    private PlayerLife playerLife; // ������������������

    void Start()
    {
        currentHealth = maxHealth; // ��ʼ��Ѫ��
        playerLife = GetComponent<PlayerLife>(); // ��ȡ��������������
    }

    // �ܵ��˺�
    public void TakeDamage(int damage)
    {
        if (isInvincible) return; // ��������޵�״̬�������˺�

        currentHealth -= damage; // ����Ѫ��
        if (currentHealth <= 0)
        {
            Die(); // ���Ѫ��С�ڵ����㣬��������
        }
        else
        {
            playerLife.OnDamageTaken(); // ���������߼�
        }
        StartCoroutine(InvincibilityCoroutine()); // �����޵�״̬
    }

    // ����
    public void Heal(int amount)
    {
        currentHealth += amount; // ����Ѫ��
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth; // Ѫ�����������ֵ
        }
    }

    // �޵�״̬Э��
    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true; // ����Ϊ�޵�״̬
        yield return new WaitForSeconds(invincibilityDuration); // �ȴ��޵�ʱ�����
        isInvincible = false; // ȡ���޵�״̬
    }

    // �����߼�
    private void Die()
    {
        Debug.Log("��ɫ����");
        // �����������Ӹ��������߼������粥���������������ٶ����
    }
}