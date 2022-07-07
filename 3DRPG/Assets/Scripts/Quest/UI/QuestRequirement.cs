using UnityEngine;
using UnityEngine.UI;

public class QuestRequirement : MonoBehaviour
{
    private Text m_RequireName;
    private Text m_ProgressNumber;

    private void Awake()
    {
        m_RequireName = GetComponent<Text>();
        m_ProgressNumber = transform.GetChild(0).GetComponent<Text>();
    }

    public void SetupRequirement(string name, int amount, int currentAmount)
    {
        m_RequireName.text = name;
        m_ProgressNumber.text = currentAmount.ToString() + " / " + amount.ToString();
    }

    public void SetupRequirement(string name, bool isFinished)
    {
        if (isFinished)
        {
            m_RequireName.text = name;
            m_ProgressNumber.text = "完成";
            m_RequireName.color = Color.gray;
            m_ProgressNumber.color = Color.gray;
        }
    }
}