using UnityEngine;

[CreateAssetMenu(menuName = "Card Setup/Card Effect/Light Source", fileName = "Card Effect - Light Source")]
public class CardEffect_LightSource : CardEffect_DataSO
{
    public override void ExecuteEffect(ICardEffectContext context, CardEffectArgs args)
    {
        CardOwner opponent = context.GetOpponent(args.owner);
        context.ForceRandomNextMove(opponent);
    }
}
