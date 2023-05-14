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
    PlayerPickUp,
    AiChoose,
    AiExecute,
    AiDecay,
    AiPickUp,

    GameWon,
    GameLost,
}

public enum GameFlowModifier
{
    Player_PlayExtraCard,
    Opponent_PlayExtraCard,

    Player_SkipDecay,
    Opponent_SkipDecay,

    Player_DrawBonusCard,
    Opponent_DrawBonusCard
}

public enum Actor
{
    Player,
    Opponent
}

public class ActorState
{
    public Actor Actor;
    public List<CardData> CardsInHand = new List<CardData>();
    public CardData SelectedCard = null;

    public ActorState(Actor actor)
    {
        Actor = actor;
    }

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
    private List<GameFlowModifier> gameFlowModifiers;

    public GamePhase CurrentPhase;
    public ActorState Player;
    public ActorState Opponent;

    public CardGame()
    {
        rng = new Random();

        deck = new Deck();

        gameFlowModifiers = new List<GameFlowModifier>();

        CurrentPhase = GamePhase.Startup;
        Player = new ActorState(Actor.Player);
        Opponent = new ActorState(Actor.Opponent);
    }

    /// <returns>Whether the specified game flow modifier is active.</returns>
    private bool UseFlowModifierIfActive(GameFlowModifier modifier)
    {
        if (gameFlowModifiers.Contains(modifier))
        {
            gameFlowModifiers.Remove(modifier);
            return true;
        }
        return false;
    }

    private GamePhase GetNextPhase(GamePhase currentPhase)
    {
        switch (currentPhase)
        {
            case GamePhase.Startup:
                return GamePhase.Dealing;
            case GamePhase.Dealing:
                return GamePhase.PlayerChoose;

            case GamePhase.PlayerChoose:
                return GamePhase.PlayerExecute;
            case GamePhase.PlayerExecute:
                if (UseFlowModifierIfActive(GameFlowModifier.Player_SkipDecay))
                {
                    return GetNextPhase(GamePhase.PlayerDecay);
                }
                return GamePhase.PlayerDecay;
            case GamePhase.PlayerDecay:
                return GamePhase.PlayerPickUp;
            case GamePhase.PlayerPickUp:
                if (UseFlowModifierIfActive(GameFlowModifier.Player_PlayExtraCard))
                {
                    return GamePhase.PlayerChoose;
                }
                return GamePhase.AiChoose;

            case GamePhase.AiChoose:
                return GamePhase.AiExecute;
            case GamePhase.AiExecute:
                if (UseFlowModifierIfActive(GameFlowModifier.Opponent_SkipDecay))
                {
                    return GetNextPhase(GamePhase.AiDecay);
                }
                return GamePhase.AiDecay;
            case GamePhase.AiDecay:
                return GamePhase.AiPickUp;
            case GamePhase.AiPickUp:
                if (UseFlowModifierIfActive(GameFlowModifier.Opponent_PlayExtraCard))
                {
                    return GamePhase.AiChoose;
                }
                return GamePhase.PlayerChoose;
        }

        throw new NotImplementedException();
    }

    internal void MoveToNextPhase()
    {
        CurrentPhase = GetNextPhase(CurrentPhase);

        // begin the next phase
        switch (CurrentPhase)
        {
            case GamePhase.Dealing:
                DealCards(initialHandSize);
                break;

            case GamePhase.PlayerChoose:
                // if the player is down to their last card, they can't win
                // zero out the card health so it perishes
                if (Player.CardsInHand.Count == 1)
                {
                    Player.CardsInHand[0].Health = 0;
                }
                break;

            case GamePhase.PlayerExecute:
                Execute(Actor.Player);
                break;

            case GamePhase.PlayerDecay:
                DecayCards(Actor.Player);
                break;

            case GamePhase.PlayerPickUp:
                PickUpCard(Actor.Player);
                while (UseFlowModifierIfActive(GameFlowModifier.Player_DrawBonusCard))
                {
                    PickUpCard(Actor.Player);
                }
                break;

            case GamePhase.AiChoose:
                // if the AI is down to their last card, they can't win
                // zero out the card health so it perishes
                if (Opponent.CardsInHand.Count == 1)
                {
                    Opponent.CardsInHand[0].Health = 0;
                }
                break;

            case GamePhase.AiExecute:
                // AI chooses a card
                var selectedCard = RandomCard(Opponent.CardsInHand);
                ChooseCard(Actor.Opponent, selectedCard);

                // AI executes that card
                Execute(Actor.Opponent);
                break;

            case GamePhase.AiDecay:
                DecayCards(Actor.Opponent);
                break;

            case GamePhase.AiPickUp:
                PickUpCard(Actor.Opponent);
                while (UseFlowModifierIfActive(GameFlowModifier.Opponent_DrawBonusCard))
                {
                    PickUpCard(Actor.Opponent);
                }
                break;
        }

        // remove perished cards
        var actors = new[] { Player, Opponent };
        foreach (var actor in actors)
        {
            for (int i = 0; i < actor.CardsInHand.Count; i++)
            {
                var card = actor.CardsInHand[i];
                if (card.DestroyEffect != CardDestroyEffect.None)
                {
                    Debug.LogWarning($"Card in hand does not have a destroy effect!");
                }
                if (card.Health <= 0)
                {
                    Debug.Log($"Card perished: {card.DisplayName} @ {card.Health}HP");

                    RemoveFromHand(actor, card, CardDestroyEffect.Perished);
                    i--;
                }
            }
        }

        // evaluate win condition
        if (Player.CardsInHand.Count == 0)
        {
            CurrentPhase = GamePhase.GameLost;
        }
        else if (Opponent.CardsInHand.Count == 0)
        {
            CurrentPhase = GamePhase.GameWon;
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
        RemoveFromHand(actorState, card, CardDestroyEffect.Selected);
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

        // ensure that cards are face up & wildcards are revealed when executing
        var executingCard = me.SelectedCard;
        executingCard.IsFaceDown = false;
        executingCard.IsWildcard = false;

        var modifiers = ExecuteCard(executingCard, me, opponent);
        foreach (var modifier in modifiers)
        {
            gameFlowModifiers.Add(modifier);
        }
    }

    private List<GameFlowModifier> ExecuteCard(CardData card, ActorState me, ActorState opponent)
    {
        GameManagerBehaviour.FindObjectOfType<AudioManager>().PlaySound(card.DisplayName);
        var flowModifiers = new List<GameFlowModifier>();

        Debug.Log($"Executing card effect '{card.Effect}'");
        switch (card.Effect)
        {
            case CardAction.DoubleTurn:
                {
                    gameFlowModifiers.Add(opponent.Actor == Actor.Player
                        ? GameFlowModifier.Player_PlayExtraCard
                        : GameFlowModifier.Opponent_PlayExtraCard);
                }
                break;
            case CardAction.Discard:
                {
                    var targetCard = RandomCard(opponent.CardsInHand);
                    RemoveFromHand(opponent, targetCard, CardDestroyEffect.Discarded);
                }
                break;
            case CardAction.Blind:
                {
                    var targetCard = RandomCard(opponent.CardsInHand);
                    targetCard.IsFaceDown = !targetCard.IsFaceDown;
                }
                break;
            case CardAction.Steal:
                {
                    var targetCard = RandomCard(opponent.CardsInHand);
                    RemoveFromHand(opponent, targetCard, CardDestroyEffect.Stolen);
                    me.CardsInHand.Add(targetCard);
                    // HACK: Add one health to the stolen card so it doesn't immediately perish
                    targetCard.Health++;

                    targetCard.IsFaceDown = me.Actor == Actor.Player ? false : true;
                }
                break;
            case CardAction.Aggro:
                {
                    var targetCard = RandomCard(opponent.CardsInHand);
                    targetCard.Health -= 2;
                }
                break;
            case CardAction.TakeLifeForce:
                {
                    foreach (var cardInHand in opponent.CardsInHand)
                    {
                        cardInHand.Health--;
                    }
                }
                break;
            case CardAction.Draw:
                {
                    gameFlowModifiers.Add(me.Actor == Actor.Player
                        ? GameFlowModifier.Player_DrawBonusCard
                        : GameFlowModifier.Opponent_DrawBonusCard);
                }
                break;
            case CardAction.LifeSteal:
                {
                    RandomCard(opponent.CardsInHand).Health--;
                    RandomCard(me.CardsInHand).Health++;
                }
                break;
            case CardAction.Skip:
                {
                    gameFlowModifiers.Add(me.Actor == Actor.Player
                        ? GameFlowModifier.Player_SkipDecay
                        : GameFlowModifier.Opponent_SkipDecay);
                }
                break;
            case CardAction.Refurbish:
                {
                    var targetCard = RandomCard(me.CardsInHand);
                    targetCard.Health += 2;
                }
                break;
        }

        return flowModifiers;
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

    private CardData RandomCard(List<CardData> choices)
    {
        return choices[rng.Next(0, choices.Count)];
    }

    private void RemoveFromHand(ActorState actor, CardData card, CardDestroyEffect effect)
    {
        card.DestroyEffect = effect;
        actor.CardsInHand.Remove(card);
    }
}