using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

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

    private CardGame game;

    private Dictionary<CardData, CardBehaviour> cardObjByData = new Dictionary<CardData, CardBehaviour>();

    // Start is called before the first frame update
    void Start()
    {
        game = new CardGame();

        MoveToNextPhase();
    }

    public void MoveToNextPhase()
    {
        // shallow-copy cards before game logic executes
        // we'll diff afterwards and update the UI accordingly
        var playerCardsBefore = new List<CardData>(game.Player.CardsInHand);
        var opponentCardsBefore = new List<CardData>(game.Opponent.CardsInHand);

        // update game simulation
        game.MoveToNextPhase();

        // diff cards to determine what UI updates to make
        var playerCardsToAdd = game.Player.CardsInHand.Where(card => !playerCardsBefore.Contains(card));
        var opponentCardsToAdd = game.Opponent.CardsInHand.Where(card => !opponentCardsBefore.Contains(card));
        var cardsToDelete = new List<CardData>();
        cardsToDelete.AddRange(playerCardsBefore.Where(card => !game.Player.CardsInHand.Contains(card)));
        cardsToDelete.AddRange(opponentCardsBefore.Where(card => !game.Opponent.CardsInHand.Contains(card)));

        // update UI
        foreach (var card in playerCardsToAdd)
        {
            CreatePlayerCardObject(card);
        }
        foreach (var card in opponentCardsToAdd)
        {
            CreateOpponentCardObject(card);
        }
        foreach (var card in cardsToDelete)
        {
            Destroy(cardObjByData[card].gameObject);
            cardObjByData.Remove(card);
        }

        currentPhaseText.text = $"Current phase: {game.CurrentPhase}";
        switch (game.CurrentPhase)
        {
            case GamePhase.Dealing:
                // todo: start deal animation
                Invoke(nameof(MoveToNextPhase), 1.0f);
                break;

            case GamePhase.PlayerChoose:
                // allow player to choose a card from their hand
                break;

            case GamePhase.PlayerExecute:
                // todo: start execute animation
                Invoke(nameof(MoveToNextPhase), 3.0f);
                break;

            case GamePhase.PlayerDecay:
                // todo: start decay animation
                Invoke(nameof(MoveToNextPhase), 1.0f);
                break;

            case GamePhase.PlayerPickUp:
                // todo: start pickup animation
                Invoke(nameof(MoveToNextPhase), 1.0f);
                break;

            case GamePhase.AiChoose:
                Invoke(nameof(MoveToNextPhase), 2.0f);
                break;

            case GamePhase.AiExecute:
                // todo: start execute animation
                Invoke(nameof(MoveToNextPhase), 3.0f);
                break;

            case GamePhase.AiDecay:
                // todo: start decay animation
                Invoke(nameof(MoveToNextPhase), 1.0f);
                break;

            case GamePhase.AiPickUp:
                // todo: start pickup animation
                Invoke(nameof(MoveToNextPhase), 1.0f);
                break;
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ChooseCardFromPlayerHand(CardBehaviour card)
    {
        Debug.Assert(game.CurrentPhase == GamePhase.PlayerChoose);

        game.ChooseCard(Actor.Player, card.data);
        Destroy(card.gameObject);
        MoveToNextPhase();
    }

    private void CreatePlayerCardObject(CardData cardData)
    {
        var cardObj = Instantiate(cardPrefab, playerHandPanel, false);
        var card = cardObj.GetComponent<CardBehaviour>();
        card.data = cardData;
        cardObjByData[cardData] = card;

        card.onClickAction.AddListener(() =>
        {
            if (game.CurrentPhase == GamePhase.PlayerChoose)
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
        GamePhase phase = game.CurrentPhase;
        if (phase == GamePhase.PlayerExecute || phase == GamePhase.AiExecute)
        {
            if (phase == GamePhase.PlayerExecute)
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
        if (phase != GamePhase.Dealing)
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
