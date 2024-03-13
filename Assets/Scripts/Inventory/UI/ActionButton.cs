using System;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    public KeyCode actionKey;
    private SlotHolder m_CurrentSlotHolder;

    private void Awake()
    {
        m_CurrentSlotHolder = GetComponent<SlotHolder>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(actionKey) && m_CurrentSlotHolder.itemUI.GetItem())
        {
            m_CurrentSlotHolder.UseItem();
        }
    }
}