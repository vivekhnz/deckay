using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;


class CardData
{
    public int Health;
    public string Effect;


}

internal class Deck
{
    private readonly Random rng;
    public List<CardData> cardList = new List<CardData>();
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
        for(int i = 0; i < deckSize; i++)
        {
            if (i < deckSize - 40)
            {
                Debug.Log("Spawned Nuetral");
                cardList.Add( new CardData { Effect = "Nuetral", Health = rng.Next(3, 7)});
            }
            else if (i >= deckSize - 40 && i < deckSize-10)
            {
                Debug.Log("Spawned Attack");
                cardList.Add(new CardData { Effect = "Attack", Health = rng.Next(3, 7)});
            }
            else if (i >= deckSize - 10 && i < deckSize)
            {
                Debug.Log("Spawned Defend");
                cardList.Add(new CardData { Effect = "Defend", Health = rng.Next(3, 7)});
            }
        }
    }

    public CardData GetNextCard() // have player hand collection as argument
    {
        // if deck stack is empty call reshuffle

        // else, pop card off top of deck stack

        // add popped card, into current player hand

        // return new CardData
        // {
        //     Health = rng.Next(3, 7)
        // };
        topOfDeck++;
        return cardList[topOfDeck - 1];
    }
}

