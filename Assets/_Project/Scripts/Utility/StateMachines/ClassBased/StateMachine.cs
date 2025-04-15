namespace NJG.Utilities
{
    public class StateMachine<T> where T : BaseState<T>
    {
        public T CurrentState { get; private set; }
        public T PreviousState { get; private set; }

        public void Initialize(T startState)
        {
            CurrentState = startState;
            CurrentState.Enter();
        }

        public void ChangeState(T newState)
        {
            CurrentState.Exit();
            PreviousState = CurrentState;
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}