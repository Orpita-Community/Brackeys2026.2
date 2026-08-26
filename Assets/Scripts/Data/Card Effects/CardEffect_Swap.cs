using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Card Setup/Card Effect/Swap", fileName = "Card Effect - Swap")]
public class CardEffect_Swap : CardEffect_DataSO
{
    public override bool CanBeUsed(ICardEffectContext context, CardEffectArgs args)
    {
        return args.selectedCard != null && context.GetHand(args.owner).Contains(args.selectedCard);
    }

    public override void ExecuteEffect(ICardEffectContext context, CardEffectArgs args)
    {
        CardOwner opponent = context.GetOpponent(args.owner);
        Card_DataSO givenCard = args.selectedCard;
        Card_DataSO receivedCard = context.GetRandomCard(opponent);

        context.MoveCard(givenCard, args.owner, opponent);
        context.MoveCard(receivedCard, opponent, args.owner);
    }
}
