using System;
using System.Collections;
using System.Collections.Generic;


class CardData
{
    public int Health;
    public string effectText;
}

internal class Deck
{
    private readonly Random rng;
    public List<CardData> cardList = new List<CardData>();
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

