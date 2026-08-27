using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private readonly List<Card_DataSO> cards = new List<Card_DataSO>();

    public int RemainingCount => cards.Count;

    public void Build(CardList_DataSO cardList)
    {
        cards.Clear();
        foreach (Card_DataSO print in cardList.cardList)
        {
            for (int i = 0; i < print.copiesInDeck; i++)
            {
                cards.Add(print);
            }
        }
    }

    public void Shuffle()
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (cards[i], cards[j]) = (cards[j], cards[i]);
        }
    }

    public List<Card_DataSO> Draw(int count)
    {
        count = Mathf.Min(count, cards.Count);
        List<Card_DataSO> drawn = cards.GetRange(0, count);
        cards.RemoveRange(0, count);
        return drawn;
    }

    public static int RollWeightedNumberValue(CardList_DataSO cardList)
    {
        int totalWeight = 0;
        foreach (Card_DataSO print in cardList.cardList)
        {
            if (print.cardType == CardType.Number)
                totalWeight += print.copiesInDeck;
        }

        if (totalWeight <= 0) return 0;

        int roll = Random.Range(0, totalWeight);
        int cumulative = 0;
        foreach (Card_DataSO print in cardList.cardList)
        {
            if (print.cardType != CardType.Number) continue;
            cumulative += print.copiesInDeck;
            if (roll < cumulative)
                return print.numberValue;
        }

        return 0;
    }
}
