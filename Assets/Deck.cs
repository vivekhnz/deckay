using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public enum CardAction
{
    /// <summary>Your opponent must take two turns.</summary>
    DoubleTurn = 0,
    /// <summary>Your opponent must discard 1 card from their hand.</summary>
    Discard,
    /// <summary>You must take an extra turn.</summary>
    ExtraTurn,
    /// <summary>One random card from your hand is hidden from view.</summary>
    Blind,
    /// <summary>Steal 1 random card from your opponent's hand.</summary>
    Steal,
    /// <summary>You lose 1 life from each of the cards in your hand.</summary>
    LifeDrain,
    /// <summary>Take 2 life from one of your opponent's cards.</summary>
    Agro,
    /// <summary>Your opponent loses 1 life from each of the cards in their hand.</summary>
    TakeLifeForce,
    /// <summary>You may draw an extra card at the end of your turn.</summary>
    Draw,
    /// <summary>Take 1 life from your opponent's card and add it to one of your own.</summary>
    LifeSteal,
    /// <summary>You do not lose any life this turn.</summary>
    Skip,
    /// <summary>One card in your hand gains 2 life.</summary>
    Refurbish,
    /// <summary>Fate will decide...</summary>
    WildCard
}


public class CardData
{
    public int Health;
    public CardAction Effect;
    public bool IsFaceDown;
}

internal class Deck
{
    private readonly Random rng;
    public List<CardData> cardList;
    private int deckSize = 50;
    private int topOfDeck = 0;
    public Deck()
    {
        rng = new Random();
        fillDeck();
        shuffle();
    }

    public void shuffle()
    {
        for(int i = 0; i < deckSize; i++)
        {
            CardData temp = cardList[i];
            int randomIndex = rng.Next(i, deckSize);
            cardList[i] = cardList[randomIndex];
            cardList[randomIndex] = temp;
        }
    }

    public void fillDeck()
    {
        cardList = new List<CardData>();

        // adding 2 of each
        for(int i = 0; i < 2; i++)
        {
            cardList.Add( new CardData { Effect = CardAction.Refurbish, Health = 2});
            cardList.Add(new CardData { Effect = CardAction.WildCard, Health = 2 });
        }

        // adding 3 of each
        for (int i = 0; i < 3; i++)
        {
            cardList.Add(new CardData { Effect = CardAction.ExtraTurn, Health = 5 });
            cardList.Add(new CardData { Effect = CardAction.Blind, Health = 4 });
        }

        // adding 4 of each
        for (int i = 0; i < 4; i++)
        {
            cardList.Add(new CardData { Effect = CardAction.DoubleTurn, Health = 5 });
            cardList.Add(new CardData { Effect = CardAction.LifeDrain, Health = 3 });
            cardList.Add(new CardData { Effect = CardAction.Draw, Health = 6 });
            cardList.Add(new CardData { Effect = CardAction.LifeSteal, Health = 3 });
            cardList.Add(new CardData { Effect = CardAction.Skip, Health = 3 });
        }

        // adding 5 of each
        for (int i = 0; i < 5; i++)
        {
            cardList.Add(new CardData { Effect = CardAction.TakeLifeForce, Health = 4 });
            cardList.Add(new CardData { Effect = CardAction.Agro, Health = 3 });
            cardList.Add(new CardData { Effect = CardAction.Steal, Health = 2 });
            cardList.Add(new CardData { Effect = CardAction.Discard, Health = 4 });
        }

        // check the whole deck was loaded correctly 
        if (cardList.Count == deckSize) { Debug.Log("Deck Loaded!"); }
    }

    public CardData GetNextCard() // have player hand collection as argument
    {
        // if deck stack is empty call reshuffle
        if(topOfDeck == deckSize)
        {
            topOfDeck = 0;
            fillDeck();
            shuffle();
        }
        topOfDeck++;
        return cardList[topOfDeck - 1];
    }
}

