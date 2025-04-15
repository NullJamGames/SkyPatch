namespace NJG.Utilities
{
    public abstract class BaseState<T> where T : BaseState<T>
    {
        protected StateMachine<T> _stateMachine;

        protected BaseState(StateMachine<T> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public virtual void Enter() { }
        public virtual void LogicUpdate() { }
        public virtual void PhysicsUpdate() { }
        public virtual void Exit() { }
    }
}