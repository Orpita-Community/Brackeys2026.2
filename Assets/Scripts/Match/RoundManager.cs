using System;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour, ICardEffectContext
{
    [Header("Deck Source")]
    [SerializeField] private CardList_DataSO fullDeckData;

    [Header("Round Setup")]
    [SerializeField] private int startingHandSize = 8;

    [Header("Reaper Timing")]
    [SerializeField] private float aiThinkDelaySeconds = 0.75f;

    [Header("Reaper AI")]
    [SerializeField] private ReaperAI reaperAI = new ReaperAI();

    public Hand PlayerHand { get; } = new Hand(CardOwner.Player);
    public Hand ReaperHand { get; } = new Hand(CardOwner.Reaper);
    public Hearts PlayerHearts { get; } = new Hearts(CardOwner.Player, 9);
    public Hearts ReaperHearts { get; } = new Hearts(CardOwner.Reaper, 9);
    public RunningTotalTracker Total { get; } = new RunningTotalTracker();

    public RoundStartState RoundStartState { get; private set; }
    public PlayerTurnState PlayerTurnState { get; private set; }
    public ReaperTurnState ReaperTurnState { get; private set; }
    public RoundEndState RoundEndState { get; private set; }
    public MatchEndState MatchEndState { get; private set; }

    public event Action<CardOwner> OnTurnStarted;
    public event Action<CardOwner> OnRoundStarted;
    public event Action<CardOwner> OnRoundEnded;
    public event Action<CardOwner> OnMatchEnded;
    public event Action<bool> OnPlayerInputChanged;

    public float AiThinkDelaySeconds => aiThinkDelaySeconds;
    public ReaperAI ReaperAI => reaperAI;
    public bool IsMatchEnded { get; private set; }

    private StateMachine stateMachine;
    private readonly Deck deck = new Deck();
    private readonly bool[] skipNextTurn = new bool[2];
    private readonly bool[] forceRandomNextMove = new bool[2];
    private CardOwner nextRoundStarter;

    private void Awake()
    {
        stateMachine = new StateMachine();
        RoundStartState = new RoundStartState(this, stateMachine);
        PlayerTurnState = new PlayerTurnState(this, stateMachine);
        ReaperTurnState = new ReaperTurnState(this, stateMachine);
        RoundEndState = new RoundEndState(this, stateMachine);
        MatchEndState = new MatchEndState(this, stateMachine);

        nextRoundStarter = UnityEngine.Random.value < 0.5f ? CardOwner.Player : CardOwner.Reaper;
    }

    private void Start()
    {
        stateMachine.Initialize(RoundStartState);
    }

    // ---------- ICardEffectContext ----------

    public int RunningTotal => Total.Value;
    public int RunningTotalLimit => Total.Limit;
    public void AddToRunningTotal(int amount) => Total.Add(amount);
    public void ResetRunningTotal() => Total.Reset();

    public void LoseHeart(CardOwner owner)
    {
        if (IsMatchEnded) return;

        Hearts hearts = GetHearts(owner);
        hearts.LoseHeart();

        if (hearts.IsDepleted)
        {
            IsMatchEnded = true;
            OnMatchEnded?.Invoke(owner);
            stateMachine.ChangeState(MatchEndState);
        }
    }

    public CardOwner GetOpponent(CardOwner owner) => owner == CardOwner.Player ? CardOwner.Reaper : CardOwner.Player;

    public IReadOnlyList<Card_DataSO> GetHand(CardOwner owner) => GetHandModel(owner).Cards;

    public void MoveCard(Card_DataSO card, CardOwner from, CardOwner to)
    {
        if (card == null) return;
        GetHandModel(from).RemoveCard(card);
        GetHandModel(to).AddCard(card);
    }

    public Card_DataSO GetRandomCard(CardOwner owner)
    {
        List<Card_DataSO> cards = GetHandModel(owner).Cards;
        return cards.Count == 0 ? null : cards[UnityEngine.Random.Range(0, cards.Count)];
    }

    public void SkipNextTurn(CardOwner owner) => skipNextTurn[(int)owner] = true;
    public void ForceRandomNextMove(CardOwner owner) => forceRandomNextMove[(int)owner] = true;
    public int RollNumberCardValue() => Deck.RollWeightedNumberValue(fullDeckData);

    // ---------- Orchestration ----------

    private Hand GetHandModel(CardOwner owner) => owner == CardOwner.Player ? PlayerHand : ReaperHand;
    private Hearts GetHearts(CardOwner owner) => owner == CardOwner.Player ? PlayerHearts : ReaperHearts;

    public void BuildAndDealHands()
    {
        deck.Build(fullDeckData);
        deck.Shuffle();
        PlayerHand.SetHand(deck.Draw(startingHandSize));
        ReaperHand.SetHand(deck.Draw(startingHandSize));
    }

    public CardOwner ConsumeRoundStarter()
    {
        CardOwner starter = nextRoundStarter;
        nextRoundStarter = GetOpponent(nextRoundStarter);
        return starter;
    }

    public void RaiseRoundStarted(CardOwner starter) => OnRoundStarted?.Invoke(starter);

    public bool PrepareTurn(CardOwner owner)
    {
        if (IsMatchEnded) return false;

        if (skipNextTurn[(int)owner])
        {
            skipNextTurn[(int)owner] = false;
            AdvanceTurn(owner);
            return false;
        }

        if (!CardRules.HasAnyLegalPlay(GetHandModel(owner).Cards, this, owner))
        {
            LoseHeart(owner);
            if (IsMatchEnded) return false;
            ResetRunningTotal();
            AdvanceTurn(owner);
            return false;
        }

        OnTurnStarted?.Invoke(owner);
        return true;
    }

    public bool ConsumeForceRandom(CardOwner owner)
    {
        if (!forceRandomNextMove[(int)owner]) return false;
        forceRandomNextMove[(int)owner] = false;
        return true;
    }

    public void PlayRandomLegalCard(CardOwner owner)
    {
        List<Card_DataSO> hand = GetHandModel(owner).Cards;
        List<Card_DataSO> legalCards = new List<Card_DataSO>();
        foreach (Card_DataSO card in hand)
        {
            if (CardRules.IsGenerallyPlayable(card, hand, this, owner))
                legalCards.Add(card);
        }

        if (legalCards.Count == 0) return;

        Card_DataSO chosen = legalCards[UnityEngine.Random.Range(0, legalCards.Count)];
        Card_DataSO selectedCard = null;
        if (chosen.cardEffect is CardEffect_Swap)
        {
            foreach (Card_DataSO card in hand)
            {
                if (card == chosen) continue;
                selectedCard = card;
                break;
            }
        }

        TryPlayCard(owner, chosen, selectedCard);
    }

    public void TryPlayCard(CardOwner owner, Card_DataSO card, Card_DataSO selectedCard)
    {
        if (IsMatchEnded || card == null) return;

        Hand hand = GetHandModel(owner);
        if (!hand.Contains(card)) return;
        if (!CardRules.IsPlayable(card, selectedCard, this, owner)) return;

        hand.RemoveCard(card);

        if (card.cardType == CardType.Number)
        {
            AddToRunningTotal(card.numberValue);
        }
        else if (card.cardEffect != null)
        {
            card.cardEffect.ExecuteEffect(this, new CardEffectArgs { owner = owner, playedCard = card, selectedCard = selectedCard });
        }

        if (IsMatchEnded) return;

        if (hand.IsEmpty)
        {
            CardOwner opponent = GetOpponent(owner);
            LoseHeart(opponent);
            if (IsMatchEnded) return;
            OnRoundEnded?.Invoke(owner);
            stateMachine.ChangeState(RoundEndState);
            return;
        }

        AdvanceTurn(owner);
    }

    private void AdvanceTurn(CardOwner actingOwner) => GoToTurnState(GetOpponent(actingOwner));

    public void GoToTurnState(CardOwner owner)
    {
        if (IsMatchEnded) return;
        stateMachine.ChangeState(owner == CardOwner.Player ? (MatchState)PlayerTurnState : ReaperTurnState);
    }

    public void GoToRoundStart()
    {
        if (IsMatchEnded) return;
        stateMachine.ChangeState(RoundStartState);
    }

    public void SetPlayerInputEnabled(bool enabled) => OnPlayerInputChanged?.Invoke(enabled);
}
