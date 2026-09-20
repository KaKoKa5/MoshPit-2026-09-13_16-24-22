using System;
using System.Collections.Generic;
using Core.Utilities;
using GamePlay.Card;

namespace Core.DeckSysteme.BackEnd
{
    public class Hand
    {
        private readonly Deck deck;

        public List<CardInfoData> Cards { get; } = new List<CardInfoData>(GameMetrix.Instance.InitialPoolSize);
        public List<CardInfoData> SelectedCards { get; } = new List<CardInfoData>(GameMetrix.Instance.MaxSelectable);
        
        public event Action<CardInfoData> CardDrawn;
        public event Action<CardInfoData> CardRemoved; 
        public event Action<CardInfoData> CardSelected;
        public event Action<CardInfoData> CardDeselected;

        private readonly List<CardInfoData> buffer = new List<CardInfoData>(GameMetrix.Instance.MaxSelectable);

        public Hand(Deck deck)
        {
            this.deck = deck;
        }

        public void FillHand()
        {
            int cardsToDraw = GameMetrix.Instance.MaxCardOnHand - Cards.Count;

            for (int i = 0; i < cardsToDraw; i++)
            {
                CardInfoData drawnCard = deck.DrawTopCard();

                if (drawnCard == null)
                    break;

                Cards.Add(drawnCard);
                CardDrawn?.Invoke(drawnCard);
            }
        }

        public bool TrySelectCard(CardInfoData card)
        {
            if (SelectedCards.Count >= GameMetrix.Instance.MaxSelectable)
                return false;

            if (SelectedCards.Contains(card) || !Cards.Contains(card))
                return false;

            SelectedCards.Add(card);
            CardSelected?.Invoke(card);
            return true;
        }

        public bool TryDeselectCard(CardInfoData card)
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

        public List<CardInfoData> ConfirmSelectionToField()
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

        private void RemoveFromHand(List<CardInfoData> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                Cards.Remove(cards[i]);
                CardRemoved?.Invoke(cards[i]);
            }
        }
    }
}