namespace NJG.Runtime.StateSystem
{
    public interface ITransition
    {
        public IState To { get; }
        public IPredicate Condition { get; }
    }
}