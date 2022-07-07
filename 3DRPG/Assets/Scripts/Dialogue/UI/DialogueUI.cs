using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : Singleton<DialogueUI>
{
    [Header("基本设置")] public Image icon;
    public Text mainText;
    public Button nextButton;
    public GameObject dialoguePanel;

    [Header("Options")] public RectTransform optionPanel;
    public OptionUI optionPrefab;

    [Header("数据信息")] public DialogueData_SO currentDialogueData;
    private int m_CurrentDialogueDataIndex;

    protected override void Awake()
    {
        base.Awake();
        nextButton.onClick.AddListener(ContinueDialogue);
    }

    private void ContinueDialogue()
    {
        if (m_CurrentDialogueDataIndex < currentDialogueData.dialoguePieceList.Count)
            UpdateMainDialogue(currentDialogueData.dialoguePieceList[m_CurrentDialogueDataIndex]);
        else dialoguePanel.SetActive(false);
    }

    public void UpdateDialogueData(DialogueData_SO data)
    {
        currentDialogueData = data;
        m_CurrentDialogueDataIndex = 0;
    }

    public void UpdateMainDialogue(DialoguePiece piece)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        dialoguePanel.SetActive(true);      
        m_CurrentDialogueDataIndex++;

        if (piece.image != null)
        {
            icon.enabled = true;
            icon.sprite = piece.image;
        }
        else icon.enabled = false;

        mainText.text = string.Empty;
        // mainText.text = piece.text;
        mainText.DOText(piece.text, 1f);

        // 没有选项 有下一句话
        if (piece.optionList.Count == 0 && currentDialogueData.dialoguePieceList.Count > 0)
        {
            nextButton.interactable = true;
            nextButton.gameObject.SetActive(true);
            nextButton.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            // nextButton.gameObject.SetActive(false);
            nextButton.interactable = false; // 设置按钮不可点按
            nextButton.transform.GetChild(0).gameObject.SetActive(false); // 把文字隐藏掉可以不改变布局
        }

        // 创建Options
        CreateOptions(piece);
    }

    private void CreateOptions(DialoguePiece piece)
    {
        if (optionPanel.childCount > 0)
        {
            for (int i = 0; i < optionPanel.childCount; i++)
            {
                Destroy(optionPanel.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < piece.optionList.Count; i++)
        {
            var option = Instantiate(optionPrefab, optionPanel);
            option.UpdateOption(piece, piece.optionList[i]);
        }
    }
}