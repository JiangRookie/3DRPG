using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemUI))]
public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ItemUI m_CurrentItemUI;
    private SlotHolder m_CurrentHolder;
    private SlotHolder m_TargetHolder;

    private void Awake()
    {
        m_CurrentItemUI = GetComponent<ItemUI>();
        m_CurrentHolder = GetComponentInParent<SlotHolder>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        InventoryManager.Instance.currentDrag = new InventoryManager.DragData();
        InventoryManager.Instance.currentDrag.originalHolder = GetComponentInParent<SlotHolder>();
        InventoryManager.Instance.currentDrag.originalParent = transform.parent as RectTransform;

        transform.SetParent(InventoryManager.Instance.dragCanvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 鼠标指针是否指向UI物品
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (InventoryManager.Instance.CheckInInventoryUI(eventData.position)
                || InventoryManager.Instance.CheckInActionUI(eventData.position)
                || InventoryManager.Instance.CheckInEquipmentUI(eventData.position))
            {
                if (eventData.pointerEnter.gameObject.GetComponent<SlotHolder>())
                {
                    m_TargetHolder = eventData.pointerEnter.gameObject.GetComponent<SlotHolder>();
                }
                else
                {
                    m_TargetHolder = eventData.pointerEnter.gameObject.GetComponentInParent<SlotHolder>();
                }
                // 判断是否目标holder是我的原holder
                if (m_TargetHolder != InventoryManager.Instance.currentDrag.originalHolder)
                {
                    switch (m_TargetHolder.slotType)
                    {
                        case SlotType.BAG:
                            SwapItem();
                            break;
                        case SlotType.WEAPON:
                            if (m_CurrentItemUI.Bag.itemList[m_CurrentItemUI.Index].itemData.itemType ==
                                ItemType.Weapon)
                                SwapItem();
                            break;
                        case SlotType.ARMOR:
                            if (m_CurrentItemUI.Bag.itemList[m_CurrentItemUI.Index].itemData.itemType == ItemType.Armor)
                                SwapItem();
                            break;
                        case SlotType.ACTION:
                            if (m_CurrentItemUI.Bag.itemList[m_CurrentItemUI.Index].itemData.itemType ==
                                ItemType.Usable)
                                SwapItem();
                            break;
                    }
                }

                m_CurrentHolder.UpdateItem();
                m_TargetHolder.UpdateItem();
            }
        }

        transform.SetParent(InventoryManager.Instance.currentDrag.originalParent);

        RectTransform theRectTransform = transform as RectTransform;
        theRectTransform.offsetMax = -Vector2.one * 5f;
        theRectTransform.offsetMin = Vector2.one * 5f;
        /*鼠标指针没有指向UI物品 指向世界 可以生成(仍)物品*/
        /*else
        {
            
        }*/
    }

    public void SwapItem()
    {
        var targetItem = m_TargetHolder.itemUI.Bag.itemList[m_TargetHolder.itemUI.Index];
        var tempItem = m_CurrentHolder.itemUI.Bag.itemList[m_CurrentHolder.itemUI.Index];

        bool isSameItem = tempItem.itemData == targetItem.itemData;

        if (isSameItem && targetItem.itemData.isStackable)
        {
            targetItem.amount += tempItem.amount;
            tempItem.itemData = null;
            tempItem.amount = 0;
        }
        else
        {
            m_CurrentHolder.itemUI.Bag.itemList[m_CurrentHolder.itemUI.Index] = targetItem;
            m_TargetHolder.itemUI.Bag.itemList[m_TargetHolder.itemUI.Index] = tempItem;
        }
    }
}