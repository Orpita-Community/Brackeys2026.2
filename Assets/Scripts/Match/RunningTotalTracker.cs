using System;

public class RunningTotalTracker
{
    public event Action<int> OnRunningTotalChanged;

    public int Value { get; private set; }
    public int Limit { get; } = 21;

    public void Add(int amount)
    {
        Value += amount;
        OnRunningTotalChanged?.Invoke(Value);
    }

    public void Reset()
    {
        Value = 0;
        OnRunningTotalChanged?.Invoke(Value);
    }
}
