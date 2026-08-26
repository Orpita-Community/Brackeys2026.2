using TMPro;
using UnityEngine;

public class RunningTotalUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalText;
    private RoundManager roundManager;

    private void Awake()
    {
        roundManager = FindAnyObjectByType<RoundManager>();
        roundManager.Total.OnRunningTotalChanged += Refresh;
        Refresh(roundManager.Total.Value);
    }

    private void OnDestroy()
    {
        if (roundManager != null) roundManager.Total.OnRunningTotalChanged -= Refresh;
    }

    private void Refresh(int value)
    {
        if (totalText != null)
            totalText.text = $"Total: {value} / {roundManager.Total.Limit}";
    }
}
