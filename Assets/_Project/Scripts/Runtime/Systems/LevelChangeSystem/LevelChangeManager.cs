using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NJG.Runtime.LevelChangeSystem
{
	public class LevelChangeManager : MonoBehaviour
	{
		[SerializeField] private LevelChanger _levelChanger;

        public void ReloadScene()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            LoadScene(currentScene);
        }

        public void LoadGameScene()
        {
            LoadScene("Level001");
        }

        public void LoadMenuScene()
        {
            LoadScene("Menu");
        }

        private void LoadScene(string sceneName)
        {
            _ = _levelChanger.ChangeSceneAsync(sceneName);
        }
    }
}
