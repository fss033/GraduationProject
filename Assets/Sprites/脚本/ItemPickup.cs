using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;  // 你的物品数据

    [Header("拾取设置")]
    public string pickupTag = "掉落物"; // 可自定义的标签名
    public float pickupRadius = 1.2f; // 拾取半径
    public bool autoPickup = true; // 是否自动拾取

    private void Update()
    {
        // 自动拾取检测
        if (autoPickup)
        {
            CheckForPickup();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 碰撞拾取检测
        if (!autoPickup && other.CompareTag("Player"))
        {
            TryPickup(other.GetComponent<Inventory>());
        }
    }

    private void CheckForPickup()
    {
        // 检测周围是否有符合标签的碰撞体
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
        // 确保物品有掉落物标签才可拾取
        if (gameObject.CompareTag(pickupTag) && inventory != null)
        {
            if (inventory.AddItem(item))
            {
                // 拾取成功效果
                Debug.Log($"拾取了: {item.itemName}");
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