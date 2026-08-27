using UnityEngine;

[CreateAssetMenu(menuName = "Card Setup/Card Effect/Reset", fileName = "Card Effect - Reset")]
public class CardEffect_Reset : CardEffect_DataSO
{
    public override void ExecuteEffect(ICardEffectContext context, CardEffectArgs args)
    {
        context.ResetRunningTotal();
    }
}
