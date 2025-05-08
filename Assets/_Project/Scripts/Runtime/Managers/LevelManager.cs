using NJG.Runtime.LevelChangeSystem;
using NJG.Utilities.Singletons;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace NJG.Runtime.Managers
{
    public class LevelManager : Singleton<LevelManager>
    {
        [FoldoutGroup("Scene References"), SerializeField]
        private string _nextLevelName = "";
        [FoldoutGroup("Scene References"), SerializeField]
        private string _mainMenuName = "2_MainMenu";

        private GameManager _gameManager;
        private LevelChangeManager _levelChangeManager;

        [Inject]
        private void Construct(GameManager gameManager, LevelChangeManager levelChangeManager)
        {
            _gameManager = gameManager;
            _levelChangeManager = levelChangeManager;
        }

        public void Start()
        {
            _gameManager.ToggleCursor(false);
        }

        public void RestartLevel()
        {
            // TODO: Should this be incorporated into level change manager?
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void LoadMainMenu() => _levelChangeManager.LoadScene(_nextLevelName);

        public void LoadNextLevel() => _levelChangeManager.LoadScene(_nextLevelName);

        public void ExitGame() => Application.Quit();
    }
}