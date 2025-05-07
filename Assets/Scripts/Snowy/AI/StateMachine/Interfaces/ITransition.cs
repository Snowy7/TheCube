namespace Snowy.AI.StateMachine
{
    public interface ITransition
    {
        IPredicate Condition { get; }
        IState To { get; }
    }
}