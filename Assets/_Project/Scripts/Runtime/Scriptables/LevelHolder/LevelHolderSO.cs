using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NJG.Runtime.Scriptables
{
    [CreateAssetMenu(menuName = "NJG/LevelHolder")]
    public class LevelHolderSO : ScriptableObject
    {
        [FoldoutGroup("NonGame"), SerializeField]
        private string _startMenu;
        [FoldoutGroup("NonGame"), SerializeField]
        private string _endLevel;
        
        [FoldoutGroup("Game"), SerializeField]
        private string[] _levels;

        public int NumberOfLevels => _levels.Length;
        
        public string GetNextScene()
        {
            return GetNextScene(SceneManager.GetActiveScene().name);
        }

        public string GetNextScene(string currentScene)
        {
            for(int i = 0; i < _levels.Length -1; i++)
                if(_levels[i] == currentScene)
                    return _levels[i+1];
            
            if(_levels[NumberOfLevels - 1] == currentScene)
                return _endLevel;

            Debug.LogError("Current scene is not in levels hierarchy");
            return _endLevel;
        }
        
        public string GetSceneWithIndex(int index)
        {
            if(_levels.Length > index)
                return _levels[index];
            Debug.LogError("wanted scene is not in levels hierarchy");
            return _endLevel;
        }

        public string GetEndScene()
        {
            return _endLevel;
        }

        public string GetMenuScene()
        {
            return _startMenu;
        }
    }
}