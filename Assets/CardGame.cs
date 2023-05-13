using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public enum GamePhase
{
    Startup = 0,
    Dealing,
    PlayerChoose,
    PlayerExecute,
    PlayerDecay,
    AiChoose,
    AiExecute,
    AiDecay
}

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
    private readonly Random rng;
    private const int initialHandSize = 5;

    private Deck deck;

    public GamePhase CurrentPhase;
    public ActorState Player;
    public ActorState Opponent;

    public CardGame()
	{
        rng = new Random();
        deck = new Deck();
        CurrentPhase = GamePhase.Startup;
        Player = new ActorState();
        Opponent = new ActorState();
    }

    internal void MoveToNextPhase()
    {
        if (CurrentPhase == GamePhase.AiDecay)
        {
            CurrentPhase = GamePhase.PlayerChoose;
        }
        else
        {
            CurrentPhase++;
        }

        // begin the next phase
        switch (CurrentPhase)
        {
            case GamePhase.Dealing:
                DealCards(initialHandSize);
                break;

            case GamePhase.PlayerChoose:
                // allow player to choose a card from their hand
                break;

            case GamePhase.PlayerExecute:
                Execute(Actor.Player);
                break;

            case GamePhase.PlayerDecay:
                DecayCards(Actor.Player);
                PickUpCard(Actor.Player);
                break;

            case GamePhase.AiChoose:
                // pretend that AI is picking a card
                break;

            case GamePhase.AiExecute:
                // AI chooses a card
                var selectedCard = Opponent.CardsInHand[0];
                ChooseCard(Actor.Opponent, selectedCard);

                // AI executes that card
                Execute(Actor.Opponent);
                break;

            case GamePhase.AiDecay:
                DecayCards(Actor.Opponent);
                PickUpCard(Actor.Opponent);
                break;
        }

        // remove expired cards
        var actors = new[] { Player, Opponent };
        foreach (var actor in actors)
        {
            for (int i = 0; i < actor.CardsInHand.Count; i++)
            {
                var card = actor.CardsInHand[i];
                if (card.Health <= 0)
                {
                    actor.CardsInHand.RemoveAt(i);
                    i--;
                }
            }
        }
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

    internal void DecayCards(Actor actor)
    {
        var actorState = GetActorState(actor);
        foreach (var card in actorState.CardsInHand)
        {
            card.Health--;
        }
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