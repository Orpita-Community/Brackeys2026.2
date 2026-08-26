using System.Collections.Generic;

public static class CardRules
{
    public static bool IsGenerallyPlayable(Card_DataSO card, IReadOnlyList<Card_DataSO> hand, ICardEffectContext context, CardOwner owner)
    {
        if (card.cardType == CardType.Number)
            return context.RunningTotal + card.numberValue <= context.RunningTotalLimit;

        if (card.cardEffect == null)
            return true;

        if (card.cardEffect is CardEffect_Swap)
            return hand.Count >= 2;

        return card.cardEffect.CanBeUsed(context, new CardEffectArgs { owner = owner, playedCard = card, selectedCard = null });
    }

    public static bool HasAnyLegalPlay(IReadOnlyList<Card_DataSO> hand, ICardEffectContext context, CardOwner owner)
    {
        foreach (Card_DataSO card in hand)
        {
            if (IsGenerallyPlayable(card, hand, context, owner))
                return true;
        }
        return false;
    }

    public static bool IsPlayable(Card_DataSO card, Card_DataSO selectedCard, ICardEffectContext context, CardOwner owner)
    {
        if (card.cardType == CardType.Number)
            return context.RunningTotal + card.numberValue <= context.RunningTotalLimit;

        if (card.cardEffect == null)
            return true;

        return card.cardEffect.CanBeUsed(context, new CardEffectArgs { owner = owner, playedCard = card, selectedCard = selectedCard });
    }
}
