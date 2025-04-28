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

        private void LoadScene(string sceneName)
        {
            _levelChanger.ChangeScene(sceneName);
        }
    }
}
