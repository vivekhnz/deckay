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

    private int handSize = 5;
    private Deck deck;
    private GamePhase currentPhase;
    private List<CardBehaviour> playerHand = new List<CardBehaviour>();
    private List<CardBehaviour> opponentHand = new List<CardBehaviour>();
    private CardBehaviour playerChosenCard = null;
    private CardBehaviour opponentChosenCard = null;

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

    public void ChooseCardFromPlayerHand(CardBehaviour card)
    {
        Debug.Log("Was Clicked" + card.data.Health);
        playerChosenCard = card;
        MoveToNextPhase();
    }

    void BeginPhase(GamePhase phase)
    {
        currentPhaseText.text = $"Current phase: {phase}";
        switch (phase)
        {
            case GamePhase.Dealing:
                deck = new Deck();

                playerHand.Clear();

                // replace for loop with card count possibly. 
                for (int i = 0; i < handSize; i++)
                {
                    CardBehaviour card;
                    
                    if (i % 2 == 1)
                    {
                        card = DealCard(deck, new Vector2(((i * 50) + 50), 0));
                    }
                    else
                    {
                        card = DealCard(deck, new Vector2(i * -50, 0));
                    }
                    playerHand.Add(card);
                }

                // todo: deal opponent's hand
                break;

            case GamePhase.PlayerChoose:
                // todo: allow player to choose a card from their hand
                for(int i = 0; i < handSize; i++)
                {
                    playerHand[i].isClickable = true;
                }
                playerChosenCard = playerHand[0];
                break;

            case GamePhase.PlayerExecute:
                ExecuteCard(playerChosenCard, true);
                for (int i = 0; i < handSize; i++)
                {
                    playerHand[i].isClickable = false;
                }
                break;

            case GamePhase.AiChoose:
                // todo: AI picks a card from their hand
                opponentChosenCard = opponentHand[0];
                break;

            case GamePhase.AiExecute:
                ExecuteCard(opponentChosenCard, false);
                break;

            case GamePhase.Decay:
                List<CardBehaviour> toRemove = new List<CardBehaviour>();
                foreach (var card in playerHand)
                {
                    card.data.Health--;
                    if(card.data.Health == 0)
                    {
                        toRemove.Add(card);
                    }
                }
                foreach (var card in toRemove)
                {
                    playerHand.Remove(card);
                }
                toRemove.Clear();
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
                }


                // todo: discard cards that have reached 0 health

                break;
        }
    }

    private CardBehaviour DealCard(Deck deck, Vector2 position)
    {
        var cardObj = Instantiate(cardPrefab);
        
        // if opponent set as opponent panel 
        cardObj.transform.SetParent(playerHandPanel, false);

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

    private void ExecuteCard(CardBehaviour card, bool isPlayerCard)
    {
        // todo: execute card's effect
        // todo: run animation for the player's chosen card

        DiscardCard(card);
    }

    private void DiscardCard(CardBehaviour card)
    {
        // todo: discard card
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
