using QFramework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jiang.Games
{
    public class SaveSystem : AbstractSystem
    {
        private const string SCENE_NAME = "level";
        public string SceneName => PlayerPrefs.GetString(SCENE_NAME);

        protected override void OnInit() { }

        public void Save(Object data, string key)
        {
            var jsonData = JsonUtility.ToJson(data, true);
            PlayerPrefs.SetString(key, jsonData);
            PlayerPrefs.SetString(SCENE_NAME, SceneManager.GetActiveScene().name);
            PlayerPrefs.Save();
        }

        public void Load(Object data, string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
            }
        }

        public void SavePlayerData()
        {
            Save(GameManager.Instance.playerCharacterStats.characterData, GameManager.Instance.playerCharacterStats.characterData.name);
        }

        public void LoadPlayerData()
        {
            Load(GameManager.Instance.playerCharacterStats.characterData, GameManager.Instance.playerCharacterStats.characterData.name);
        }
    }
}