namespace Snowy.AI.StateMachine
{
    public abstract class BaseState : IState
    {
        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void OnUpdate();
        public abstract void OnFixedUpdate();
    }
}