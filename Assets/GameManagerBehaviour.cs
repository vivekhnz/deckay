using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class GameManagerBehaviour : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform rootCanvas;
    public Transform playerHandPanel;
    public Transform opponentHandPanel;
    public Text currentPhaseText;
    public Text executingNameText;
    public Text executingDescriptionText;
    public Text winConditionText;
    public Image executeBackground;
    public CardBehaviour executingCard;

    private CardGame game;

    private Dictionary<CardData, CardBehaviour> cardObjByData = new Dictionary<CardData, CardBehaviour>();

    // Start is called before the first frame update
    void Start()
    {
        game = new CardGame();
        MoveToNextPhase();
    }

    private void MoveToNextPhase()
    {
        UpdateGameScene(game => game.MoveToNextPhase());
    }

    private void UpdateGameScene(Action<CardGame> updateSim)
    {
        // shallow-copy cards before game logic executes
        // we'll diff afterwards and update the UI accordingly
        var playerCardsBefore = new List<CardData>(game.Player.CardsInHand);
        var opponentCardsBefore = new List<CardData>(game.Opponent.CardsInHand);

        // update game simulation
        updateSim(game);

        // diff cards to determine what UI updates to make
        var playerCardsToAdd = game.Player.CardsInHand.Where(card => !playerCardsBefore.Contains(card)).ToList();
        var opponentCardsToAdd = game.Opponent.CardsInHand.Where(card => !opponentCardsBefore.Contains(card)).ToList();
        var cardsToDelete = new List<CardData>();
        cardsToDelete.AddRange(playerCardsBefore.Where(card => !game.Player.CardsInHand.Contains(card)));
        cardsToDelete.AddRange(opponentCardsBefore.Where(card => !game.Opponent.CardsInHand.Contains(card)));

        // update UI
        foreach (var card in cardsToDelete)
        {
            Destroy(cardObjByData[card].gameObject);
            cardObjByData.Remove(card);
        }
        foreach (var card in playerCardsToAdd)
        {
            CreatePlayerCardObject(card);
        }
        foreach (var card in opponentCardsToAdd)
        {
            CreateOpponentCardObject(card);
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
        FindObjectOfType<AudioManager>().PlaySound("GameOver");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ChooseCardFromPlayerHand(CardBehaviour card)
    {
        Debug.Assert(game.CurrentPhase == GamePhase.PlayerChoose);

        UpdateGameScene(game =>
        {
            game.ChooseCard(Actor.Player, card.data);
            game.MoveToNextPhase();
        });
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
                executingNameText.text = executingCard.data.DisplayName;
                executingDescriptionText.text = executingCard.data.Description;
                executingCard.data = game.Player.SelectedCard;
            }
            else
            {
                executingNameText.text = executingCard.data.DisplayName;
                executingDescriptionText.text = executingCard.data.Description;
                executingCard.data = game.Opponent.SelectedCard;
            }

            executingNameText.gameObject.SetActive(true);
            executingDescriptionText.gameObject.SetActive(true);
            executeBackground.gameObject.SetActive(true);
            executingCard.gameObject.SetActive(true);
        }
        else
        {
            executingNameText.gameObject.SetActive(false);
            executingDescriptionText.gameObject.SetActive(false);
            executeBackground.gameObject.SetActive(false);
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

            if (win)
                FindObjectOfType<AudioManager>().PlaySound("YouWin");
            else
                FindObjectOfType<AudioManager>().PlaySound("YouLose");

            // todo: don't fire this every frame
            Invoke(nameof(RestartGame), 3.0f);
        }
        else
        {
            winConditionText.gameObject.SetActive(false);
        }
    }
}
