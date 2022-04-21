using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mulle.Lib.Cards
{
    public enum CardValue
    {
        Ace = 1,
        Two = 2,
        Three = 3,
        Four = 4, 
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13

	}

    public enum CardSuit
    {
        Heart,
        Spade,
        Club,
        Diamond
    }

    public struct Card : IEqualityComparer<Card>
    {
        public CardSuit Suit{ get; set; }
        public CardValue Value { get; set; }
        public string ImagePath { get; set; }
        public int ActualValue { get; set; }
        public bool IsOnHand { get; set; }

        public override string ToString()
        {
            return string.Format("{0} of {1}\n image path: {2}", Value, Suit, ImagePath);
        }

        public bool Equals(Card x, Card y)
        {
            return x.Value == y.Value && x.Suit == y.Suit;
        }

        public int GetHashCode(Card obj)
        {
            return obj.GetHashCode();
        }
    }
}
