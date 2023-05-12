using System;

namespace Assets
{
    class CardData
    {
        public int Health;
    }

    internal class Deck
    {
        private readonly Random rng;

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
}
