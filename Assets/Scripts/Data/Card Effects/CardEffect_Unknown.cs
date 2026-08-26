using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Card Setup/Card Effect/Unknown", fileName = "Card Effect - Unknown")]
public class CardEffect_Unknown : CardEffect_DataSO
{
    public event Action<int> OnValueRevealed;

    public override void ExecuteEffect(ICardEffectContext context, CardEffectArgs args)
    {
        int revealedValue = context.RollNumberCardValue();
        OnValueRevealed?.Invoke(revealedValue);

        context.AddToRunningTotal(revealedValue);

        if (context.RunningTotal > context.RunningTotalLimit)
        {
            context.LoseHeart(args.owner);
            context.ResetRunningTotal();
        }
    }
}
