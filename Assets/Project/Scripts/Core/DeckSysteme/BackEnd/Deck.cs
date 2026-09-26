using System;
using System.Collections.Generic;
using Core.DeckSysteme.Cards;
using GamePlay.Card;
using UnityEngine;

namespace Core.DeckSysteme
{
	public class Deck
	{
		public List<CardInstance> Cards { get; } = new List<CardInstance>();
		public List<CardInstance> Discard { get; } = new List<CardInstance>();

		public event Action DeckChanged;

		public Deck(List<CardInfoData> masterDeck)
		{
			foreach (CardInfoData data in masterDeck)
			{
				Cards.Add(new CardInstance(data));
			}

			Shuffle();
		}

		public void Shuffle()
		{
			for (int i = Cards.Count - 1; i > 0; i--)
			{
				int j = UnityEngine.Random.Range(0, i + 1);
				(Cards[i], Cards[j]) = (Cards[j], Cards[i]);
			}

			DeckChanged?.Invoke();
		}

		public CardInstance DrawTopCard()
		{
			if (Cards.Count == 0)
				RecycleDiscard();

			if (Cards.Count == 0)
				return null;

			CardInstance topCard = Cards[0];
			Cards.RemoveAt(0);
			DeckChanged?.Invoke();
			return topCard;
		}

		public void AddToDeck(CardInstance card)
		{
			Cards.Add(card);
		}

		public void RemoveFromDeck(CardInstance card)
		{
			Cards.Remove(card);
		}

		public void AddToDiscard(List<CardInstance> cards)
		{
			Discard.AddRange(cards);
			DeckChanged?.Invoke();
		}

		private void RecycleDiscard()
		{
			if (Discard.Count == 0)
				return;

			Cards.AddRange(Discard);
			Discard.Clear();
			Shuffle();
		}
	}
}