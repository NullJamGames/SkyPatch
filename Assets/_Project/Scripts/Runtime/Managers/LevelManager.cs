using System;
using NJG.Runtime.LevelChangeSystem;
using NJG.Utilities.Singletons;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Zenject;

namespace NJG.Runtime.Managers
{
    public class LevelManager : Singleton<LevelManager>
    {
        [FoldoutGroup("References"), SerializeField]
        private Volume _levelPPVolume;
        
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

        public void LoadMainMenu() => _levelChangeManager.LoadMenuScene();

        public void LoadNextLevel() => _levelChangeManager.LoadNextScene();

        public void ExitGame() => Application.Quit();
    }
}