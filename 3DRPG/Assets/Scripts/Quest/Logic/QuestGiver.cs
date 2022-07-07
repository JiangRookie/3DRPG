using System;
using UnityEngine;

[RequireComponent(typeof(DialogueController))]
public class QuestGiver : MonoBehaviour
{
    private DialogueController m_DialogueController;
    private QuestData_SO m_CurrentQuest;
    public DialogueData_SO startDialogue;
    public DialogueData_SO progressDialogue;
    public DialogueData_SO completeDialogue;
    public DialogueData_SO finishDialogue;

    #region 获取任务状态

    public bool IsStarted
    {
        get
        {
            if (QuestManager.Instance.HaveQuest(m_CurrentQuest))
                return QuestManager.Instance.GetTask(m_CurrentQuest).IsStarted;
            return false;
        }
    }

    public bool IsCompleted
    {
        get
        {
            if (QuestManager.Instance.HaveQuest(m_CurrentQuest))
                return QuestManager.Instance.GetTask(m_CurrentQuest).IsCompleted;
            return false;
        }
    }

    public bool IsFinished
    {
        get
        {
            if (QuestManager.Instance.HaveQuest(m_CurrentQuest))
                return QuestManager.Instance.GetTask(m_CurrentQuest).IsFinished;
            return false;
        }
    }

    #endregion


    private void Awake()
    {
        m_DialogueController = GetComponent<DialogueController>();
    }

    private void Start()
    {
        m_DialogueController.currentDialogueData = startDialogue;
        m_CurrentQuest = m_DialogueController.currentDialogueData.GetQuest();
    }

    private void Update()
    {
        if (IsStarted)
        {
            m_DialogueController.currentDialogueData = IsCompleted
                ? completeDialogue
                : progressDialogue;
        }

        if (IsFinished) 
            m_DialogueController.currentDialogueData = finishDialogue;
    }
}