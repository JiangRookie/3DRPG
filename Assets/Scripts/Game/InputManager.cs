using QFramework;
using UnityEngine;

namespace Jiang.Games
{
    public partial class InputManager : ViewController
    {
        private SaveSystem _SaveSystem;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            _SaveSystem = this.GetSystem<SaveSystem>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneController.Instance.TransitionToMainScene();
                _SaveSystem.SavePlayerData();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                _SaveSystem.LoadPlayerData();
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                InventoryManager.Instance.SwitchBag();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                QuestUI.Instance.SwitchQuestUI();
            }
        }
    }
}