using System.Collections;
using UnityEngine;

public class TablePlayAreaUI : MonoBehaviour
{
    [SerializeField] private CardView cardViewPrefab;
    [SerializeField] private RectTransform playerDropSpot;
    [SerializeField] private RectTransform reaperDropSpot;
    [SerializeField] private RectTransform playerHandOrigin;
    [SerializeField] private RectTransform reaperHandOrigin;

    [Header("Toss Animation")]
    [SerializeField] private float flightDuration = 0.4f;
    [SerializeField] private float maxTossRotationDegrees = 10f;

    private RoundManager roundManager;
    private RectTransform selfRect;

    private CardView playerTableCard;
    private CardView reaperTableCard;
    private Coroutine playerFlightRoutine;
    private Coroutine reaperFlightRoutine;
    private Coroutine pairClearRoutine;

    private void Awake()
    {
        selfRect = (RectTransform)transform;
        roundManager = FindAnyObjectByType<RoundManager>();
        roundManager.OnCardPlayed += HandleCardPlayed;
        roundManager.OnRoundEnded += HandleRoundEnded;
        roundManager.OnRoundStarted += HandleRoundStarted;
    }

    private void OnDestroy()
    {
        if (roundManager != null)
        {
            roundManager.OnCardPlayed -= HandleCardPlayed;
            roundManager.OnRoundEnded -= HandleRoundEnded;
            roundManager.OnRoundStarted -= HandleRoundStarted;
        }
    }

    // Fires synchronously, in the same call stack as RoundManager.TryPlayCard deciding the
    // round (the instant a hand goes empty) — clearing happens here and now, before that
    // call stack even unwinds. Since Unity is single-threaded and no turn/pick can start
    // until later (RoundEndState's ~2s banner, then RoundStartState dealing new hands), this
    // guarantees the table is already empty long before either side's next turn.
    private void HandleRoundEnded(CardOwner winner)
    {
        ClearSide(CardOwner.Player);
        ClearSide(CardOwner.Reaper);
    }

    // Defensive backstop: guarantees the table is never left showing stale cards into a new
    // round even if that round somehow started without OnRoundEnded firing first.
    private void HandleRoundStarted(CardOwner starter)
    {
        ClearSide(CardOwner.Player);
        ClearSide(CardOwner.Reaper);
    }

    private void HandleCardPlayed(CardOwner owner, Card_DataSO card)
    {
        if (card == null) return;

        RectTransform dropSpot = owner == CardOwner.Player ? playerDropSpot : reaperDropSpot;
        RectTransform handOrigin = owner == CardOwner.Player ? playerHandOrigin : reaperHandOrigin;

        ClearSide(owner);

        CardView view = Instantiate(cardViewPrefab, selfRect);
        view.SetCardData(card, true);

        Vector2 startPos = WorldPointToLocalAnchoredPosition(handOrigin.position, selfRect);
        Vector2 endPos = dropSpot.anchoredPosition;
        float endRot = Random.Range(-maxTossRotationDegrees, maxTossRotationDegrees);

        view.SetLayout(startPos, 0f);

        Coroutine routine = StartCoroutine(FlightRoutine(view, startPos, endPos, 0f, endRot));
        if (owner == CardOwner.Player) { playerTableCard = view; playerFlightRoutine = routine; }
        else { reaperTableCard = view; reaperFlightRoutine = routine; }

        // A "round" of the table display is one exchange: one card down from each side,
        // whichever order they land in. Once both spots are occupied, that exchange is
        // decided — let both cards sit for a beat, then sweep the table clear so the next
        // exchange starts on two empty spots.
        if (playerTableCard != null && reaperTableCard != null)
        {
            if (pairClearRoutine != null) StopCoroutine(pairClearRoutine);
            pairClearRoutine = StartCoroutine(ClearBothAfterExchange());
        }
    }

    private IEnumerator ClearBothAfterExchange()
    {
        yield return new WaitForSeconds(flightDuration);
        pairClearRoutine = null;
        ClearSide(CardOwner.Player);
        ClearSide(CardOwner.Reaper);
    }

    private IEnumerator FlightRoutine(CardView view, Vector2 startPos, Vector2 endPos, float startRot, float endRot)
    {
        float t = 0f;
        while (t < flightDuration)
        {
            if (view == null) yield break;
            t += Time.deltaTime;
            float eased = 1f - Mathf.Pow(1f - Mathf.Clamp01(t / flightDuration), 3f);
            view.SetLayout(Vector2.Lerp(startPos, endPos, eased), Mathf.Lerp(startRot, endRot, eased));
            yield return null;
        }
        if (view != null) view.SetLayout(endPos, endRot);
    }

    private void ClearSide(CardOwner owner)
    {
        if (owner == CardOwner.Player)
        {
            if (playerFlightRoutine != null) { StopCoroutine(playerFlightRoutine); playerFlightRoutine = null; }
            if (playerTableCard != null) { Destroy(playerTableCard.gameObject); playerTableCard = null; }
        }
        else
        {
            if (reaperFlightRoutine != null) { StopCoroutine(reaperFlightRoutine); reaperFlightRoutine = null; }
            if (reaperTableCard != null) { Destroy(reaperTableCard.gameObject); reaperTableCard = null; }
        }
    }

    private static Vector2 WorldPointToLocalAnchoredPosition(Vector3 worldPos, RectTransform targetParent)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(targetParent, screenPoint, null, out Vector2 localPoint);
        return localPoint;
    }
}
