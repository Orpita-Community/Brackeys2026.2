using System.Collections;
using UnityEngine;

public class ReaperTurnState : MatchState
{
    public ReaperTurnState(RoundManager roundManager, StateMachine stateMachine) : base(roundManager, stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        if (!roundManager.PrepareTurn(CardOwner.Reaper)) return;

        if (roundManager.ConsumeForceRandom(CardOwner.Reaper))
        {
            roundManager.PlayRandomLegalCard(CardOwner.Reaper);
            return;
        }

        roundManager.StartCoroutine(AITurnRoutine());
    }

    private IEnumerator AITurnRoutine()
    {
        yield return new WaitForSeconds(roundManager.AiThinkDelaySeconds);

        var (card, selectedCard) = roundManager.ReaperAI.ChooseMove(
            roundManager.GetHand(CardOwner.Reaper), roundManager, CardOwner.Reaper);

        roundManager.TryPlayCard(CardOwner.Reaper, card, selectedCard);
    }
}
