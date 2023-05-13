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

                game.DealCards(initialHandSize);
                foreach (var card in game.Player.CardsInHand)
                {
                    CreatePlayerCardObject(card);
                }
                foreach (var card in game.Opponent.CardsInHand)
                {
                    CreateOpponentCardObject(card);
                }

                // todo: start deal animation
                Invoke(nameof(MoveToNextPhase), 1.0f);
                break;

            case GamePhase.PlayerChoose:
                // allow player to choose a card from their hand
                break;

            case GamePhase.PlayerExecute:
                game.Execute(Actor.Player);

                // todo: start execute animation
                Invoke(nameof(MoveToNextPhase), 3.0f);
                break;

            case GamePhase.PlayerDecay:
                {
                    var expiredCards = game.DecayCards(Actor.Player);
                    foreach (var card in expiredCards)
                    {
                        Destroy(cardObjByData[card].gameObject);
                    }

                    // pick up new card to player hand
                    var cardData = game.PickUpCard(Actor.Player);
                    CreatePlayerCardObject(cardData);

                    // todo: start decay animation
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

                game.Execute(Actor.Opponent);

                // todo: start execute animation
                Invoke(nameof(MoveToNextPhase), 3.0f);
                break;

            case GamePhase.AiDecay:
                {
                    var expiredCards = game.DecayCards(Actor.Opponent);
                    foreach (var card in expiredCards)
                    {
                        Destroy(cardObjByData[card].gameObject);
                    }

                    // pick up new card to ai hand
                    var cardData = game.PickUpCard(Actor.Opponent);
                    CreateOpponentCardObject(cardData);

                    // todo: start decay animation
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

    // Update is called once per frame
    void Update()
    {
        if (currentPhase == GamePhase.PlayerExecute || currentPhase == GamePhase.AiExecute)
        {
            if (currentPhase == GamePhase.PlayerExecute)
            {
                executingPlayerText.text = "You played";
                executingCard.data = game.Player.SelectedCard;
            }
            else
            {
                executingPlayerText.text = "Opponent played";
                executingCard.data = game.Opponent.SelectedCard;
            }

            executingPlayerText.gameObject.SetActive(true);
            executingCard.gameObject.SetActive(true);
        }
        else
        {
            executingPlayerText.gameObject.SetActive(false);
            executingCard.gameObject.SetActive(false);
        }

        Actor? winner = null;
        if (currentPhase != GamePhase.Dealing)
        {
            if (game.Opponent.CardsInHand.Count == 0)
            {
                winner = Actor.Player;
            }
            else if (game.Player.CardsInHand.Count == 0)
            {
                winner = Actor.Opponent;
            }
        }
        if (winner.HasValue)
        {
            bool win = winner.Value == Actor.Player;
            winConditionText.text = $"You {(win ? "Win" : "Lose")}";
            winConditionText.gameObject.SetActive(true);

            // todo: don't fire this every frame
            Invoke(nameof(RestartGame), 3.0f);
        }
        else
        {
            winConditionText.gameObject.SetActive(false);
        }
    }
}
