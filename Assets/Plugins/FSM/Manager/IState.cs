
namespace FSM
{
    public interface IState
    {
        void StartState();
		void UpdateState(float dt);
        void ExitState();
    }
}
