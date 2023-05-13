using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public GameObject cardPrefab;
    public Transform rootCanvas;
    public Transform playerHandPanel;
    public Transform opponentHandPanel;
    public Text currentPhaseText;
    public Text executingPlayerText;
    public Text winConditionText;
    public CardBehaviour executingCard;

    private int handSize = 5;
    private int handOffset = 50;
    private Deck deck;
    private GamePhase currentPhase;
    private List<CardBehaviour> playerHand = new List<CardBehaviour>();
    private List<CardBehaviour> opponentHand = new List<CardBehaviour>();
    private CardData playerChosenCard = null;
    private CardData opponentChosenCard = null;

    // Start is called before the first frame update
    void Start()
    {
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

    public void GameOver(bool win)
    {
        winConditionText.text = $"You {(win ? "Win" : "Lose")}";
        winConditionText.gameObject.SetActive(true);

        Invoke(nameof(RestartGame), 3.0f);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ChooseCardFromPlayerHand(CardBehaviour card)
    {
        Debug.Log("Was Clicked" + card.data.Health);
        playerChosenCard = card.data;
        playerHand.Remove(card);
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

                winConditionText.gameObject.SetActive(false);
                executingPlayerText.gameObject.SetActive(false);
                executingCard.gameObject.SetActive(false);

                playerHand.Clear();

                bool playerDeal = true;

                // replace for loop with card count possibly. 
                for (int i = 0; i < handSize; i++)
                {
                    CardBehaviour card;

                    if (i % 2 == 1)
                    {
                        card = DealCard(deck, new Vector2(((i * handOffset) + handOffset), 0), playerDeal);
                    }
                    else
                    {
                        card = DealCard(deck, new Vector2(i * -handOffset, 0), playerDeal);
                    }
                    card.flipped = true;
                    playerHand.Add(card);
                }

                playerDeal = false;

                for (int i = 0; i < handSize; i++)
                {
                    CardBehaviour card;

                    if (i % 2 == 1)
                    {
                        card = DealCard(deck, new Vector2(((i * handOffset) + handOffset), 0), playerDeal);
                    }
                    else
                    {
                        card = DealCard(deck, new Vector2(i * -handOffset, 0), playerDeal);
                    }
                    card.flipped = false;
                    opponentHand.Add(card);
                }

                Invoke(nameof(MoveToNextPhase), 1.0f);

                break;

            case GamePhase.PlayerChoose:
                // allow player to choose a card from their hand
                foreach (var card in playerHand)
                {
                    card.isClickable = true;
                }
                break;

            case GamePhase.PlayerExecute:
                ExecuteCard(playerChosenCard, true);
                foreach (var card in playerHand)
                {
                    card.isClickable = false;
                }

                Invoke(nameof(MoveToNextPhase), 3.0f);
                break;

            case GamePhase.PlayerDecay:
                {
                    executingPlayerText.gameObject.SetActive(false);
                    executingCard.gameObject.SetActive(false);

                    var toRemove = new List<CardBehaviour>();
                    foreach (var card in playerHand)
                    {
                        card.data.Health--;
                        if (card.data.Health == 0)
                        {
                            toRemove.Add(card);
                        }
                    }
                    foreach (var card in toRemove)
                    {
                        playerHand.Remove(card);
                        Destroy(card.gameObject);
                        //adjustHand(playerHand, true);
                    }

                    // pick up new card to player hand
                    int currentHandSize = playerHand.Count;
                    CardBehaviour cardDraw;
                    if (currentHandSize % 2 == 1)
                    {
                        cardDraw = DealCard(deck, new Vector2(((currentHandSize * handOffset) + handOffset), 0), true);
                    }
                    else
                    {
                        cardDraw = DealCard(deck, new Vector2(currentHandSize * -handOffset, 0), true);
                    }

                    cardDraw.flipped = true;
                    playerHand.Add(cardDraw);
                    Invoke(nameof(MoveToNextPhase), 1.0f);
                }
                break;

            case GamePhase.AiChoose:
                Invoke(nameof(MoveToNextPhase), 3.0f);
                break;

            case GamePhase.AiExecute:
                var chosenCard = opponentHand[0];
                opponentHand.RemoveAt(0);
                opponentChosenCard = chosenCard.data;
                Destroy(chosenCard.gameObject);

                ExecuteCard(opponentChosenCard, false);

                Invoke(nameof(MoveToNextPhase), 3.0f);
                break;

            case GamePhase.AiDecay:
                {
                    executingPlayerText.gameObject.SetActive(false);
                    executingCard.gameObject.SetActive(false);

                    var toRemove = new List<CardBehaviour>();
                    foreach (var card in opponentHand)
                    {
                        card.data.Health--;
                        if (card.data.Health == 0)
                        {
                            toRemove.Add(card);
                        }
                    }
                    foreach (var card in toRemove)
                    {
                        opponentHand.Remove(card);
                        Destroy(card.gameObject);
                        //adjustHand(opponentHand, false);
                    }

                    // pick up new card to ai hand
                    int opponentHandSize = opponentHand.Count;
                    CardBehaviour cardDraw;
                    if (opponentHandSize % 2 == 1)
                    {
                        cardDraw = DealCard(deck, new Vector2(((opponentHandSize * handOffset) + handOffset), 0), false);
                    }
                    else
                    {
                        cardDraw = DealCard(deck, new Vector2(opponentHandSize * -handOffset, 0), false);
                    }

                    cardDraw.flipped = false;
                    opponentHand.Add(cardDraw);
                    Invoke(nameof(MoveToNextPhase), 1.0f);
                }
                break;
        }
    }

    private CardBehaviour DealCard(Deck deck, Vector2 position, bool playerDeal)
    {
        var cardObj = Instantiate(cardPrefab);

        if (playerDeal)
        {
            cardObj.transform.SetParent(playerHandPanel, false);
        }
        else
        {
            cardObj.transform.SetParent(opponentHandPanel, false);
        }

        var transform = cardObj.GetComponent<RectTransform>();
        transform.anchoredPosition = position;

        // if opponent display back not front of card
        var card = cardObj.GetComponent<CardBehaviour>();
        card.data = deck.GetNextCard();

        card.onClickAction.AddListener(() =>
        {
            if (currentPhase == GamePhase.PlayerChoose)
            {
                ChooseCardFromPlayerHand(card);
            }
        });

        return card;
    }

    private void adjustHand(List<CardBehaviour> hand, bool playerTurn)
    {
        for (int i = 0; i < hand.Count; i++)
        {
            var card = hand[i];
            var transform = card.gameObject.GetComponent<RectTransform>();

            if (i % 2 == 1)
            {
                transform.anchoredPosition = new Vector2(((i * handOffset) + handOffset), 0);
            }
            else
            {
                transform.anchoredPosition = new Vector2(i * -handOffset, 0);
            }
        }
    }

    private void ExecuteCard(CardData card, bool isPlayerCard)
    {
        // todo: execute card's effect
        executingPlayerText.text = $"{(isPlayerCard ? "You" : "Opponent")} played";
        executingCard.flipped = true;
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
        if (currentPhase != GamePhase.Dealing)
        {
            if (playerHand.Count == 0) { GameOver(true); }
            else if (opponentHand.Count == 0) { GameOver(false); }
        }
    }
}
