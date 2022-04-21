using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mulle.Lib.Cards
{
    public interface IPileState
    {
        void BuildUp(CardPile pile, Card card);
        void BuildDown(CardPile pile, Card card);
        void LockPile(CardPile pile, Card card);
        void AddSequence(CardPile pile, IList<Card> cards);
    }
}
