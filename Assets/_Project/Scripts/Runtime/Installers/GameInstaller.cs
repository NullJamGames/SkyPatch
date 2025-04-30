using NJG.Runtime.Audio;
using NJG.Runtime.Managers;
using NJG.Runtime.Signals;
using NJG.Utilities;
using UnityEngine;
using Zenject;
using NJG.Runtime.LevelChangeSystem;

namespace NJG.Runtime.Installers
{
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        // [SerializeField, Tooltip("Path to the console prefab in the resources folder.")]
        // private string _consolePath = "Console/QuantumConsole";
        // [SerializeField, Tooltip("Path to the console prefab in the resources folder.")]
        // private string _playerInputReaderPath = "Input/PlayerInputReader";
        [SerializeField, Tooltip("Path to the games audio data in the resources folder.")]
        private string _audioDataPath = "Audio/AudioData";
        // [SerializeField, Tooltip("Path to the GameManagers prefab in the resources folder.")]
        // private string _gameManagersPath = "Managers/GameManagers";
        
        [SerializeField, Tooltip("Level Change system")]
        private LevelChangeManager _levelChangeSystem;
        
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            DeclareSignals();
            
            // Game Manager
            //if (_gameManagerPrefab != null)
            //    Container.Bind<GameManager>().FromComponentInNewPrefab(_gameManagerPrefab.gameObject).AsSingle().NonLazy();
            
            // Game Managers
            // if (Tools.TryLoadResource(_gameManagersPath, out GameObject gameManagersPrefab))
            //     Container.Bind<GameManager>().FromComponentInNewPrefab(gameManagersPrefab).AsSingle().NonLazy();
            
            // Input
            // if (Tools.TryLoadResource(_playerInputReaderPath, out InputReaderSO playerInputReader))
            //     Container.BindInstance(playerInputReader).AsSingle();
            
            // Console
            // if (Tools.TryLoadResource(_consolePath, out GameObject consolePrefab))
            //     Container.Bind<QuantumConsole>().FromComponentInNewPrefab(consolePrefab).AsSingle().NonLazy();
            
            // Audio
            if (Tools.TryLoadResource(_audioDataPath, out AudioDataSO audioData))
            {
                Container.BindInstance(audioData).AsSingle();
                Container.Bind<GameObject>().FromInstance(gameObject).WhenInjectedInto<AudioManager>();
                Container.BindInterfacesAndSelfTo<AudioManager>().AsSingle().NonLazy();
            }
            
            
            // Other - will probably later use a settings SO...
            Container.BindInterfacesAndSelfTo<GameManager>().AsSingle().NonLazy();
            
            Container.Bind<LevelChangeManager>().FromComponentInNewPrefab(_levelChangeSystem).AsSingle().NonLazy();        
        }

        private void DeclareSignals()
        {
            // Cozy
            Container.DeclareSignal<DayTimeChangeSignal>();
        }
    }
}