using System;
using System.Collections.Generic;
using Core.DeckSysteme.Cards;
using Core.Utilities;

namespace Core.DeckSysteme
{
    public class Hand
    {
        private readonly Deck deck;

        public List<CardInstance> Cards { get; }
        public List<CardInstance> SelectedCards { get; }

        public event Action<CardInstance, int> CardDrawn;
        public event Action<CardInstance> CardRemoved;
        public event Action<CardInstance> CardSelected;
        public event Action<CardInstance> CardDeselected;

        private readonly List<CardInstance> buffer;

        public Hand(Deck deck)
        {
            this.deck = deck;
            
            Cards = new List<CardInstance>(GameMetrix.MaxCardOnHand);
            SelectedCards = new List<CardInstance>(GameMetrix.MaxSelectable);
            buffer = new List<CardInstance>(GameMetrix.MaxSelectable);
        }

        public void FillHand()
        {
            int cardsToDraw = GameMetrix.MaxCardOnHand - Cards.Count;

            for (int i = 0; i < cardsToDraw; i++)
            {
                CardInstance drawnCard = deck.DrawTopCard();

                if (drawnCard == null)
                    break;

                Cards.Add(drawnCard);
                CardDrawn?.Invoke(drawnCard, Cards.Count - 1);
            }
        }

        public bool TrySelectCard(CardInstance card)
        {
            if (SelectedCards.Count >= GameMetrix.MaxSelectable)
                return false;

            if (SelectedCards.Contains(card) || !Cards.Contains(card))
                return false;

            SelectedCards.Add(card);
            CardSelected?.Invoke(card);
            return true;
        }

        public bool TryDeselectCard(CardInstance card)
        {
            if (!SelectedCards.Remove(card))
                return false;

            CardDeselected?.Invoke(card);
            return true;
        }

        public bool CanConfirmSelection()
        {
            int count = SelectedCards.Count;
            return count >= GameMetrix.MinSelectable && count <= GameMetrix.MaxSelectable;
        }

        public List<CardInstance> ConfirmSelectionToField()
        {
            if (!CanConfirmSelection())
                return null;

            buffer.Clear();
            buffer.AddRange(SelectedCards);
            RemoveFromHand(buffer);
            SelectedCards.Clear();
            return buffer;
        }

        public void DiscardSelection()
        {
            if (SelectedCards.Count == 0)
                return;

            buffer.Clear();
            buffer.AddRange(SelectedCards);
            RemoveFromHand(buffer);
            deck.AddToDiscard(buffer);
            SelectedCards.Clear();

            //FillHand();
        }

        private void RemoveFromHand(List<CardInstance> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                Cards.Remove(cards[i]);
                CardRemoved?.Invoke(cards[i]);
            }
        }
    }
}