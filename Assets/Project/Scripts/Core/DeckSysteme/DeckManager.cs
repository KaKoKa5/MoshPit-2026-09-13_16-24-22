using System;
using System.Collections.Generic;
using Core.Utilities;
using GamePlay.Card;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.DeckSysteme
{
	public class DeckManager : Singleton<DeckManager>
	{
		[field: SerializeField] public List<CardInfoData> MasterDeck { get;private  set;} =  new List<CardInfoData>();
		[field: SerializeField] public List<CardInfoData> Deck { get;private set;}  =  new List<CardInfoData>();
		[field: SerializeField] public List<CardInfoData> Discard { get;private  set;}   =  new List<CardInfoData>();
		
		public event Action OnDeckUpdated;
		
		private void InitDeck()
		{
			Deck.Clear();
			Deck.AddRange(MasterDeck);
			ShuffleDeck();
		}

		private void Start()
		{
			InitDeck();
		}

		public void ShuffleDeck()
		{
			for (int i = Deck.Count - 1; i > 0; i--)
			{
				int j = Random.Range(0, i + 1);
				(Deck[i], Deck[j]) = (Deck[j], Deck[i]);
			}
		}
		
		public CardInfoData DrawTopCard()
		{
			if (Deck.Count == 0)
				return null;

			CardInfoData topCard = Deck[0];
			Deck.RemoveAt(0);
			return topCard;
		}
		
		public void DiscardCards(List<CardInfoData> cards)
		{
			if (cards == null || cards.Count == 0) return;
            
			Discard.AddRange(cards);
			OnDeckUpdated?.Invoke();
		}
		
		
		public void AddCardToMasterDeck(CardInfoData card)
		{
			if (card == null) return;
			MasterDeck.Add(card);
			OnDeckUpdated?.Invoke();
		}
		
		public bool RemoveCardFromMasterDeck(CardInfoData card)
		{
			bool removed = MasterDeck.Remove(card);
			if (removed)
			{
				OnDeckUpdated?.Invoke();
			}
			return removed;
		}
	}
}