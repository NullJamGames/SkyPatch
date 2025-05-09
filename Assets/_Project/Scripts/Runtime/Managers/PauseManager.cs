using NJG.Runtime.Input;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace NJG.Runtime.Managers
{
    public class PauseManager : MonoBehaviour
    {
        [FoldoutGroup("References"), SerializeField]
        private InputReader _input;

        [FoldoutGroup("References"), SerializeField]
        private GameObject pauseMenu;

        private GameManager _gameManager;

        private bool _isPaused;

        [Inject]
        private void Construct(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Tab))
                if (_isPaused)
                    Resume();
                else
                    Pause();
        }

        public void Pause()
        {
            _isPaused = true;
            _input.DisablePlayerActions();
            _gameManager.ToggleCursor(true);
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }

        public void Resume()
        {
            _isPaused = false;
            _input.EnablePlayerActions();
            _gameManager.ToggleCursor(false);
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }
    }
}