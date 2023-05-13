using System;
using System.Collections.Generic;

public enum Hand
{
    Player,
    Opponent
}

internal class CardGame
{
    private Deck deck;

    public List<CardData> PlayerHand;
    public List<CardData> OpponentHand;

    public CardGame()
	{
		deck = new Deck();
        PlayerHand = new List<CardData>();
        OpponentHand = new List<CardData>();
	}

    internal void DealCards(int cardsPerHand)
    {
        PlayerHand.Clear();
        for (int i = 0; i < cardsPerHand; i++)
        {
            var cardData = deck.GetNextCard();
            PlayerHand.Add(cardData);
        }

        OpponentHand.Clear();
        for (int i = 0; i < cardsPerHand; i++)
        {
            var cardData = deck.GetNextCard();
            cardData.IsFaceDown = true;
            OpponentHand.Add(cardData);
        }
    }

    internal CardData PickUpCard(Hand hand)
    {
        var card = deck.GetNextCard();

        switch (hand)
        {
            case Hand.Player:
                PlayerHand.Add(card);
                break;
            case Hand.Opponent:
                card.IsFaceDown = true;
                OpponentHand.Add(card);
                break;
            default:
                throw new NotImplementedException();
        }

        return card;
    }
}