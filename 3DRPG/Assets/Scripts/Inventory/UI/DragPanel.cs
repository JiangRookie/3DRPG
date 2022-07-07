using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPanel : MonoBehaviour, IDragHandler,IPointerDownHandler
{
    private RectTransform m_RectTransform;
    private Canvas m_Canvas;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_Canvas = InventoryManager.Instance.GetComponent<Canvas>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_RectTransform.anchoredPosition += eventData.delta / m_Canvas.scaleFactor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_RectTransform.SetSiblingIndex(2);
    }
}