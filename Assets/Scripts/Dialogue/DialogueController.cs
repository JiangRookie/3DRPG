using System;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public DialogueData_SO currentDialogueData;
    private bool m_bCanTalk = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && currentDialogueData != null)
        {
            m_bCanTalk = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueUI.Instance.dialoguePanel.SetActive(false);
            m_bCanTalk = false; // 避免随时可以启动对话窗口
        }
    }

    private void Update()
    {
        if (m_bCanTalk && Input.GetMouseButtonDown(1))
        {
            OpenDialogue();
        }
    }

    private void OpenDialogue()
    {
        // 打开UI面板
        // 传输对话内容信息
        DialogueUI.Instance.UpdateDialogueData(currentDialogueData);
        DialogueUI.Instance.UpdateMainDialogue(currentDialogueData.dialoguePieceList[0]);
    }
}