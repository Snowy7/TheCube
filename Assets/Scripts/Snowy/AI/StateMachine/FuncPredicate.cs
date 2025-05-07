namespace Snowy.AI.StateMachine
{
    public class FuncPredicate : IPredicate
    {
        readonly System.Func<bool> func;
        
        public FuncPredicate(System.Func<bool> func) => this.func = func;
        
        public bool Evaluate() => func();
    }
}