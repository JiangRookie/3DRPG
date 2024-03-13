using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemData_SO itemData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //TODO:将物品添加到背包
            InventoryManager.Instance.inventoryData.AddItem(itemData, itemData.itemAmount);
            InventoryManager.Instance.inventoryUI.RefreshUI();
            // GameManager.Instance.playerCharacterStats.EquipWeapon(itemData);

            QuestManager.Instance.UpdateQuestProgress(itemData.itemName, itemData.itemAmount);
            Destroy(gameObject);
        }
    }
}