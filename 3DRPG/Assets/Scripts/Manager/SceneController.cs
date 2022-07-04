using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>, IEndGameObserver
{
    public GameObject playerPrefab;
    public SceneFader sceneFaderPrefab;
    private bool m_bFadeFinished;

    private GameObject m_Player;
    private NavMeshAgent m_PlayerAgent;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        GameManager.Instance.AddObserver(this);
        m_bFadeFinished = true;
    }

    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name,
                    transitionPoint.transitionDestinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName,
                    transitionPoint.transitionDestinationTag));
                break;
        }
    }

    private IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)
    {
        SaveManager.Instance.SavePlayerData();

        if (SceneManager.GetActiveScene().name != sceneName) // 跨场景传送
        {
            //TODO:可以加入fader
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPrefab,
                GetDestination(destinationTag).transform.position,
                GetDestination(destinationTag).transform.rotation);

            SaveManager.Instance.LoadPlayerData();
        }
        else
        {
            m_Player = GameManager.Instance.playerStats.gameObject;
            m_PlayerAgent = m_Player.GetComponent<NavMeshAgent>();

            m_PlayerAgent.enabled = false;

            m_Player.transform.SetPositionAndRotation(
                GetDestination(destinationTag).transform.position,
                GetDestination(destinationTag).transform.rotation);

            m_PlayerAgent.enabled = true;
            yield return null;
        }
    }

    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();

        for (int i = 0; i < entrances.Length; i++)
        {
            if (entrances[i].destinationTag == destinationTag)
                return entrances[i];
        }

        return null;
    }

    public void TransitionToMainScene()
    {
        StartCoroutine(LoadMainScene());
    }

    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadScene(SaveManager.Instance.SceneName));
    }

    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadScene("Level01"));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);

        if (sceneName == "") yield break;

        yield return StartCoroutine(fade.FadeOut(2f));

        yield return SceneManager.LoadSceneAsync(sceneName);
        yield return m_Player = Instantiate(playerPrefab,
            GameManager.Instance.GetEntrance().position,
            GameManager.Instance.GetEntrance().rotation);

        SaveManager.Instance.SavePlayerData();

        yield return StartCoroutine(fade.FadeIn(2f));
    }

    private IEnumerator LoadMainScene()
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fade.FadeOut(2f));
        yield return SceneManager.LoadSceneAsync("MainScene");
        yield return StartCoroutine(fade.FadeIn(2f));
    }

    public void EndNotify() // 死亡后跳转到主场景
    {
        if (!m_bFadeFinished) return;
        m_bFadeFinished = false;
        StartCoroutine(LoadMainScene());
    }
}