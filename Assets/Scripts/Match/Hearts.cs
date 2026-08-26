using System;

public class Hearts
{
    public event Action<int> OnHeartsChanged;

    public CardOwner Owner { get; }
    public int Max { get; }
    public int Current { get; private set; }
    public bool IsDepleted => Current <= 0;

    public Hearts(CardOwner owner, int max)
    {
        Owner = owner;
        Max = max;
        Current = max;
    }

    public void LoseHeart()
    {
        if (Current <= 0) return;
        Current--;
        OnHeartsChanged?.Invoke(Current);
    }
}
