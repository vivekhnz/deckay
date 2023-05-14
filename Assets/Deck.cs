using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public enum CardAction
{
    /// <summary>Your opponent must take two turns.</summary>
    DoubleTurn = 0,
    /// <summary>Your opponent must discard 1 card from their hand.</summary>
    Discard,
    /// <summary>One random card from your hand is hidden from view.</summary>
    Blind,
    /// <summary>Steal 1 random card from your opponent's hand.</summary>
    Steal,
    /// <summary>You lose 1 life from each of the cards in your hand.</summary>
    LifeDrain,
    /// <summary>Take 2 life from one of your opponent's cards.</summary>
    Aggro,
    /// <summary>Your opponent loses 1 life from each of the cards in their hand.</summary>
    TakeLifeForce,
    /// <summary>You may draw an extra card at the end of your turn.</summary>
    Draw,
    /// <summary>Take 1 life from your opponent's card and add it to one of your own.</summary>
    LifeSteal,
    /// <summary>You do not lose any life this turn.</summary>
    Skip,
    /// <summary>One card in your hand gains 2 life.</summary>
    Refurbish
}


public class CardData
{
    public int Health;
    public CardAction Effect;
    public bool IsFaceDown;
    public bool IsWildcard;
}

internal class Deck
{
    private readonly Random rng;
    public List<CardData> cardList;
    private int topOfDeck = 0;

    public Deck()
    {
        rng = new Random();
        fillDeck();
        shuffle();
    }

    public void shuffle()
    {
        for(int i = 0; i < cardList.Count; i++)
        {
            CardData temp = cardList[i];
            int randomIndex = rng.Next(i, cardList.Count);
            cardList[i] = cardList[randomIndex];
            cardList[randomIndex] = temp;
        }
    }

    public void fillDeck()
    {
        cardList = new List<CardData>();

        // adding 2 of each
        for(int i = 0; i < 5; i++)
        {
            cardList.Add( new CardData { Effect = CardAction.Refurbish, Health = 2});
        }

        // adding 3 of each
        for (int i = 0; i < 3; i++)
        {
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
            cardList.Add(new CardData { Effect = CardAction.Aggro, Health = 3 });
            cardList.Add(new CardData { Effect = CardAction.Steal, Health = 2 });
            cardList.Add(new CardData { Effect = CardAction.Discard, Health = 4 });
        }

        // mark a certain no. of cards as wildcards
        int wildcardCount = 2;
        var wildcardIndexes = new List<int>();
        while (wildcardIndexes.Count < wildcardCount)
        {
            int index = rng.Next(cardList.Count);
            if (!wildcardIndexes.Contains(index))
            {
                wildcardIndexes.Add(index);
            }
        }
        foreach (int index in wildcardIndexes)
        {
            cardList[index].IsWildcard = true;
        }
    }

    public CardData GetNextCard() // have player hand collection as argument
    {
        // if deck stack is empty call reshuffle
        if(topOfDeck == cardList.Count)
        {
            topOfDeck = 0;
            fillDeck();
            shuffle();
        }
        topOfDeck++;
        return cardList[topOfDeck - 1];
    }
}

