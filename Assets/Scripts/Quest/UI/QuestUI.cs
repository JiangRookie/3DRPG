using UnityEngine;
using UnityEngine.UI;

public class QuestUI : Singleton<QuestUI>
{
    [Header("Elements")]
    public GameObject questPanel;
    public ItemTooltip tooltip;
    private bool m_bIsOpen;

    [Header("Quest Name")]
    public RectTransform questListTransform;
    public QuestNameButton questNameButton;

    [Header("Text Content")]
    public Text questContentText;

    [Header("Requirement")]
    public RectTransform requireTransform;
    public QuestRequirement requirement;

    [Header("Reward Panel")]
    public RectTransform rewardTransform;
    public ItemUI rewardUI;

    public void SwitchQuestUI()
    {
        m_bIsOpen = !m_bIsOpen;
        questPanel.SetActive(m_bIsOpen);
        questContentText.text = string.Empty;

        // 显示内容面板
        SetupQuestList();

        if (!m_bIsOpen)
            tooltip.gameObject.SetActive(false);
    }

    public void SetupQuestList()
    {
        foreach (Transform item in questListTransform)
        {
            Destroy(item.gameObject);
        }

        foreach (Transform item in rewardTransform)
        {
            Destroy(item.gameObject);
        }

        foreach (Transform item in requireTransform)
        {
            Destroy(item.gameObject);
        }

        foreach (var task in QuestManager.Instance.taskList)
        {
            var newTask = Instantiate(questNameButton, questListTransform);
            newTask.SetupNameButton(task.questData);

            // newTask.questContentText = questContentText;
        }
    }

    public void SetupRequireList(QuestData_SO questData)
    {
        questContentText.text = questData.description;

        foreach (Transform item in requireTransform)
        {
            Destroy(item.gameObject);
        }

        foreach (QuestData_SO.QuestRequire require in questData.questRequireList)
        {
            var q = Instantiate(requirement, requireTransform);

            if (questData.isFinished)
                q.SetupRequirement(require.name, true);
            else
                q.SetupRequirement(require.name, require.requireAmount, require.currentAmount);
        }
    }

    public void SetupRewardItem(ItemData_SO itemData, int amount)
    {
        var item = Instantiate(rewardUI, rewardTransform);
        item.SetupItemUI(itemData, amount);
    }
}