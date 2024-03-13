using UnityEngine;

public enum ItemType { Usable, Weapon, Armor }

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data", order = 0)]
public class ItemData_SO : ScriptableObject
{
    [Header("物品详情")] public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    public int itemAmount;
    [TextArea] public string description = string.Empty;
    public bool isStackable; // 是否可堆叠

    [Header("可使用物品信息")] public UsableItemData_SO usableItemData;

    [Header("武器信息")] public GameObject weaponPrefab;
    public AttackData_SO weaponData;
    public AnimatorOverrideController weaponAnimator;
}