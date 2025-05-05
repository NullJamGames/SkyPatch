using NJG.Runtime.Entities;
using NJG.Runtime.Input;
using UnityEngine;

namespace NJG.Utilities.PredicateStateMachines
{
    public interface IState
    {
        public void OnEnter();
        public void OnUpdate(float deltaTime);
        public void OnFixedUpdate(float fixedDeltaTime);
        public void OnExit();
    }
}