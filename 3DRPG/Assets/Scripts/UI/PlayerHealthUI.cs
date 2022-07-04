using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    private Text m_LevelText;
    private Image m_HealthSlider;
    private Image m_ExpSlider;

    private void Awake()
    {
        m_HealthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        m_ExpSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        m_LevelText = transform.GetChild(2).GetComponent<Text>();
    }

    private void Update()
    {
        m_LevelText.text = "LEVEL  " + GameManager.Instance.playerStats.characterData.currentLevel.ToString("00");
        UpdateHealth();
        UpdateExp();
    }

    private void UpdateHealth()
    {
        var sliderPercent = (float)GameManager.Instance.playerStats.CurrentHealth /
                            GameManager.Instance.playerStats.MaxHealth;
        m_HealthSlider.fillAmount = sliderPercent;
    }

    private void UpdateExp()
    {
        var sliderPercent = (float)GameManager.Instance.playerStats.characterData.currentExp /
                            GameManager.Instance.playerStats.characterData.baseExp;
        m_ExpSlider.fillAmount = sliderPercent;
    }
}