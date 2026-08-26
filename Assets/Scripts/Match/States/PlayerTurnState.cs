public class PlayerTurnState : MatchState
{
    public PlayerTurnState(RoundManager roundManager, StateMachine stateMachine) : base(roundManager, stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        if (!roundManager.PrepareTurn(CardOwner.Player)) return;

        if (roundManager.ConsumeForceRandom(CardOwner.Player))
        {
            roundManager.PlayRandomLegalCard(CardOwner.Player);
            return;
        }

        roundManager.SetPlayerInputEnabled(true);
    }

    public override void Exit()
    {
        base.Exit();
        roundManager.SetPlayerInputEnabled(false);
    }
}
