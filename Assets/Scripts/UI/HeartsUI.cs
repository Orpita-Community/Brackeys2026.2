using TMPro;
using UnityEngine;

public class HeartsUI : MonoBehaviour
{
    public CardOwner owner;
    [SerializeField] private TextMeshProUGUI heartsCountText;

    private HeartSlot[] slots;
    private Hearts heartsModel;

    private void Awake()
    {
        slots = GetComponentsInChildren<HeartSlot>(true);
        RoundManager roundManager = FindAnyObjectByType<RoundManager>();
        heartsModel = owner == CardOwner.Player ? roundManager.PlayerHearts : roundManager.ReaperHearts;
        heartsModel.OnHeartsChanged += Refresh;
        Refresh(heartsModel.Current);
    }

    private void OnDestroy()
    {
        if (heartsModel != null) heartsModel.OnHeartsChanged -= Refresh;
    }

    private void Refresh(int current)
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i].SetFilled(i < current);

        if (heartsCountText != null)
            heartsCountText.text = $"{current}/{heartsModel.Max}";
    }
}
