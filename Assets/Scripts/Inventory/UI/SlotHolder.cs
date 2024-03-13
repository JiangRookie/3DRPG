using System;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SlotType { BAG, WEAPON, ARMOR, ACTION }

public class SlotHolder : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public SlotType slotType;
    public ItemUI itemUI;

    private void OnDisable()
    {
        InventoryManager.Instance.tooltip.gameObject.SetActive(false);
    }

    public void UpdateItem()
    {
        switch (slotType)
        {
            case SlotType.BAG:
                itemUI.Bag = InventoryManager.Instance.inventoryData;
                break;
            case SlotType.WEAPON:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                // 装备武器 切换武器
                if (itemUI.Bag.itemList[itemUI.Index].itemData != null)
                {
                    GameManager.Instance.playerCharacterStats.ChangeWeapon(itemUI.Bag.itemList[itemUI.Index].itemData);
                }
                else
                {
                    GameManager.Instance.playerCharacterStats.UnEquipmentWeapon();
                }

                break;
            case SlotType.ARMOR:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                break;
            case SlotType.ACTION:
                itemUI.Bag = InventoryManager.Instance.actionData;
                break;
        }

        var item = itemUI.Bag.itemList[itemUI.Index];
        itemUI.SetupItemUI(item.itemData, item.amount);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount % 2 == 0)
        {
            UseItem();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemUI.GetItem())
        {
            InventoryManager.Instance.tooltip.SetupTooltip(itemUI.GetItem());
            InventoryManager.Instance.tooltip.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryManager.Instance.tooltip.gameObject.SetActive(false);
    }

    public void UseItem()
    {
        if (itemUI.GetItem() == null) return;
        if (itemUI.GetItem().itemType == ItemType.Usable && itemUI.Bag.itemList[itemUI.Index].amount > 0)
        {
            GameManager.Instance.playerCharacterStats.ApplyHealth(itemUI.GetItem().usableItemData.healthPoint);

            itemUI.Bag.itemList[itemUI.Index].amount -= 1;

            // 检查任务物品更新进度
            QuestManager.Instance.UpdateQuestProgress(itemUI.GetItem().itemName, -1);
        }

        UpdateItem();
    }
}