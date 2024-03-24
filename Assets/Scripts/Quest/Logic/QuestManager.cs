using System;
using System.Collections.Generic;
using System.Linq;
using Jiang.Games;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    [Serializable]
    public class QuestTask
    {
        public QuestData_SO questData;

        public bool IsStarted
        {
            get => questData.isStarted;
            set => questData.isStarted = value;
        }

        public bool IsCompleted
        {
            get => questData.isComplete;
            set => questData.isComplete = value;
        }

        public bool IsFinished
        {
            get => questData.isFinished;
            set => questData.isFinished = value;
        }
    }
    
    private SaveSystem _SaveSystem;

    public List<QuestTask> taskList = new List<QuestTask>();

    private void Start()
    {
        _SaveSystem = Global.Interface.GetSystem<SaveSystem>();
        LoadQuestData();
    }

    // 敌人死亡、拾取物品时调用此函数
    public void UpdateQuestProgress(string requireName, int amount)
    {
        foreach (QuestTask task in taskList)
        {
            if (task.IsFinished)
                continue;
            var matchTask = task.questData.questRequireList.Find(r => r.name == requireName);
            if (matchTask != null)
                matchTask.currentAmount += amount;

            task.questData.CheckQuestProgress();
        }
    }

    public bool HaveQuest(QuestData_SO data)
    {
        if (data != null)
            return taskList.Any(q => q.questData.questName == data.questName);
        return false;
    }

    public QuestTask GetTask(QuestData_SO data)
    {
        return taskList.Find(q => q.questData.questName == data.questName);
    }

    public void LoadQuestData()
    {
        var questCount = PlayerPrefs.GetInt("QuestCount");

        for (int i = 0; i < questCount; i++)
        {
            var newQuestData = ScriptableObject.CreateInstance<QuestData_SO>();
            _SaveSystem.Load(newQuestData, "task" + i);
            taskList.Add(new QuestTask { questData = newQuestData });
        }
    }

    public void SaveQuestData() // 保存任务列表数量
    {
        PlayerPrefs.SetInt("QuestData", taskList.Count);
        for (int i = 0; i < taskList.Count; i++)
        {
            _SaveSystem.Save(taskList[i].questData, "task" + i);
        }
    }
}