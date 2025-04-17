using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public Sprite icon;
    public int maxStack = 1;
    [HideInInspector] public int currentAmount = 1; // ��ǰ����

    // �Ƿ���Զѵ�
    public bool IsStackable
    {
        get { return maxStack > 1; }
    }
}

public class Inventory : MonoBehaviour
{
    public int slotCount = 3;
    public List<Item> items = new List<Item>();

    // ��Ʒ�仯�¼�
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
        // �ȳ��Զѵ�������Ʒ
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
                    Debug.Log($"�ѵ� {newItem.itemName}�������� {items[i].currentAmount}��");
                    return true;
                }
            }
        }

        // ���û�пɶѵ��ģ��ҿ�λ
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items[i] = newItem;
                items[i].currentAmount = 1;
                OnInventoryChanged?.Invoke();
                Debug.Log($"��� {newItem.itemName} ����λ {i}");
                return true;
            }
        }

        Debug.Log("��Ʒ������");
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