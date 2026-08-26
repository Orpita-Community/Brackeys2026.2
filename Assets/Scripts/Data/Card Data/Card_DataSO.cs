using UnityEngine;

[CreateAssetMenu(menuName = "Card Setup/Card Data", fileName = "Card Data - ")]
public class Card_DataSO : ScriptableObject
{
    [Header("Card Details")]
    public string cardName;
    public Sprite cardIcon;
    public CardType cardType;

    [Header("Number Value")]
    [Tooltip("Used only when Card Type is Number. Special cards ignore this.")]
    [Range(0, 7)]
    public int numberValue;

    [Header("Deck Composition")]
    [Tooltip("How many copies of this card are shuffled into the prototype deck.")]
    public int copiesInDeck = 1;

    [Header("Card Effect")]
    [Tooltip("Special cards assign their behavior here. Leave empty for plain number cards.")]
    public CardEffect_DataSO cardEffect;
}
