using TMPro;
using UnityEngine;

public class TurnBannerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bannerText;
    private RoundManager roundManager;

    private void Awake()
    {
        roundManager = FindAnyObjectByType<RoundManager>();
        roundManager.OnTurnStarted += HandleTurnStarted;
        roundManager.OnRoundStarted += HandleRoundStarted;
        roundManager.OnRoundEnded += HandleRoundEnded;
        roundManager.OnMatchEnded += HandleMatchEnded;
    }

    private void OnDestroy()
    {
        if (roundManager == null) return;
        roundManager.OnTurnStarted -= HandleTurnStarted;
        roundManager.OnRoundStarted -= HandleRoundStarted;
        roundManager.OnRoundEnded -= HandleRoundEnded;
        roundManager.OnMatchEnded -= HandleMatchEnded;
    }

    private void HandleTurnStarted(CardOwner owner) => SetText($"{OwnerName(owner)}'s Turn");
    private void HandleRoundStarted(CardOwner starter) => SetText($"{OwnerName(starter)} Starts the Round");
    private void HandleRoundEnded(CardOwner winner) => SetText($"{OwnerName(winner)} Wins the Round!");
    private void HandleMatchEnded(CardOwner loser) => SetText($"{OwnerName(roundManager.GetOpponent(loser))} Wins the Match!");

    private void SetText(string text)
    {
        if (bannerText != null) bannerText.text = text;
    }

    private static string OwnerName(CardOwner owner) => owner == CardOwner.Player ? "Player" : "Reaper";
}
