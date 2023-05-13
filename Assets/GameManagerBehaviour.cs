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
    public int initialHandSize = 5;
    public GameObject cardPrefab;
    public Transform rootCanvas;
    public Transform playerHandPanel;
    public Transform opponentHandPanel;
    public Text currentPhaseText;
    public Text executingPlayerText;
    public Text winConditionText;
    public CardBehaviour executingCard;

    private CardGame game;

    private GamePhase currentPhase;
    private CardData playerChosenCard = null;
    private CardData opponentChosenCard = null;
    private Dictionary<CardData, CardBehaviour> cardObjByData = new Dictionary<CardData, CardBehaviour>();

    // Start is called before the first frame update
    void Start()
    {
        game = new CardGame();

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
        if (currentPhase != GamePhase.PlayerChoose)
        {
            // ignore click if we're not in the choose phase
            return;
        }

        Debug.Log("Was Clicked" + card.data.Health);
        playerChosenCard = card.data;
        game.PlayerHand.Remove(card.data);
        Destroy(card.gameObject);
        MoveToNextPhase();
    }

    void BeginPhase(GamePhase phase)
    {
        currentPhaseText.text = $"Current phase: {phase}";
        switch (phase)
        {
            case GamePhase.Dealing:
                cardObjByData.Clear();

                winConditionText.gameObject.SetActive(false);
                executingPlayerText.gameObject.SetActive(false);
                executingCard.gameObject.SetActive(false);

                game.DealCards(initialHandSize);
                foreach (var card in game.PlayerHand)
                {
                    CreatePlayerCardObject(card);
                }
                foreach (var card in game.OpponentHand)
                {
                    CreateOpponentCardObject(card);
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
                    foreach (var card in game.PlayerHand)
                    {
                        card.Health--;
                        if (card.Health == 0)
                        {
                            toRemove.Add(card);
                        }
                    }
                    foreach (var card in toRemove)
                    {
                        game.PlayerHand.Remove(card);
                        Destroy(cardObjByData[card].gameObject);
                    }

                    // pick up new card to player hand
                    var cardData = game.PickUpCard(Hand.Player);
                    CreatePlayerCardObject(cardData);

                    Invoke(nameof(MoveToNextPhase), 1.0f);
                }
                break;

            case GamePhase.AiChoose:
                Invoke(nameof(MoveToNextPhase), 3.0f);
                break;

            case GamePhase.AiExecute:
                var chosenCard = game.OpponentHand[0];
                game.OpponentHand.RemoveAt(0);
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
                    foreach (var card in game.OpponentHand)
                    {
                        card.Health--;
                        if (card.Health == 0)
                        {
                            toRemove.Add(card);
                        }
                    }
                    foreach (var card in toRemove)
                    {
                        game.OpponentHand.Remove(card);
                        Destroy(cardObjByData[card].gameObject);
                    }

                    // pick up new card to ai hand
                    var cardData = game.PickUpCard(Hand.Opponent);
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
        if (currentPhase != GamePhase.Dealing)
        {
            if (game.PlayerHand.Count == 0) { GameOver(true); }
            else if (game.OpponentHand.Count == 0) { GameOver(false); }
        }
    }
}
