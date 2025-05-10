using NJG.Runtime.LevelChangeSystem;
using UnityEngine;
using Zenject;

namespace NJG.Runtime.Managers
{
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField]
        private string _nextSceneName = "1_Splash";

        private LevelChangeManager _levelManager;

        [Inject]
        private void Construct(LevelChangeManager levelManager) => _levelManager = levelManager;

        private void Start() => OnGameStarted();

        private void OnGameStarted() => _levelManager.LoadScene(_nextSceneName);
    }
}