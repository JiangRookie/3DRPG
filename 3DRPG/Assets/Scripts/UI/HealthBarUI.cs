using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthBarUIPrefab;
    public Transform healthBarPoint;
    public bool isAlwaysVisible = true;
    public float visibleTime = 3f;

    private float m_RemainVisibleTime;
    private Image m_CurrentHealthImage;
    private Transform m_UIbar;
    private Transform m_Camera;

    private CharacterStats m_EnemyCurrentStats;

    private void Awake()
    {
        m_EnemyCurrentStats = GetComponent<CharacterStats>();

        m_EnemyCurrentStats.UpdateHealthBarOnAttack += UpdateHealthBarUI;
    }

    private void OnEnable()
    {
        m_Camera = Camera.main.transform;

        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                m_UIbar = Instantiate(healthBarUIPrefab, canvas.transform).transform;
                m_CurrentHealthImage = m_UIbar.GetChild(0).GetComponent<Image>();
                m_UIbar.gameObject.SetActive(isAlwaysVisible);
            }
        }
    }

    private void LateUpdate()
    {
        if (m_UIbar == null) return;
        m_UIbar.position = healthBarPoint.position;
        m_UIbar.forward = -m_Camera.forward;

        if (m_RemainVisibleTime <= 0 && !isAlwaysVisible)
            m_UIbar.gameObject.SetActive(false);
        else
            m_RemainVisibleTime -= Time.deltaTime;
    }

    private void UpdateHealthBarUI(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0)
            Destroy(m_UIbar.gameObject);

        m_UIbar.gameObject.SetActive(true);
        m_RemainVisibleTime = visibleTime;

        var sliderPercent = (float)currentHealth / maxHealth;
        m_CurrentHealthImage.fillAmount = sliderPercent;
    }
}