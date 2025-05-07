namespace Snowy.AI.StateMachine
{
    public interface IState
    {
        void OnEnter();
        void OnExit();
        void OnUpdate();
        void OnFixedUpdate();
    }
}