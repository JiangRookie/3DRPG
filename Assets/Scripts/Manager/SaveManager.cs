using Jiang.Games;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    private SaveSystem _SaveSystem;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        _SaveSystem = Global.Interface.GetSystem<SaveSystem>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneController.Instance.TransitionToMainScene();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            _SaveSystem.SavePlayerData();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            _SaveSystem.LoadPlayerData();
        }
    }
}