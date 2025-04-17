using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Inventory))]
public class InventoryUI : MonoBehaviour
{
    [Header("��������")]
    public Inventory inventory;
    public Image[] slotIcons;          // ��Ʒͼ����ʾ
    public TextMeshProUGUI[] amountTexts; // �����ı� (��ʽ: ��ǰ/���)
    public GameObject[] slotHighlights; // ѡ�и���Ч��

    [Header("������ʾ����")]
    public Color normalAmountColor = Color.white;
    public Color fullAmountColor = Color.green;
    public Color lowAmountColor = new Color(1, 0.8f, 0); // ǳ��ɫ
    public bool hideAmountWhenSingle = true; // ������Ϊ1ʱ�Ƿ�����

    [Header("�Զ���ʼ��")]
    public bool autoFindSlots = true;
    public string slotNamePrefix = "Slot";
    public string iconName = "Icon";
    public string amountTextName = "AmountText";

    private void OnEnable()
    {
        // ������Ʒ����¼�
        if (inventory != null)
        {
            inventory.OnInventoryChanged += UpdateAllSlots;
        }
    }

    private void OnDisable()
    {
        // ȡ������
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= UpdateAllSlots;
        }
    }

    private void Start()
    {
        // ȷ��������ȷ
        if (inventory == null)
        {
            inventory = GetComponent<Inventory>();
        }

        // �Զ���ʼ��UIԪ��
        if (autoFindSlots)
        {
            InitializeUISlots();
        }

        // ��ʼ����
        UpdateAllSlots();
    }

    /// <summary>
    /// �Զ����Ҳ���ʼ������UI��λ
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
                // ����ͼ��
                Transform iconTransform = slotTransform.Find(iconName);
                if (iconTransform != null)
                {
                    slotIcons[i] = iconTransform.GetComponent<Image>();
                }

                // ���������ı�
                Transform amountTransform = slotTransform.Find(amountTextName);
                if (amountTransform != null)
                {
                    amountTexts[i] = amountTransform.GetComponent<TextMeshProUGUI>();
                }

                // ���Ҹ������󣨿�ѡ��
                Transform highlightTransform = slotTransform.Find("Highlight");
                if (highlightTransform != null)
                {
                    slotHighlights[i] = highlightTransform.gameObject;
                }
            }

            // ���Ծ���
            if (slotIcons[i] == null) Debug.LogWarning($"δ�ҵ���λ {i} ��ͼ��");
            if (amountTexts[i] == null) Debug.LogWarning($"δ�ҵ���λ {i} �������ı�");
        }
    }

    /// <summary>
    /// ����������Ʒ�۵�UI��ʾ
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
    /// ���µ�����Ʒ�۵�UI
    /// </summary>
    /// <param name="slotIndex">��λ����</param>
    private void UpdateSlotUI(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slotIcons.Length) return;

        bool slotValid = slotIndex < inventory.items.Count;
        Item item = slotValid ? inventory.items[slotIndex] : null;
        bool hasItem = item != null;

        // ����ͼ��
        if (slotIcons[slotIndex] != null)
        {
            slotIcons[slotIndex].sprite = hasItem ? item.icon : null;
            slotIcons[slotIndex].color = hasItem ? Color.white : Color.clear;
        }

        // ���������ı�
        if (amountTexts != null && slotIndex < amountTexts.Length && amountTexts[slotIndex] != null)
        {
            if (hasItem && item.IsStackable && (!hideAmountWhenSingle || item.currentAmount > 1))
            {
                amountTexts[slotIndex].text = $"{item.currentAmount}/{item.maxStack}";
                amountTexts[slotIndex].gameObject.SetActive(true);

                // ��������������ɫ
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

        // ���¸���״̬����ѡ��
        if (slotHighlights != null && slotIndex < slotHighlights.Length)
        {
            if (slotHighlights[slotIndex] != null)
            {
                slotHighlights[slotIndex].SetActive(false); // Ĭ������
            }
        }
    }

    /// <summary>
    /// ��Ʒ�۵���¼����󶨵�UI��ť��
    /// </summary>
    public void OnSlotClick(int slotIndex)
    {
        if (inventory == null) return;

        // ʹ����Ʒ
        inventory.UseItem(slotIndex);

        // ���¸�����ʾ
        if (slotHighlights != null && slotIndex < slotHighlights.Length)
        {
            // �������и���
            foreach (var highlight in slotHighlights)
            {
                if (highlight != null) highlight.SetActive(false);
            }

            // ������ǰ��λ�������λ����Ʒ��
            if (slotHighlights[slotIndex] != null &&
                slotIndex < inventory.items.Count &&
                inventory.items[slotIndex] != null)
            {
                slotHighlights[slotIndex].SetActive(true);
            }
        }
    }

    /// <summary>
    /// ǿ�Ƹ���ָ����λ��UI���ⲿ���ã�
    /// </summary>
    public void RefreshSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slotIcons.Length)
        {
            UpdateSlotUI(slotIndex);
        }
    }
}