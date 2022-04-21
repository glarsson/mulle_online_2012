using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mulle.Lib.Cards
{
    public class CardPile
    {
        public Guid Guid { get; set; }
        public bool IsLocked { get; set; }
        public int Value { get; set; }
        public int TotalScore { get; set; }
        public IList<Card> _cards;
        public IPileState State { get; set; }
        public CardPile()
        {

        }
        public CardPile(Card card)
        {
            _cards = new List<Card>{ new Card{ Value = card.Value, Suit = card.Suit, ImagePath = card.ImagePath }};
            Value = (int)card.Value;

        }

        public void BuildUp(Card card)
        {
            State.BuildUp(this, card);
        }

        public void BuildDown(Card card)
        {
            State.BuildDown(this, card);
        }

        public void Lock(Card card)
        {
            State.LockPile(this, card);
        }

        public void AddSequence(IList<Card> cards)
        {
            State.AddSequence(this, cards);
        }

        public int CalculateTotalScore()
        {
            var allSpades = _cards.Where(c => c.Suit == CardSuit.Spade);
            var allAces = _cards.Where(c => c.Value == CardValue.Ace);
            var tenOfDiamonds = _cards.Where(c => c.Value == CardValue.Ten && c.Suit == CardSuit.Diamond);
            var duceOfSpades = _cards.Where(c => c.Value == CardValue.Two && c.Suit == CardSuit.Spade);
            var mullar = GetAllDuplicates();

            var totalScore = 0;

            foreach (var spade in allSpades)
            {
                TotalScore += 1;
            }

            foreach (var ace in allAces)
            {
                TotalScore += 1;
            }

            foreach (var card in mullar)
            {
                if (card.Value == CardValue.Ace && this.Value == 14)
                {
                    totalScore += 14;
                }
                else if(this.Value == 15 && card.Value == CardValue.Two && card.Suit == CardSuit.Spade)
                {
                    totalScore += 15;
                }
                else if(this.Value == 16 && card.Value == CardValue.Ten && card.Suit == CardSuit.Diamond)
                {
                    totalScore += 16;
                }
                else
                {
                    totalScore += (int)card.Value;
                }
            }

            foreach (var card in tenOfDiamonds)
            {
                totalScore += 2;
            }

            return totalScore;

        }

        private IEnumerable<Card> GetAllDuplicates()
        {
            for (int i = 0; i < _cards.Count - 1; i++)
            {
                for (int j = i+1; j < _cards.Count; j++)
                {
                    var card1 = _cards[i];
                    var card2 = _cards[j];
                    if (card1.Equals(card2))
                    {
                        yield return card1;
                    }
                }
            }
        }

    }
}
