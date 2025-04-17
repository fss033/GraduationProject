using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{
    private Rigidbody2D rb; // ������������ڿ��������˶�
    private Animator anim; // ���������������ڲ��Ŷ���
    private CharacterHealth characterHealth; // ����Ѫ���������
    private bool isHit = false; // �Ƿ�������״̬
    private float hitDirection; // ����ʱ���ƶ�����
    private float hitTime = 0.5f; // ���˺��ƶ���ʱ��
    private float hitTimer = 0f; // ����״̬��ʱ��

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        characterHealth = GetComponent<CharacterHealth>(); // ��ȡѪ���������
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("����"))
        {
            characterHealth.TakeDamage(10); // �ܵ������˺�������ÿ��������� 10 ���˺�
        }
    }

    void Update()
    {
        // ���Ѫ���Ƿ�Ϊ��
        if (characterHealth.currentHealth <= 0)
        {
            Die();
        }

        // �����������״̬��ִ�����˺���ƶ��߼�
        if (isHit)
        {
            hitTimer += Time.deltaTime;
            if (hitTimer < hitTime)
            {
                // ���෴�����ƶ�
                rb.velocity = new Vector2(hitDirection * 5f, rb.velocity.y); // 5f ���ƶ��ٶ�
            }
            else
            {
                // ����״̬����
                isHit = false;
                rb.velocity = Vector2.zero; // ֹͣ�ƶ�
            }
        }
    }

    // ������ܵ��˺�ʱ����
    public void OnDamageTaken()
    {
        anim.SetTrigger("Hit"); // �������˶���
        isHit = true; // ����Ϊ����״̬
        hitTimer = 0f; // ���ü�ʱ��

        // ��ȡ��ҵ�ǰ���ƶ�����ȡ��
        float moveDirection = Input.GetAxisRaw("Horizontal"); // ����ʹ��ˮƽ������ƶ�
        hitDirection = -Mathf.Sign(moveDirection); // ȡ������
    }

    // ��������߼�
    private void Die()
    {
        rb.bodyType = RigidbodyType2D.Static; // ֹͣ�����˶�
        anim.SetTrigger("death"); // ������������
        StartCoroutine(RestartLevelAfterDelay(2.0f)); // 2 ��������ؿ�
    }

    // �ӳ������ؿ�
    private IEnumerator RestartLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // �ȴ�ָ��ʱ��
        RestartLevel();
    }

    // ������ǰ�ؿ�
    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}