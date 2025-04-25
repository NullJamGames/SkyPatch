namespace NJG.Utilities.PredicateStateMachines
{
    public interface ITransition
    {
        public IState To { get; }
        public IPredicate Condition { get; }
    }
}