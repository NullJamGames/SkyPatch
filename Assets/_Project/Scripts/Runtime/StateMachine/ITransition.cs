namespace NJG.Runtime.StateMachine
{
    public interface ITransition
    {
        public IState To { get; }
        public IPredicate Condition { get; }
    }
}