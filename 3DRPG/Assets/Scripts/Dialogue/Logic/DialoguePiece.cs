using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialoguePiece
{
    public string ID;
    public Sprite image; /*NPC头像、任务奖励etc*/
    [TextArea] public string text; /*文本*/
    public QuestData_SO quest;
    public List<DialogueOption> optionList = new List<DialogueOption>();

    [HideInInspector] // 在Editor中使用
    public bool canExpand;
}