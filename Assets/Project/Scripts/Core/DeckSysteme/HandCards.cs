using System;
using System.Collections.Generic;
using Core.Utilities;
using GamePlay.Card;
using UnityEngine;

namespace Core.DeckSysteme
{
    public class HandCards : MonoBehaviour
    {
        [field: SerializeField] public List<CardInfoData> Cards { get; private set; } = new List<CardInfoData>(6);
        [field: SerializeField] public List<CardInfoData> SelectedCards { get; private set; } = new List<CardInfoData>(4);
        [field: SerializeField] public List<CardInfoData> FieldCards { get; private set; } = new List<CardInfoData>(4);
        private readonly List<CardInfoData> selectionBuffer = new List<CardInfoData>(4);
        public event Action HandReady;
        public event Action<CardInfoData> CardSelected;
        public event Action<CardInfoData> CardDeselected;
        public event Action OnCardsPlayedToField;

        public void TakeCards()
        {
            Cards.Clear(); 

            for (int i = 0; i < GameMetrix.MaxCardOnHand; i++)
            {
                CardInfoData drawnCard = DeckManager.Instance.DrawTopCard();

                if (drawnCard == null)
                {
                    Debug.LogWarning("Deck vide, impossible de compléter la main.");
                    break;
                }

                Cards.Add(drawnCard);
            }

            HandReady?.Invoke();
        }

        public bool TrySelectCard(CardInfoData card)
        {
            if (SelectedCards.Count >= GameMetrix.MaxSelectable)
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
            return count >= GameMetrix.MinSelectable && count <= GameMetrix.MaxSelectable;
        }
        
        public List<CardInfoData> PlaySelectedCardsToField()
        {
            if (!CanConfirmSelection())
                return null;

            selectionBuffer.Clear();
            selectionBuffer.AddRange(SelectedCards);
            
            for (int i = 0; i < selectionBuffer.Count; i++)
            {
                Cards.Remove(selectionBuffer[i]);
            }

            SelectedCards.Clear();
            OnCardsPlayedToField?.Invoke();
            return selectionBuffer;
        }
        
    }
}