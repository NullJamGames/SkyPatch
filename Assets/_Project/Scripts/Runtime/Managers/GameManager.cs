using NJG.Runtime.Audio;
using UnityEngine;
using Zenject;

namespace NJG.Runtime.Managers
{
    public class GameManager : IInitializable, ILateDisposable
    {
        private readonly AudioManager _audioManager;
        
        public GameManager(AudioManager audioManager) => _audioManager = audioManager;

        public void Initialize() => _audioManager.PlayPersistent(_audioManager.AudioData.Music);

        public void LateDispose() { }

        public void ToggleCursor(bool isVisible)
        {
            Cursor.visible = isVisible;
            Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}