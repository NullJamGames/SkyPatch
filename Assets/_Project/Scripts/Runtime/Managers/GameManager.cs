using System;
using NJG.Runtime.Input;
using NJG.Utilities.Singletons;

namespace NJG.Runtime.Managers
{
    public class GameManager : SingletonPersistent<GameManager>
    {
        //private PlayerInputActions _playerInputActions;
        //private InputReader _inputReader;

        public override void Awake()
        {
            base.Awake();
            
            //_playerInputActions = new PlayerInputActions();
            //_inputReader = new InputReader();
        }

        // private void OnEnable()
        // {
        //     _playerInputActions.Enable();
        // }
        //
        // private void OnDisable()
        // {
        //     _playerInputActions.Disable();
        // }
        //
        // public IInputReader GetInputProvider()
        // {
        //     return _inputReader;
        // }
    }
}