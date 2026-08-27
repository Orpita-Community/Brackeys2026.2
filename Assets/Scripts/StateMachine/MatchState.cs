public abstract class MatchState
{
    protected readonly RoundManager roundManager;
    protected readonly StateMachine stateMachine;

    protected MatchState(RoundManager roundManager, StateMachine stateMachine)
    {
        this.roundManager = roundManager;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
