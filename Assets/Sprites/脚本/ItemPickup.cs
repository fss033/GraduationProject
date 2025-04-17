using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;  // �����Ʒ����

    [Header("ʰȡ����")]
    public string pickupTag = "������"; // ���Զ���ı�ǩ��
    public float pickupRadius = 1.2f; // ʰȡ�뾶
    public bool autoPickup = true; // �Ƿ��Զ�ʰȡ

    private void Update()
    {
        // �Զ�ʰȡ���
        if (autoPickup)
        {
            CheckForPickup();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ��ײʰȡ���
        if (!autoPickup && other.CompareTag("Player"))
        {
            TryPickup(other.GetComponent<Inventory>());
        }
    }

    private void CheckForPickup()
    {
        // �����Χ�Ƿ��з��ϱ�ǩ����ײ��
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pickupRadius);

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                TryPickup(collider.GetComponent<Inventory>());
                break;
            }
        }
    }

    private void TryPickup(Inventory inventory)
    {
        // ȷ����Ʒ�е������ǩ�ſ�ʰȡ
        if (gameObject.CompareTag(pickupTag) && inventory != null)
        {
            if (inventory.AddItem(item))
            {
                // ʰȡ�ɹ�Ч��
                Debug.Log($"ʰȡ��: {item.itemName}");
                Destroy(gameObject);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}