using NJG.Runtime.LevelChangeSystem;
using NJG.Utilities.Singletons;
using Sirenix.OdinInspector;
using UnityEngine;
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

        [Button(ButtonSizes.Large)]
        private void TestNextScene()
        {
            _levelChangeManager.LoadScene(_nextLevelName);
        }
    }
}