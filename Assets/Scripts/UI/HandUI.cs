using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandUI : MonoBehaviour
{
    public CardOwner owner;
    [SerializeField] private bool isInteractable;
    [SerializeField] private CardView cardViewPrefab;
    [SerializeField] private RectTransform fanContainer;
    [SerializeField] private Button playButton;
    [SerializeField] private TextMeshProUGUI promptText;

    [Header("Fan Layout")]
    [SerializeField] private float fanArcDegrees = 40f;
    [SerializeField] private float cardSpacing = 90f;
    [SerializeField] private float fanRise = 40f;

    private RoundManager roundManager;
    private Hand handModel;
    private readonly List<CardView> activeViews = new List<CardView>();

    private CardView selectedView;
    private bool awaitingSwapTarget;
    private bool turnActive;

    private void Awake()
    {
        roundManager = FindAnyObjectByType<RoundManager>();
        handModel = owner == CardOwner.Player ? roundManager.PlayerHand : roundManager.ReaperHand;

        handModel.OnHandChanged += Redraw;
        roundManager.Total.OnRunningTotalChanged += OnRunningTotalChanged;
        roundManager.OnTurnStarted += HandleTurnStarted;
        roundManager.OnPlayerInputChanged += HandlePlayerInputChanged;

        if (playButton != null) playButton.onClick.AddListener(ConfirmPlaySelected);
        if (promptText != null) promptText.text = string.Empty;

        Redraw();
    }

    private void OnDestroy()
    {
        if (handModel != null) handModel.OnHandChanged -= Redraw;
        if (roundManager != null)
        {
            roundManager.Total.OnRunningTotalChanged -= OnRunningTotalChanged;
            roundManager.OnTurnStarted -= HandleTurnStarted;
            roundManager.OnPlayerInputChanged -= HandlePlayerInputChanged;
        }
    }

    private void OnRunningTotalChanged(int _) => RefreshPlayability();

    private void HandleTurnStarted(CardOwner turnOwner)
    {
        turnActive = turnOwner == owner;
        if (!turnActive) ClearSelection();
        RefreshInteractable();
    }

    private void HandlePlayerInputChanged(bool enabled)
    {
        if (owner != CardOwner.Player) return;
        turnActive = enabled;
        if (!enabled) ClearSelection();
        RefreshInteractable();
    }

    private void Redraw()
    {
        foreach (CardView view in activeViews)
        {
            view.OnClicked -= HandleCardClicked;
            Destroy(view.gameObject);
        }
        activeViews.Clear();
        selectedView = null;
        awaitingSwapTarget = false;
        if (promptText != null) promptText.text = string.Empty;

        List<Card_DataSO> cards = handModel.Cards;
        int count = cards.Count;

        for (int i = 0; i < count; i++)
        {
            CardView view = Instantiate(cardViewPrefab, fanContainer);
            view.SetCardData(cards[i], owner == CardOwner.Player);
            view.SetLayout(ComputeFanPosition(i, count), ComputeFanRotation(i, count));
            view.OnClicked += HandleCardClicked;
            activeViews.Add(view);
        }

        RefreshPlayability();
        RefreshInteractable();
    }

    private Vector2 ComputeFanPosition(int index, int count)
    {
        float centered = index - (count - 1) / 2f;
        float t = count <= 1 ? 0f : centered / ((count - 1) / 2f);
        float x = centered * cardSpacing;
        float y = -Mathf.Abs(t) * fanRise;
        return new Vector2(x, y);
    }

    private float ComputeFanRotation(int index, int count)
    {
        if (count <= 1) return 0f;
        float centered = index - (count - 1) / 2f;
        float t = centered / ((count - 1) / 2f);
        return -t * fanArcDegrees / 2f;
    }

    private void RefreshPlayability()
    {
        List<Card_DataSO> cards = handModel.Cards;
        for (int i = 0; i < activeViews.Count; i++)
        {
            bool playable = CardRules.IsGenerallyPlayable(cards[i], cards, roundManager, owner);
            activeViews[i].SetPlayable(playable);
        }
    }

    private void RefreshInteractable()
    {
        bool canInteract = isInteractable && turnActive;
        foreach (CardView view in activeViews) view.SetInteractable(canInteract);
        if (playButton != null) playButton.interactable = canInteract && selectedView != null && !awaitingSwapTarget;
    }

    private void HandleCardClicked(CardView view)
    {
        if (!isInteractable || !turnActive) return;

        if (awaitingSwapTarget)
        {
            if (view == selectedView) return;
            CommitPlay(selectedView.CardData, view.CardData);
            return;
        }

        if (view == selectedView)
        {
            ConfirmPlaySelected();
            return;
        }

        List<Card_DataSO> cards = handModel.Cards;
        if (!CardRules.IsGenerallyPlayable(view.CardData, cards, roundManager, owner)) return;

        SetSelected(view);
    }

    private void SetSelected(CardView view)
    {
        if (selectedView != null) selectedView.SetSelected(false);
        selectedView = view;
        selectedView.SetSelected(true);
        RefreshInteractable();
    }

    private void ClearSelection()
    {
        if (selectedView != null) selectedView.SetSelected(false);
        selectedView = null;
        awaitingSwapTarget = false;
        if (promptText != null) promptText.text = string.Empty;
    }

    private void ConfirmPlaySelected()
    {
        if (selectedView == null) return;

        if (selectedView.CardData.cardEffect is CardEffect_Swap)
        {
            awaitingSwapTarget = true;
            if (promptText != null) promptText.text = "Choose a card to give away";
            return;
        }

        CommitPlay(selectedView.CardData, null);
    }

    private void CommitPlay(Card_DataSO card, Card_DataSO selectedCard)
    {
        roundManager.TryPlayCard(owner, card, selectedCard);
        ClearSelection();
    }
}
