using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GamePhase
{
    Dealing = 0,
    PlayerChoose,
    PlayerExecute,
    AiChoose,
    AiExecute,
    Decay
}

public class GameManagerBehaviour : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform rootCanvas;
    public Transform playerHandPanel;
    public Text currentPhaseText;

    private Deck deck;
    private GamePhase currentPhase;
    private List<CardBehaviour> playerHand = new List<CardBehaviour>();
    private List<CardBehaviour> opponentHand = new List<CardBehaviour>();

    // Start is called before the first frame update
    void Start()
    {
        currentPhase = GamePhase.Dealing;
        BeginPhase(currentPhase);
    }

    public void MoveToNextPhase()
    {
        if (currentPhase == GamePhase.Decay)
        {
            currentPhase = GamePhase.PlayerChoose;
        }
        else
        {
            currentPhase++;
        }

        BeginPhase(currentPhase);
    }

    void BeginPhase(GamePhase phase)
    {
        currentPhaseText.text = $"Current phase: {phase}";
        switch (phase)
        {
            case GamePhase.Dealing:
                deck = new Deck();

                playerHand.Clear();
                for (int i = 0; i < 5; i++)
                {
                    var card = DealCard(deck, new Vector2(i * 75, 0));
                    playerHand.Add(card);
                }

                // todo: deal opponent's hand
                break;

            case GamePhase.PlayerChoose:
                // todo: allow player to choose a card from their hand
                break;

            case GamePhase.PlayerExecute:
                // todo: run animation for the player's chosen card
                break;

            case GamePhase.AiChoose:
                // todo: AI picks a card from their hand
                break;

            case GamePhase.AiExecute:
                // todo: run animation for the AI's chosen card
                break;

            case GamePhase.Decay:
                // todo: decay all cards in hand

                foreach (var card in playerHand)
                {
                    card.data.Health--;
                }
                foreach (var card in opponentHand)
                {
                    card.data.Health--;
                }

                break;
        }
    }

    private CardBehaviour DealCard(Deck deck, Vector2 position)
    {
        var cardObj = Instantiate(cardPrefab);
        cardObj.transform.SetParent(playerHandPanel, false);

        var transform = cardObj.GetComponent<RectTransform>();
        transform.anchoredPosition = position;

        var card = cardObj.GetComponent<CardBehaviour>();
        card.data = deck.GetNextCard();

        return card;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
