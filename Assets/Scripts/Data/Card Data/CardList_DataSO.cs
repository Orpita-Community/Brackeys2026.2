using UnityEngine;

[CreateAssetMenu(menuName = "Card Setup/Card List", fileName = "Card List - ")]
public class CardList_DataSO : ScriptableObject
{
    public Card_DataSO[] cardList;
}
