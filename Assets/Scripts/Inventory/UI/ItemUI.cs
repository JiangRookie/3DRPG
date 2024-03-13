using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public Image icon = null;
    public Text amount = null;
    public ItemData_SO currentItemData;
    public InventoryData_SO Bag { get; set; }
    public int Index { get; set; } = -1;

    public void SetupItemUI(ItemData_SO item, int itemAmount)
    {
        if (itemAmount == 0)
        {
            Bag.itemList[Index].itemData = null;
            icon.gameObject.SetActive(false);
            return;
        }

        if (itemAmount < 0) item = null; // Jump To "icon.gameObject.SetActive(false);"

        if (item != null)
        {
            currentItemData = item;
            icon.sprite = item.itemIcon;
            amount.text = itemAmount.ToString();
            icon.gameObject.SetActive(true);
        }
        else
        {
            icon.gameObject.SetActive(false);
        }
    }

    public ItemData_SO GetItem() => Bag.itemList[Index].itemData;
}