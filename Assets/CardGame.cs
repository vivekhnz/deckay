using System;
using System.Collections.Generic;
using UnityEngine;

public enum Actor
{
    Player,
    Opponent
}

public class ActorState
{
    public List<CardData> CardsInHand = new List<CardData>();
    public CardData SelectedCard = null;

    public void Reset()
    {
        CardsInHand.Clear();
        SelectedCard = null;
    }
}

internal class CardGame
{
    private Deck deck;

    public ActorState Player;
    public ActorState Opponent;

    public CardGame()
	{
		deck = new Deck();
        Player = new ActorState();
        Opponent = new ActorState();
    }

    internal void DealCards(int cardsPerHand)
    {
        Player.Reset();
        for (int i = 0; i < cardsPerHand; i++)
        {
            var cardData = deck.GetNextCard();
            Player.CardsInHand.Add(cardData);
        }

        Opponent.Reset();
        for (int i = 0; i < cardsPerHand; i++)
        {
            var cardData = deck.GetNextCard();
            cardData.IsFaceDown = true;
            Opponent.CardsInHand.Add(cardData);
        }
    }

    internal CardData PickUpCard(Actor actor)
    {
        var card = deck.GetNextCard();

        switch (actor)
        {
            case Actor.Player:
                Player.CardsInHand.Add(card);
                break;
            case Actor.Opponent:
                card.IsFaceDown = true;
                Opponent.CardsInHand.Add(card);
                break;
            default:
                throw new NotImplementedException();
        }

        return card;
    }

    internal void ChooseCard(Actor actor, CardData card)
    {
        var actorState = GetActorState(actor);
        Debug.Assert(actorState.CardsInHand.Contains(card));

        actorState.SelectedCard = card;
        actorState.CardsInHand.Remove(card);
    }

    private ActorState GetActorState(Actor actor)
    {
        switch (actor)
        {
            case Actor.Player:
                return Player;
            case Actor.Opponent:
                return Opponent;
            default:
                throw new NotImplementedException();
        }
    }
}