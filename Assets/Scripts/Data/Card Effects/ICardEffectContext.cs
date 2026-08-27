using System.Collections.Generic;

public interface ICardEffectContext
{
    int RunningTotal { get; }
    int RunningTotalLimit { get; }
    void AddToRunningTotal(int amount);
    void ResetRunningTotal();

    void LoseHeart(CardOwner owner);

    CardOwner GetOpponent(CardOwner owner);

    IReadOnlyList<Card_DataSO> GetHand(CardOwner owner);
    void MoveCard(Card_DataSO card, CardOwner from, CardOwner to);
    Card_DataSO GetRandomCard(CardOwner owner);

    void SkipNextTurn(CardOwner owner);
    void ForceRandomNextMove(CardOwner owner);

    int RollNumberCardValue();
}
