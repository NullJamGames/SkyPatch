using UnityEngine;
using Zenject;

namespace NJG.Runtime.Managers
{
    public class GameManager : IInitializable, ILateDisposable
    {
        public GameObject Player { get; private set; }

        public void Initialize() { }

        public void LateDispose() { }

        public void ToggleCursor(bool isVisible)
        {
            Cursor.visible = isVisible;
            Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}