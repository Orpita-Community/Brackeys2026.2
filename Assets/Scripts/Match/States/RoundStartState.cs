public class RoundStartState : MatchState
{
    public RoundStartState(RoundManager roundManager, StateMachine stateMachine) : base(roundManager, stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        roundManager.ResetRunningTotal();
        roundManager.BuildAndDealHands();

        CardOwner starter = roundManager.ConsumeRoundStarter();
        roundManager.RaiseRoundStarted(starter);
        roundManager.GoToTurnState(starter);
    }
}
