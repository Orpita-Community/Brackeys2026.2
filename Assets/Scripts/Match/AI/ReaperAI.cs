using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ReaperAI
{
    [SerializeField] private int closeToLimitThreshold = 18;
    [SerializeField] private int lowValueThreshold = 2;

    public (Card_DataSO card, Card_DataSO selectedCard) ChooseMove(IReadOnlyList<Card_DataSO> hand, ICardEffectContext context, CardOwner owner)
    {
        List<Card_DataSO> safeNumbers = hand
            .Where(c => c.cardType == CardType.Number && context.RunningTotal + c.numberValue <= context.RunningTotalLimit)
            .OrderByDescending(c => c.numberValue)
            .ToList();

        bool cornered = context.RunningTotal >= closeToLimitThreshold &&
                         (safeNumbers.Count == 0 || safeNumbers[0].numberValue <= lowValueThreshold);

        Card_DataSO resetCard = hand.FirstOrDefault(c => c.cardEffect is CardEffect_Reset);
        if (cornered && resetCard != null && CardRules.IsGenerallyPlayable(resetCard, hand, context, owner))
            return (resetCard, null);

        if (safeNumbers.Count > 0)
            return (safeNumbers[0], null);

        Card_DataSO stopCard = hand.FirstOrDefault(c => c.cardEffect is CardEffect_Stop
            && CardRules.IsGenerallyPlayable(c, hand, context, owner));
        if (stopCard != null) return (stopCard, null);

        Card_DataSO lightSourceCard = hand.FirstOrDefault(c => c.cardEffect is CardEffect_LightSource
            && CardRules.IsGenerallyPlayable(c, hand, context, owner));
        if (lightSourceCard != null) return (lightSourceCard, null);

        Card_DataSO swapCard = hand.FirstOrDefault(c => c.cardEffect is CardEffect_Swap
            && CardRules.IsGenerallyPlayable(c, hand, context, owner));
        if (swapCard != null) return (swapCard, ChooseGiveawayCard(hand, swapCard));

        Card_DataSO unknownCard = hand.FirstOrDefault(c => c.cardEffect is CardEffect_Unknown
            && CardRules.IsGenerallyPlayable(c, hand, context, owner));
        if (unknownCard != null) return (unknownCard, null);

        if (resetCard != null && CardRules.IsGenerallyPlayable(resetCard, hand, context, owner))
            return (resetCard, null);

        return (null, null);
    }

    private Card_DataSO ChooseGiveawayCard(IReadOnlyList<Card_DataSO> hand, Card_DataSO swapCard)
    {
        List<Card_DataSO> others = hand.Where(c => c != swapCard).ToList();

        Card_DataSO lowestNumber = others
            .Where(c => c.cardType == CardType.Number)
            .OrderBy(c => c.numberValue)
            .FirstOrDefault();

        return lowestNumber != null ? lowestNumber : others.FirstOrDefault();
    }
}
