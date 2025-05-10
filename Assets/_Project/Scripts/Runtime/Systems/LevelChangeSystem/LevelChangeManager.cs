using NJG.Runtime.Scriptables;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace NJG.Runtime.LevelChangeSystem
{
    public class LevelChangeManager : MonoBehaviour
    {
        [SerializeField]
        private LevelChanger _levelChanger;
        
        private LevelHolderSO _levelHolder;

        [Inject]
        void Construct(LevelHolderSO levelHolder)
        {
            _levelHolder = levelHolder;
        }

        public void ReloadScene()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            LoadScene(currentScene);
        }

        public void LoadGameScene()
        {
            LoadScene("3_Level01");
        }
        
        public void LoadGameScene(int levelIndex)
        {
            LoadScene(_levelHolder.GetSceneWithIndex(levelIndex));
        }

        public void LoadMenuScene()
        {
            LoadScene(_levelHolder.GetMenuScene());
        }

        public void LoadNextScene()
        {
            LoadScene(_levelHolder.GetNextScene());
        }

        private void LoadScene(string sceneName)
        {
            _ = _levelChanger.ChangeSceneAsync(sceneName);
        }
    }
}