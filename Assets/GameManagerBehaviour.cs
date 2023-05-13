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

        game.ChooseCard(Actor.Player, card.data);
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
                foreach (var card in game.Player.CardsInHand)
                {
                    CreatePlayerCardObject(card);
                }
                foreach (var card in game.Opponent.CardsInHand)
                {
                    CreateOpponentCardObject(card);
                }

                Invoke(nameof(MoveToNextPhase), 1.0f);
                break;

            case GamePhase.PlayerChoose:
                // allow player to choose a card from their hand
                break;

            case GamePhase.PlayerExecute:
                ExecuteCard(game.Player.SelectedCard, true);
                Invoke(nameof(MoveToNextPhase), 3.0f);
                break;

            case GamePhase.PlayerDecay:
                {
                    executingPlayerText.gameObject.SetActive(false);
                    executingCard.gameObject.SetActive(false);

                    var expiredCards = game.DecayCards(Actor.Player);
                    foreach (var card in expiredCards)
                    {
                        Destroy(cardObjByData[card].gameObject);
                    }

                    // pick up new card to player hand
                    var cardData = game.PickUpCard(Actor.Player);
                    CreatePlayerCardObject(cardData);

                    Invoke(nameof(MoveToNextPhase), 1.0f);
                }
                break;

            case GamePhase.AiChoose:
                Invoke(nameof(MoveToNextPhase), 3.0f);
                break;

            case GamePhase.AiExecute:
                var selectedCard = game.Opponent.CardsInHand[0];
                game.ChooseCard(Actor.Opponent, selectedCard);
                Destroy(cardObjByData[selectedCard].gameObject);

                ExecuteCard(game.Opponent.SelectedCard, false);

                Invoke(nameof(MoveToNextPhase), 3.0f);
                break;

            case GamePhase.AiDecay:
                {
                    executingPlayerText.gameObject.SetActive(false);
                    executingCard.gameObject.SetActive(false);

                    var expiredCards = game.DecayCards(Actor.Opponent);
                    foreach (var card in expiredCards)
                    {
                        Destroy(cardObjByData[card].gameObject);
                    }

                    // pick up new card to ai hand
                    var cardData = game.PickUpCard(Actor.Opponent);
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
            if (game.Opponent.CardsInHand.Count == 0) { GameOver(true); }
            else if (game.Player.CardsInHand.Count == 0) { GameOver(false); }
        }
    }
}
