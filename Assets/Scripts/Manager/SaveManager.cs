using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    private readonly string m_SceneName = "level";

    public string SceneName => PlayerPrefs.GetString(m_SceneName);

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) SceneController.Instance.TransitionToMainScene();

        if (Input.GetKeyDown(KeyCode.S)) SavePlayerData();

        if (Input.GetKeyDown(KeyCode.L)) LoadPlayerData();
    }

    public void SavePlayerData()
    {
        Save(GameManager.Instance.playerCharacterStats.characterData, GameManager.Instance.playerCharacterStats.characterData.name);
    }

    public void LoadPlayerData()
    {
        Load(GameManager.Instance.playerCharacterStats.characterData, GameManager.Instance.playerCharacterStats.characterData.name);
    }

    public void Save(Object data, string key)
    {
        var jsonData = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.SetString(m_SceneName, SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }

    public void Load(Object data, string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
        }
    }
}