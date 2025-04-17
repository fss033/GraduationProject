using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Inventory))]
public class InventoryUI : MonoBehaviour
{
    [Header("必需引用")]
    public Inventory inventory;
    public Image[] slotIcons;          // 物品图标显示
    public TextMeshProUGUI[] amountTexts; // 数量文本 (格式: 当前/最大)
    public GameObject[] slotHighlights; // 选中高亮效果

    [Header("数量显示设置")]
    public Color normalAmountColor = Color.white;
    public Color fullAmountColor = Color.green;
    public Color lowAmountColor = new Color(1, 0.8f, 0); // 浅黄色
    public bool hideAmountWhenSingle = true; // 当数量为1时是否隐藏

    [Header("自动初始化")]
    public bool autoFindSlots = true;
    public string slotNamePrefix = "Slot";
    public string iconName = "Icon";
    public string amountTextName = "AmountText";

    private void OnEnable()
    {
        // 订阅物品变更事件
        if (inventory != null)
        {
            inventory.OnInventoryChanged += UpdateAllSlots;
        }
    }

    private void OnDisable()
    {
        // 取消订阅
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= UpdateAllSlots;
        }
    }

    private void Start()
    {
        // 确保引用正确
        if (inventory == null)
        {
            inventory = GetComponent<Inventory>();
        }

        // 自动初始化UI元素
        if (autoFindSlots)
        {
            InitializeUISlots();
        }

        // 初始更新
        UpdateAllSlots();
    }

    /// <summary>
    /// 自动查找并初始化所有UI槽位
    /// </summary>
    private void InitializeUISlots()
    {
        slotIcons = new Image[inventory.slotCount];
        amountTexts = new TextMeshProUGUI[inventory.slotCount];
        slotHighlights = new GameObject[inventory.slotCount];

        for (int i = 0; i < inventory.slotCount; i++)
        {
            string slotName = $"{slotNamePrefix}{i + 1}";
            Transform slotTransform = transform.Find(slotName);

            if (slotTransform != null)
            {
                // 查找图标
                Transform iconTransform = slotTransform.Find(iconName);
                if (iconTransform != null)
                {
                    slotIcons[i] = iconTransform.GetComponent<Image>();
                }

                // 查找数量文本
                Transform amountTransform = slotTransform.Find(amountTextName);
                if (amountTransform != null)
                {
                    amountTexts[i] = amountTransform.GetComponent<TextMeshProUGUI>();
                }

                // 查找高亮对象（可选）
                Transform highlightTransform = slotTransform.Find("Highlight");
                if (highlightTransform != null)
                {
                    slotHighlights[i] = highlightTransform.gameObject;
                }
            }

            // 调试警告
            if (slotIcons[i] == null) Debug.LogWarning($"未找到槽位 {i} 的图标");
            if (amountTexts[i] == null) Debug.LogWarning($"未找到槽位 {i} 的数量文本");
        }
    }

    /// <summary>
    /// 更新所有物品槽的UI显示
    /// </summary>
    public void UpdateAllSlots()
    {
        if (inventory == null) return;

        for (int i = 0; i < slotIcons.Length; i++)
        {
            UpdateSlotUI(i);
        }
    }

    /// <summary>
    /// 更新单个物品槽的UI
    /// </summary>
    /// <param name="slotIndex">槽位索引</param>
    private void UpdateSlotUI(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slotIcons.Length) return;

        bool slotValid = slotIndex < inventory.items.Count;
        Item item = slotValid ? inventory.items[slotIndex] : null;
        bool hasItem = item != null;

        // 更新图标
        if (slotIcons[slotIndex] != null)
        {
            slotIcons[slotIndex].sprite = hasItem ? item.icon : null;
            slotIcons[slotIndex].color = hasItem ? Color.white : Color.clear;
        }

        // 更新数量文本
        if (amountTexts != null && slotIndex < amountTexts.Length && amountTexts[slotIndex] != null)
        {
            if (hasItem && item.IsStackable && (!hideAmountWhenSingle || item.currentAmount > 1))
            {
                amountTexts[slotIndex].text = $"{item.currentAmount}/{item.maxStack}";
                amountTexts[slotIndex].gameObject.SetActive(true);

                // 根据数量设置颜色
                if (item.currentAmount >= item.maxStack)
                {
                    amountTexts[slotIndex].color = fullAmountColor;
                }
                else if (item.currentAmount <= 1)
                {
                    amountTexts[slotIndex].color = lowAmountColor;
                }
                else
                {
                    amountTexts[slotIndex].color = normalAmountColor;
                }
            }
            else
            {
                amountTexts[slotIndex].gameObject.SetActive(false);
            }
        }

        // 更新高亮状态（可选）
        if (slotHighlights != null && slotIndex < slotHighlights.Length)
        {
            if (slotHighlights[slotIndex] != null)
            {
                slotHighlights[slotIndex].SetActive(false); // 默认隐藏
            }
        }
    }

    /// <summary>
    /// 物品槽点击事件（绑定到UI按钮）
    /// </summary>
    public void OnSlotClick(int slotIndex)
    {
        if (inventory == null) return;

        // 使用物品
        inventory.UseItem(slotIndex);

        // 更新高亮显示
        if (slotHighlights != null && slotIndex < slotHighlights.Length)
        {
            // 重置所有高亮
            foreach (var highlight in slotHighlights)
            {
                if (highlight != null) highlight.SetActive(false);
            }

            // 高亮当前槽位（如果槽位有物品）
            if (slotHighlights[slotIndex] != null &&
                slotIndex < inventory.items.Count &&
                inventory.items[slotIndex] != null)
            {
                slotHighlights[slotIndex].SetActive(true);
            }
        }
    }

    /// <summary>
    /// 强制更新指定槽位的UI（外部调用）
    /// </summary>
    public void RefreshSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slotIcons.Length)
        {
            UpdateSlotUI(slotIndex);
        }
    }
}