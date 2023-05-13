using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum GamePhase
{
    Dealing = 0,
    PlayerChoose,
    PlayerExecute,
    PlayerDecay,
    AiChoose,
    AiExecute,
    AiDecay
}

public class GameManagerBehaviour : MonoBehaviour
{
    public int initialHandSize = 5;
    public GameObject cardPrefab;
    public Transform rootCanvas;
    public Transform playerHandPanel;
    public Transform opponentHandPanel;
    public Text currentPhaseText;
    public Text executingPlayerText;
    public CardBehaviour executingCard;

    private Deck deck;
    private GamePhase currentPhase;
    private List<CardData> playerHand = new List<CardData>();
    private List<CardData> opponentHand = new List<CardData>();
    private CardData playerChosenCard = null;
    private CardData opponentChosenCard = null;
    private Dictionary<CardData, CardBehaviour> cardObjByData = new Dictionary<CardData, CardBehaviour>();

    // Start is called before the first frame update
    void Start()
    {
        executingPlayerText.gameObject.SetActive(false);
        executingCard.gameObject.SetActive(false);

        currentPhase = GamePhase.Dealing;
        BeginPhase(currentPhase);
    }

    public void MoveToNextPhase()
    {
        if (currentPhase == GamePhase.AiDecay)
        {
            currentPhase = GamePhase.PlayerChoose;
        }
        else
        {
            currentPhase++;
        }

        BeginPhase(currentPhase);
    }

    public void ChooseCardFromPlayerHand(CardBehaviour card)
    {
        if (currentPhase != GamePhase.PlayerChoose)
        {
            // ignore click if we're not in the choose phase
            return;
        }

        Debug.Log("Was Clicked" + card.data.Health);
        playerChosenCard = card.data;
        playerHand.Remove(card.data);
        Destroy(card.gameObject);
        MoveToNextPhase();
    }

    void BeginPhase(GamePhase phase)
    {
        currentPhaseText.text = $"Current phase: {phase}";
        switch (phase)
        {
            case GamePhase.Dealing:
                deck = new Deck();
                cardObjByData.Clear();

                playerHand.Clear();
                for (int i = 0; i < initialHandSize; i++)
                {
                    var cardData = deck.GetNextCard();
                    playerHand.Add(cardData);
                    CreatePlayerCardObject(cardData);
                }

                opponentHand.Clear();
                for (int i = 0; i < initialHandSize; i++)
                {
                    var cardData = deck.GetNextCard();
                    cardData.IsFaceDown = true;
                    opponentHand.Add(cardData);
                    CreateOpponentCardObject(cardData);
                }

                Invoke(nameof(MoveToNextPhase), 1.0f);
                break;

            case GamePhase.PlayerChoose:
                // allow player to choose a card from their hand
                break;

            case GamePhase.PlayerExecute:
                ExecuteCard(playerChosenCard, true);
                Invoke(nameof(MoveToNextPhase), 3.0f);
                break;

            case GamePhase.PlayerDecay:
                {
                    executingPlayerText.gameObject.SetActive(false);
                    executingCard.gameObject.SetActive(false);

                    var toRemove = new List<CardData>();
                    foreach (var card in playerHand)
                    {
                        card.Health--;
                        if (card.Health == 0)
                        {
                            toRemove.Add(card);
                        }
                    }
                    foreach (var card in toRemove)
                    {
                        playerHand.Remove(card);
                        Destroy(cardObjByData[card].gameObject);
                    }

                    // pick up new card to player hand
                    var cardData = deck.GetNextCard();
                    playerHand.Add(cardData);
                    CreatePlayerCardObject(cardData);

                    Invoke(nameof(MoveToNextPhase), 1.0f);
                }
                break;

            case GamePhase.AiChoose:
                Invoke(nameof(MoveToNextPhase), 3.0f);
                break;

            case GamePhase.AiExecute:
                var chosenCard = opponentHand[0];
                opponentHand.RemoveAt(0);
                opponentChosenCard = chosenCard;
                Destroy(cardObjByData[chosenCard].gameObject);

                ExecuteCard(opponentChosenCard, false);

                Invoke(nameof(MoveToNextPhase), 3.0f);
                break;

            case GamePhase.AiDecay:
                {
                    executingPlayerText.gameObject.SetActive(false);
                    executingCard.gameObject.SetActive(false);

                    var toRemove = new List<CardData>();
                    foreach (var card in opponentHand)
                    {
                        card.Health--;
                        if (card.Health == 0)
                        {
                            toRemove.Add(card);
                        }
                    }
                    foreach (var card in toRemove)
                    {
                        opponentHand.Remove(card);
                        Destroy(cardObjByData[card].gameObject);
                    }

                    // pick up new card to ai hand
                    var cardData = deck.GetNextCard();
                    cardData.IsFaceDown = true;
                    opponentHand.Add(cardData);
                    CreateOpponentCardObject(cardData);

                    Invoke(nameof(MoveToNextPhase), 1.0f);
                }
                break;
        }
    }

    private void CreatePlayerCardObject(CardData cardData)
    {
        var cardObj = Instantiate(cardPrefab, playerHandPanel, false);
        var card = cardObj.GetComponent<CardBehaviour>();
        card.data = cardData;
        cardObjByData[cardData] = card;

        card.onClickAction.AddListener(() =>
        {
            if (currentPhase == GamePhase.PlayerChoose)
            {
                ChooseCardFromPlayerHand(card);
            }
        });
    }

    private void CreateOpponentCardObject(CardData cardData)
    {
        var cardObj = Instantiate(cardPrefab, opponentHandPanel, false);
        var card = cardObj.GetComponent<CardBehaviour>();
        card.data = cardData;
        cardObjByData[cardData] = card;
    }

    private void ExecuteCard(CardData card, bool isPlayerCard)
    {
        card.IsFaceDown = false;

        // todo: execute card's effect
        executingPlayerText.text = $"{(isPlayerCard ? "You" : "Opponent")} played";
        executingCard.data = card;
        executingPlayerText.gameObject.SetActive(true);
        executingCard.gameObject.SetActive(true);

        // todo: run animation for the player's chosen card

        DiscardCard(card);
    }

    private void DiscardCard(CardData card)
    {
        // todo: discard card
    }

    // Update is called once per frame
    void Update()
    {

    }
}
