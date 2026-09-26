using System;
using System.Collections.Generic;
using Core.DeckSysteme.Cards;
using Core.Utilities;

namespace Core.DeckSysteme
{
    public class Hand
    {
        private readonly Deck deck;

        public List<CardInstance> Cards { get; } = new List<CardInstance>(GameMetrix.Instance.MaxCardOnHand);
        public List<CardInstance> SelectedCards { get; } = new List<CardInstance>(GameMetrix.Instance.MaxSelectable);
        
        public event Action<CardInstance, int> CardDrawn;
        public event Action<CardInstance> CardRemoved;
        public event Action<CardInstance> CardSelected;
        public event Action<CardInstance> CardDeselected;

        private readonly List<CardInstance> buffer = new List<CardInstance>(GameMetrix.Instance.MaxSelectable);

        public Hand(Deck deck)
        {
            this.deck = deck;
        }

        public void FillHand()
        {
            int cardsToDraw = GameMetrix.Instance.MaxCardOnHand - Cards.Count;

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
            if (SelectedCards.Count >= GameMetrix.Instance.MaxSelectable)
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
            return count >= GameMetrix.Instance.MinSelectable && count <= GameMetrix.Instance.MaxSelectable;
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

            FillHand();
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