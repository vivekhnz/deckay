using System;
using System.Collections;
using System.Collections.Generic;


class CardData
{
    public int Health;
}

internal class Deck
{
    private readonly Random rng;
    public List<Card> cardList = new List<Card>();
    public Deck()
    {


        rng = new Random();
    }

    public CardData GetNextCard()
    {
        return new CardData
        {
            Health = rng.Next(3, 7)
        };
    }
}

