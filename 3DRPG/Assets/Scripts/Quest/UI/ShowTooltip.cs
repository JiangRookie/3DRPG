using UnityEngine;
using UnityEngine.EventSystems;

public class ShowTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ItemUI m_CurrentItemUI;

    private void Awake()
    {
        m_CurrentItemUI = GetComponent<ItemUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        QuestUI.Instance.tooltip.gameObject.SetActive(true);
        QuestUI.Instance.tooltip.SetupTooltip(m_CurrentItemUI.currentItemData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        QuestUI.Instance.tooltip.gameObject.SetActive(false);
    }
}