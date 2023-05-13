using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

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


    private readonly Random rng;

    public ActorState Player;
    public ActorState Opponent;

    public CardGame()
	{
        rng = new Random();
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

    /// <returns>Cards that decayed to zero health.</returns>
    internal List<CardData> DecayCards(Actor actor)
    {
        var actorState = GetActorState(actor);

        var removedCards = new List<CardData>();
        for (int i = 0; i < actorState.CardsInHand.Count; i++)
        {
            var card = actorState.CardsInHand[i];
            card.Health--;
            if (card.Health <= 0)
            {
                removedCards.Add(card);
                actorState.CardsInHand.RemoveAt(i);
                i--;
            }
        }

        return removedCards;
    }

    internal void Execute(Actor user)
    {
        ActorState me;
        ActorState opponent;
        switch (user)
        {
            case Actor.Player:
                me = Player;
                opponent = Opponent;
                break;
            case Actor.Opponent:
                me = Opponent;
                opponent = Player;
                break;
            default:
                throw new NotImplementedException();
        }

        // ensure that cards are always face up when executing
        var executingCard = me.SelectedCard;
        executingCard.IsFaceDown = false;
        ExecuteCard(executingCard, me, opponent);
    }

    private void ExecuteCard(CardData card, ActorState me, ActorState opponent)
    {
        // todo: update actor states according to card's effect
        Debug.Log($"Executing card effect '{card.Effect}'");
        switch (card.Effect)
        {
            case CardAction.DoubleTurn:
                break;
            case CardAction.Discard:
                CardData removedCard = opponent.CardsInHand[0];
                opponent.CardsInHand.Remove(removedCard);
                break;
            case CardAction.ExtraTurn:
                break;
            case CardAction.Blind:
                CardData blindCard = opponent.CardsInHand[0];
                blindCard.IsFaceDown = blindCard.IsFaceDown ? false : true;
                break;
            case CardAction.Steal:
                CardData oppCard = opponent.CardsInHand[0];
                CardData stolenCard = new CardData { Health = oppCard.Health, Effect = oppCard.Effect, IsFaceDown = oppCard.IsFaceDown ? false : true };
                opponent.CardsInHand.Remove(oppCard);
                me.CardsInHand.Add(stolenCard);
                break;
            case CardAction.LifeDrain:
                foreach(CardData handCard in me.CardsInHand)
                {
                    handCard.Health = handCard.Health - 1;
                }
                break;
            case CardAction.Agro:
                CardData selectedCard = opponent.CardsInHand[0];
                selectedCard.Health = selectedCard.Health - 2;
                break;
            case CardAction.TakeLifeForce:
                foreach (CardData handCard in opponent.CardsInHand)
                {
                    handCard.Health = handCard.Health - 1;
                }
                break;
            case CardAction.Draw:
                me.CardsInHand.Add(deck.GetNextCard());
                break;
            case CardAction.LifeSteal:
                me.CardsInHand[0].Health += 1;
                opponent.CardsInHand[0].Health -= 1;
                break;
            case CardAction.Skip:
                break;
            case CardAction.Refurbish:
                me.CardsInHand[0].Health += 2;
                break;
            case CardAction.WildCard:
                int roll = rng.Next(1, 7);
                break;
        }
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