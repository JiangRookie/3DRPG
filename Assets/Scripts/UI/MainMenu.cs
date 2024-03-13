using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private Button m_NewGameButton;
    private Button m_ContinueGameButton;
    private Button m_QuitGame;

    private void Awake()
    {
        m_NewGameButton = transform.GetChild(1).GetComponent<Button>();
        m_ContinueGameButton = transform.GetChild(2).GetComponent<Button>();
        m_QuitGame = transform.GetChild(3).GetComponent<Button>();

        m_NewGameButton.onClick.AddListener(NewGame);
        m_ContinueGameButton.onClick.AddListener(ContinueGame);
        m_QuitGame.onClick.AddListener(QuitGame);
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

    private void QuitGame() => Application.Quit();
}