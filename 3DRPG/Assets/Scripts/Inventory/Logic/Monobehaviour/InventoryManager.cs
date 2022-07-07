using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>
{
    public class DragData
    {
        public SlotHolder originalHolder;
        public RectTransform originalParent;
    }

    // 最后添加模板用于保存数据
    [Header("Inventory Data")] public InventoryData_SO inventoryTemplate;
    public InventoryData_SO inventoryData;
    public InventoryData_SO actionTemplate;
    public InventoryData_SO actionData;
    public InventoryData_SO equipmentTemplate;
    public InventoryData_SO equipmentData;

    [Header("Container")] public ContainerUI inventoryUI;
    public ContainerUI actionUI;
    public ContainerUI equipmentUI;

    [Header("Drag Canvas")] public Canvas dragCanvas;

    [Header("UI Panel")] public GameObject bagPanel;
    public GameObject characterStatsPanel;

    [Header("Stats Text")] public Text healthText;
    public Text attackText;

    [Header("Tooltip")] public ItemTooltip tooltip;

    public DragData currentDrag;

    private bool m_IsOpen = false;

    protected override void Awake()
    {
        base.Awake();
        if (inventoryTemplate != null) inventoryData = Instantiate(inventoryTemplate);

        if (actionTemplate != null) actionData = Instantiate(actionTemplate);

        if (equipmentTemplate != null) equipmentData = Instantiate(equipmentTemplate);
    }

    private void Start()
    {
        LoadData();
        inventoryUI.RefreshUI();
        actionUI.RefreshUI();
        equipmentUI.RefreshUI();
    }

    private void Update()
    {
        //BUG:后续单独设置两个面板的开启与关闭 同时需要设置Close按钮的Click事件
        if (Input.GetKeyDown(KeyCode.B))
        {
            m_IsOpen = !m_IsOpen;
            bagPanel.SetActive(m_IsOpen);
            characterStatsPanel.SetActive(m_IsOpen);
        }

        // TODO:后续在GameManager封装一个获取数值的方法
        UpdateStatsText(GameManager.Instance.playerCharacterStats.MaxHealth,
            GameManager.Instance.playerCharacterStats.attackData.minDamage,
            GameManager.Instance.playerCharacterStats.attackData.maxDamage);
    }

    public void SaveData()
    {
        SaveManager.Instance.Save(inventoryData, inventoryData.name);
        SaveManager.Instance.Save(actionData, actionData.name);
        SaveManager.Instance.Save(equipmentData, equipmentData.name);
    }

    public void LoadData()
    {
        SaveManager.Instance.Load(inventoryData, inventoryData.name);
        SaveManager.Instance.Load(actionData, actionData.name);
        SaveManager.Instance.Load(equipmentData, equipmentData.name);
    }

    #region 检查拖拽物品是否在每一个 Slot 范围内

    public bool CheckInInventoryUI(Vector3 mousePosition)
    {
        for (int i = 0; i < inventoryUI.slotHolders.Length; i++)
        {
            RectTransform theRectTransform = inventoryUI.slotHolders[i].transform as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(theRectTransform, mousePosition))
            {
                return true;
            }
        }

        return false;
    }

    public bool CheckInActionUI(Vector3 mousePosition)
    {
        for (int i = 0; i < actionUI.slotHolders.Length; i++)
        {
            RectTransform theRectTransform = actionUI.slotHolders[i].transform as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(theRectTransform, mousePosition))
            {
                return true;
            }
        }

        return false;
    }

    public bool CheckInEquipmentUI(Vector3 mousePosition)
    {
        for (int i = 0; i < equipmentUI.slotHolders.Length; i++)
        {
            RectTransform theRectTransform = equipmentUI.slotHolders[i].transform as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(theRectTransform, mousePosition))
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    public void UpdateStatsText(int health, int min, int max)
    {
        healthText.text = health.ToString();
        attackText.text = min + " - " + max;
    }

    #region 检测任务物品

    public void CheckQuestItemInBag(string questItemName)
    {
        foreach (var item in inventoryData.itemList)
        {
            if (item.itemData != null)
            {
                if (item.itemData.itemName == questItemName)
                    QuestManager.Instance.UpdateQuestProgress(item.itemData.itemName, item.amount);
            }
        }

        foreach (var item in actionData.itemList)
        {
            if (item.itemData != null)
            {
                if (item.itemData.itemName == questItemName)
                    QuestManager.Instance.UpdateQuestProgress(item.itemData.itemName, item.amount);
            }
        }
    }

    #endregion

    // 检测背包和快捷栏物品
    public InventoryItem QuestItemInBag(ItemData_SO questItem)
    {
        return inventoryData.itemList.Find(i => i.itemData == questItem);
    }

    public InventoryItem QuestItemInAction(ItemData_SO questItem)
    {
        return actionData.itemList.Find(i => i.itemData == questItem);
    }
}