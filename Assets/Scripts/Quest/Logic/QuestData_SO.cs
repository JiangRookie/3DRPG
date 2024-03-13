using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest Data", order = 0)]
public class QuestData_SO : ScriptableObject
{
    [Serializable]
    public class QuestRequire
    {
        public string name;
        public int requireAmount;
        public int currentAmount;
    }

    public string questName;
    [TextArea] public string description;

    public bool isStarted;
    public bool isComplete;
    public bool isFinished;

    public List<QuestRequire> questRequireList = new List<QuestRequire>();

    public List<InventoryItem> rewardList = new List<InventoryItem>();

    public void CheckQuestProgress()
    {
        var finishRequires = questRequireList.Where(r => r.requireAmount <= r.currentAmount);
        isComplete = finishRequires.Count() == questRequireList.Count;

        if (isComplete) Debug.Log("任务完成");
    }

    // 当前任务中所需要的 收集、消灭 的目标名字列表
    public List<string> RequireTargetName()
    {
        List<string> targetNameList = new List<string>();

        foreach (var require in questRequireList)
        {
            targetNameList.Add(require.name);
        }

        return targetNameList;
    }

    public void GiveRewards()
    {
        foreach (var reward in rewardList)
        {
            if (reward.amount < 0) //需要上交任务物品的情况
            {
                int requireCount = Mathf.Abs(reward.amount);

                if (InventoryManager.Instance.QuestItemInBag(reward.itemData) != null) //背包当中有需要交的物品
                {
                    // 背包当中需要上交物品的数量刚好够或者不够的情况
                    if (InventoryManager.Instance.QuestItemInBag(reward.itemData).amount <= requireCount)
                    {
                        requireCount -= InventoryManager.Instance.QuestItemInBag(reward.itemData).amount;
                        InventoryManager.Instance.QuestItemInBag(reward.itemData).amount = 0;
                        if (InventoryManager.Instance.QuestItemInAction(reward.itemData) != null)
                        {
                            InventoryManager.Instance.QuestItemInAction(reward.itemData).amount -= requireCount;
                        }
                    }
                    else // InventoryManager.Instance.QuestItemInBag(reward.itemData).amount > requireCount
                        // 背包当中上交物品的数量充足
                    {
                        InventoryManager.Instance.QuestItemInBag(reward.itemData).amount -= requireCount;
                    }
                }
                else // 背包当中没有上交物品代表Action中一定满足了任务物品的数量
                {
                    InventoryManager.Instance.QuestItemInAction(reward.itemData).amount -= requireCount;
                }
            }
            else // 正常获得的额外物品奖励添加到背包中
            {
                InventoryManager.Instance.inventoryData.AddItem(reward.itemData, reward.amount);
            }

            InventoryManager.Instance.inventoryUI.RefreshUI();
            InventoryManager.Instance.actionUI.RefreshUI();
        }
    }
}