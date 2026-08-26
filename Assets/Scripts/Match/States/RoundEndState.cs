using System.Collections;
using UnityEngine;

public class RoundEndState : MatchState
{
    private const float RoundEndDelaySeconds = 2f;

    public RoundEndState(RoundManager roundManager, StateMachine stateMachine) : base(roundManager, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        roundManager.StartCoroutine(DelayThenNextRound());
    }

    private IEnumerator DelayThenNextRound()
    {
        yield return new WaitForSeconds(RoundEndDelaySeconds);
        roundManager.GoToRoundStart();
    }
}
