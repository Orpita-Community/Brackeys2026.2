using UnityEngine;

[CreateAssetMenu(menuName = "Card Setup/Card Effect/Stop", fileName = "Card Effect - Stop")]
public class CardEffect_Stop : CardEffect_DataSO
{
    public override void ExecuteEffect(ICardEffectContext context, CardEffectArgs args)
    {
        CardOwner opponent = context.GetOpponent(args.owner);
        context.SkipNextTurn(opponent);
    }
}
