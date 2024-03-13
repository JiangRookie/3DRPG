using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerCharacterStats;

    private CinemachineFreeLook m_FollowCamera;

    private List<IEndGameObserver> m_EndGameObserverList = new List<IEndGameObserver>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void RegisterPlayer(CharacterStats thePlayerCharacterStats)
    {
        playerCharacterStats = thePlayerCharacterStats;

        m_FollowCamera = FindObjectOfType<CinemachineFreeLook>();

        if (m_FollowCamera != null)
        {
            m_FollowCamera.Follow = playerCharacterStats.transform.GetChild(2);
            m_FollowCamera.LookAt = playerCharacterStats.transform.GetChild(2);
        }
    }

    public void AddObserver(IEndGameObserver observer)
    {
        m_EndGameObserverList.Add(observer);
    }

    public void RemoveObserver(IEndGameObserver observer)
    {
        m_EndGameObserverList.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach (var observer in m_EndGameObserverList)
        {
            observer.EndNotify();
        }
    }

    public Transform GetEntrance()
    {
        foreach (var item in FindObjectsOfType<TransitionDestination>())
        {
            if (item.destinationTag == TransitionDestination.DestinationTag.ENTER)
                return item.transform;
        }

        return null;
    }
}