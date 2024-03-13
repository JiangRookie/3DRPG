using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    public Text optionText;

    private Button m_Button;
    private DialoguePiece m_CurrentPiece;
    private string m_NextPieceID;
    private bool m_bTakeQuest;

    private void Awake()
    {
        m_Button = GetComponent<Button>();
        m_Button.onClick.AddListener(OnOptionClicked);
    }


    public void UpdateOption(DialoguePiece piece, DialogueOption option)
    {
        m_CurrentPiece = piece;
        optionText.text = option.text;
        m_NextPieceID = option.targetID;
        m_bTakeQuest = option.takeQuest;
    }

    private void OnOptionClicked()
    {
        if (m_CurrentPiece.quest != null)
        {
            var newTask = new QuestManager.QuestTask
            {
                questData = Instantiate(m_CurrentPiece.quest)
            };

            if (m_bTakeQuest)
            {
                // 添加到任务列表
                // 判断是否已经有任务
                if (QuestManager.Instance.HaveQuest(newTask.questData))
                {
                    // 判断是否完成给予奖励
                    if (QuestManager.Instance.GetTask(newTask.questData).IsCompleted)
                    {
                        newTask.questData.GiveRewards();
                        QuestManager.Instance.GetTask(newTask.questData).IsFinished = true;
                    }
                }
                else
                {
                    // 没有任务 接受任务
                    QuestManager.Instance.taskList.Add(newTask);
                    QuestManager.Instance.GetTask(newTask.questData).IsStarted = true;
                    foreach (var requireItem in newTask.questData.RequireTargetName())
                    {
                        InventoryManager.Instance.CheckQuestItemInBag(requireItem);
                    }
                }
            }
        }

        if (m_NextPieceID == string.Empty)
        {
            DialogueUI.Instance.dialoguePanel.SetActive(false);
            return;
        }
        else
        {
            DialogueUI.Instance.UpdateMainDialogue(
                DialogueUI.Instance.currentDialogueData.dialogueIndex[m_NextPieceID]);
        }
    }
}