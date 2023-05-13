using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public enum CardAction
{
    DoubleTurn = 0,
    Discard,
    ExtraTurn,
    Blind,
    Steal,
    LifeDrain,
    Agro,
    TakeLifeForce,
    Draw,
    LifeSteal,
    Skip,
    Refurbish,
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
            cardList.Add( new CardData { Effect = CardAction.Refurbish, Health = rng.Next(3, 7)});
            cardList.Add(new CardData { Effect = CardAction.WildCard, Health = rng.Next(3, 7) });
        }

        // adding 3 of each
        for (int i = 0; i < 3; i++)
        {
            cardList.Add(new CardData { Effect = CardAction.ExtraTurn, Health = rng.Next(3, 7) });
            cardList.Add(new CardData { Effect = CardAction.Blind, Health = rng.Next(3, 7) });
        }

        // adding 4 of each
        for (int i = 0; i < 4; i++)
        {
            cardList.Add(new CardData { Effect = CardAction.DoubleTurn, Health = rng.Next(3, 7) });
            cardList.Add(new CardData { Effect = CardAction.LifeDrain, Health = rng.Next(3, 7) });
            cardList.Add(new CardData { Effect = CardAction.Draw, Health = rng.Next(3, 7) });
            cardList.Add(new CardData { Effect = CardAction.LifeSteal, Health = rng.Next(3, 7) });
            cardList.Add(new CardData { Effect = CardAction.Skip, Health = rng.Next(3, 7) });
        }

        // adding 5 of each
        for (int i = 0; i < 5; i++)
        {
            cardList.Add(new CardData { Effect = CardAction.TakeLifeForce, Health = rng.Next(3, 7) });
            cardList.Add(new CardData { Effect = CardAction.Agro, Health = rng.Next(3, 7) });
            cardList.Add(new CardData { Effect = CardAction.Steal, Health = rng.Next(3, 7) });
            cardList.Add(new CardData { Effect = CardAction.Discard, Health = rng.Next(3, 7) });
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

