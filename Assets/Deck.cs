﻿using System;
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

    public CardData GetNextCard() // have player hand collection as argument
    {
        // if deck stack is empty call reshuffle

        // else, pop card off top of deck stack

        // add popped card, into current player hand

        return new CardData
        {
            Health = rng.Next(3, 7)
        };
    }
}

