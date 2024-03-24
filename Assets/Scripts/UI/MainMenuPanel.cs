using UnityEngine;

namespace QFramework.Example
{
    public class MainMenuPanelData : UIPanelData { }

    public partial class MainMenuPanel : UIPanel
    {
        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as MainMenuPanelData ?? new MainMenuPanelData();

            // please add init code here

            Btn_NewGame.onClick.AddListener(() =>
            {
                PlayerPrefs.DeleteAll();
                Hide();
                SceneController.Instance.TransitionToFirstLevel();
            });

            Btn_Continue.onClick.AddListener(() =>
            {
                Hide();
                SceneController.Instance.TransitionToLoadGame();
            });

            Btn_Quit.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                Debug.Log("------Quit Game ------");
#endif
                Application.Quit();
            });
        }

        protected override void OnOpen(IUIData uiData = null) { }

        protected override void OnShow() { }

        protected override void OnHide() { }

        protected override void OnClose() { }
    }
}