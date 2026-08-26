using UnityEngine;

public abstract class CardEffect_DataSO : ScriptableObject
{
    [TextArea]
    public string effectDescription;

    public virtual bool CanBeUsed(ICardEffectContext context, CardEffectArgs args)
    {
        return true;
    }

    public abstract void ExecuteEffect(ICardEffectContext context, CardEffectArgs args);
}
