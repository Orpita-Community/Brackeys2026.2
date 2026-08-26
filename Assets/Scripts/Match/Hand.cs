using System;
using System.Collections.Generic;

public class Hand
{
    public event Action OnHandChanged;

    public CardOwner Owner { get; }
    public List<Card_DataSO> Cards { get; } = new List<Card_DataSO>();
    public bool IsEmpty => Cards.Count == 0;

    public Hand(CardOwner owner)
    {
        Owner = owner;
    }

    public void SetHand(List<Card_DataSO> cards)
    {
        Cards.Clear();
        Cards.AddRange(cards);
        OnHandChanged?.Invoke();
    }

    public void AddCard(Card_DataSO card)
    {
        if (card == null) return;
        Cards.Add(card);
        OnHandChanged?.Invoke();
    }

    public void RemoveCard(Card_DataSO card)
    {
        if (card == null) return;
        Cards.Remove(card);
        OnHandChanged?.Invoke();
    }

    public bool Contains(Card_DataSO card) => Cards.Contains(card);
}
