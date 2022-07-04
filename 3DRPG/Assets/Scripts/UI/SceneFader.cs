using System.Collections;
using UnityEngine;

public class SceneFader : MonoBehaviour
{
    public float fadeInDuration = 1.5f;
    public float fadeOutDuration = 1.5f;
    private CanvasGroup m_CanvasGroup;

    private void Awake()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator FadeOutIn()
    {
        yield return FadeOut(fadeOutDuration);

        yield return FadeIn(fadeInDuration);
    }

    public IEnumerator FadeOut(float time)
    {
        while (m_CanvasGroup.alpha < 1)
        {
            m_CanvasGroup.alpha += Time.deltaTime / time;
            yield return null;
        }
    }

    public IEnumerator FadeIn(float time)
    {
        while (m_CanvasGroup.alpha != 0)
        {
            m_CanvasGroup.alpha -= Time.deltaTime / time;
            yield return null;
        }
    }
}