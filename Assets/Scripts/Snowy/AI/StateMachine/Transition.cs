namespace Snowy.AI.StateMachine
{
    public class Transition : ITransition
    {
        public IPredicate Condition { get; }
        public IState To { get; }

        public Transition(IState to, IPredicate condition)
        {
            Condition = condition;
            To = to;
        }
    }
}