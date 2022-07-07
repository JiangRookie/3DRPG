using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public ItemData_SO itemData;
    public int amount;
}

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Inventory Data", order = 0)]
public class InventoryData_SO : ScriptableObject
{
    public List<InventoryItem> itemList = new List<InventoryItem>();

    public void AddItem(ItemData_SO newItemData, int amount)
    {
        bool isFoundSameItem = false; // 是否找到相同的道具
        if (newItemData.isStackable) // 可堆叠
        {
            foreach (InventoryItem item in itemList)
            {
                if (item.itemData == newItemData)
                {
                    item.amount += amount;
                    isFoundSameItem = true;
                    break;
                }
            }
        }

        // 不可堆叠
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].itemData == null && !isFoundSameItem)
            {
                itemList[i].itemData = newItemData;
                itemList[i].amount = amount;
                break;
            }
        }
    }
}