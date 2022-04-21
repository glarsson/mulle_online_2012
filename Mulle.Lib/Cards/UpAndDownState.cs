using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mulle.Lib.Cards
{
    public class UpAndDownState : IPileState
    {
        public void BuildUp(CardPile pile, Card card)
        {
            pile._cards.Add(card);
            pile.Value += (int)card.Value;
        }

        public void BuildDown(CardPile pile, Card card)
        {
            pile._cards.Add(card);
            pile.Value -= (int)card.Value;
        }

        public void LockPile(CardPile pile, Card card)
        {
            if (pile.Value == (int)card.Value)
            {
                pile._cards.Add(card);
                pile.State = new LockState();
            }
        }

        public void AddSequence(CardPile pile, IList<Card> cards)
        {
            if (pile.Value == cards.Sum(c => (int)c.Value))
            {
                foreach (var card in cards)
                {
                    pile._cards.Add(card);
                    pile.State = new LockState();
                }
            }
        }
    }
}
