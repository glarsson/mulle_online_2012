using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mulle.Lib.Cards
{
    public class Deck
    {
        public IList<Card> _cards;

        public Deck()
        {
            _cards = CreateDeck();
        }

        private IList<Card> CreateDeck()
        {

            var cards = new List<Card>();
            var numberOfDecks = 0;

            while (numberOfDecks < 2)
            {
                for (int suit = 0; suit < 4; suit++)
                {
                    for (int color = 1; color < 14; color++)
                    {
                        cards.Add(new Card { Value = (CardValue)color, Suit = (CardSuit)suit, ImagePath = string.Format("Resources/{0}{1}.bmp", (CardSuit)suit, (CardValue)color) });
                    }
                }
                numberOfDecks++;
            }
            
            return cards;
        }

        public void Shuffle()
        {
            //Add logic here
            var tempCards = new List<Card>();
            var random = new Random();

            while (_cards.Any())
            {
                var index = random.Next(_cards.Count);
                tempCards.Add(_cards[index]);
                _cards.Remove(_cards[index]);
            }

            _cards = tempCards;

        }

        public Card Deal()
        {
            var card = _cards[0];
            _cards.Remove(_cards[0]);
            return card;
        }
    }
}
