using System.Linq;
using UnityEngine;

public class UI : MonoBehaviour
{
    public HandUI PlayerHandUI { get; private set; }
    public HandUI ReaperHandUI { get; private set; }
    public HeartsUI PlayerHeartsUI { get; private set; }
    public HeartsUI ReaperHeartsUI { get; private set; }
    public RunningTotalUI RunningTotalUI { get; private set; }
    public TurnBannerUI TurnBannerUI { get; private set; }

    private void Awake()
    {
        HandUI[] hands = GetComponentsInChildren<HandUI>(true);
        PlayerHandUI = hands.FirstOrDefault(h => h.owner == CardOwner.Player);
        ReaperHandUI = hands.FirstOrDefault(h => h.owner == CardOwner.Reaper);

        HeartsUI[] hearts = GetComponentsInChildren<HeartsUI>(true);
        PlayerHeartsUI = hearts.FirstOrDefault(h => h.owner == CardOwner.Player);
        ReaperHeartsUI = hearts.FirstOrDefault(h => h.owner == CardOwner.Reaper);

        RunningTotalUI = GetComponentInChildren<RunningTotalUI>(true);
        TurnBannerUI = GetComponentInChildren<TurnBannerUI>(true);
    }
}
