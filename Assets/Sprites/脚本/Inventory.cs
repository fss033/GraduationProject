using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public Sprite icon;
    public int maxStack = 1;
    [HideInInspector] public int currentAmount = 1; // 当前数量

    // 是否可以堆叠
    public bool IsStackable
    {
        get { return maxStack > 1; }
    }
}

public class Inventory : MonoBehaviour
{
    public int slotCount = 3;
    public List<Item> items = new List<Item>();

    // 物品变化事件
    public event System.Action OnInventoryChanged;

    private void Start()
    {
        InitializeInventory();
    }

    private void InitializeInventory()
    {
        items.Clear();
        for (int i = 0; i < slotCount; i++)
        {
            items.Add(null);
        }
    }

    public bool AddItem(Item newItem)
    {
        // 先尝试堆叠已有物品
        if (newItem.IsStackable)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null &&
                    items[i].itemName == newItem.itemName &&
                    items[i].currentAmount < items[i].maxStack)
                {
                    items[i].currentAmount++;
                    OnInventoryChanged?.Invoke();
                    Debug.Log($"堆叠 {newItem.itemName}，现在有 {items[i].currentAmount}个");
                    return true;
                }
            }
        }

        // 如果没有可堆叠的，找空位
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items[i] = newItem;
                items[i].currentAmount = 1;
                OnInventoryChanged?.Invoke();
                Debug.Log($"添加 {newItem.itemName} 到槽位 {i}");
                return true;
            }
        }

        Debug.Log("物品栏已满");
        return false;
    }

    public void RemoveItem(int slotIndex, int amount = 1)
    {
        if (IsValidSlot(slotIndex) && items[slotIndex] != null)
        {
            items[slotIndex].currentAmount -= amount;

            if (items[slotIndex].currentAmount <= 0)
            {
                items[slotIndex] = null;
            }

            OnInventoryChanged?.Invoke();
        }
    }

    public void UseItem(int slotIndex)
    {
        if (IsValidSlot(slotIndex) && items[slotIndex] != null)
        {
            Debug.Log($"Using {items[slotIndex].itemName}");
            RemoveItem(slotIndex);
        }
    }

    private bool IsValidSlot(int index)
    {
        return index >= 0 && index < items.Count;
    }
}