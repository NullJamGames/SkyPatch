using NJG.Runtime.LevelChangeSystem;
using NJG.Runtime.Managers;
using NJG.Runtime.Scriptables;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace NJG.Runtime.UI
{
    public class LevelSelectionPanel : MonoBehaviour
    {
        [FoldoutGroup("References")]
        [FoldoutGroup("References/Scene"), SerializeField]
        private Transform _elementContainer;
        
        [FoldoutGroup("References/Prefab"), SerializeField]
        private LevelSelectionElement _levelSelectionElementPrefab;
        
        private LevelChangeManager _levelChangeManager;
        private LevelHolderSO _levelHolder;
        private SaveManager _saveManager;
        
        [Inject]
        void Construct(LevelHolderSO levelHolder,  LevelChangeManager levelChangeManager, SaveManager saveManager)
        {
            _levelHolder = levelHolder;
            _levelChangeManager = levelChangeManager;
            _saveManager = saveManager;
        }

        private void Start()
        {
            InitializeSelectionElements();
        }

        private void InitializeSelectionElements()
        {
            int levelCount = _levelHolder.NumberOfLevels;
            
            print(levelCount);

            for (int i = 0; i < levelCount; i++)
                InitializeElement(i);
        }

        private void InitializeElement(int levelIndex)
        {
            LevelSelectionElement element = Instantiate(_levelSelectionElementPrefab, _elementContainer);
            
            element.InitializeElement((GetLevelData(levelIndex)), OnLevelSelected);
        }

        private SLevelData GetLevelData(int levelIndex)
        {
            SLevelData levelData = new SLevelData();
            levelData.LevelIndex = levelIndex;
            levelData.Islocked = !_saveManager.IsLevelUnlocked(levelIndex);
            return levelData;
        }

        private void OnLevelSelected(int levelIndex)
        {
            _levelChangeManager.LoadGameScene(levelIndex);
        }
    }
    
    public struct SLevelData
    {
        public int LevelIndex;
        public bool Islocked;
    }
}
