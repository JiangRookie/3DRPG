using QFramework;
using UnityEngine;

namespace Jiang.Games
{
    public partial class MainMenuPanel : ViewController
    {
        void Start()
        {
            Btn_NewGame.onClick.AddListener(NewGame);
            Btn_Continue.onClick.AddListener(ContinueGame);
            Btn_Quit.onClick.AddListener(QuitGame);
        }

        private void OnDestroy()
        {
            Btn_NewGame.onClick.RemoveListener(NewGame);
            Btn_Continue.onClick.RemoveListener(ContinueGame);
            Btn_Quit.onClick.RemoveListener(QuitGame);
        }

        private void NewGame()
        {
            PlayerPrefs.DeleteAll();
            SceneController.Instance.TransitionToFirstLevel();
        }

        private void ContinueGame()
        {
            SceneController.Instance.TransitionToLoadGame();
        }

        private void QuitGame()
        {
            Application.Quit();
        }
    }
}