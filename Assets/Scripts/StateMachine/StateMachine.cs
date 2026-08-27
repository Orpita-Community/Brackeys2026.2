public class StateMachine
{
    public MatchState CurrentState { get; private set; }

    public void Initialize(MatchState startState)
    {
        CurrentState = startState;
        CurrentState.Enter();
    }

    public void ChangeState(MatchState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}
