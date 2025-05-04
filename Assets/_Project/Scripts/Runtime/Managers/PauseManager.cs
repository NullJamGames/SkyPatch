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
        
        private bool _isPaused = false;
        
        private GameManager _gameManager;

        [Inject]
        void Construct(GameManager gameManager)
        {
           _gameManager = gameManager; 
        }
        
        void Update()
        {
            if(UnityEngine.Input.GetKeyDown(KeyCode.Tab))
                if(_isPaused)
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
