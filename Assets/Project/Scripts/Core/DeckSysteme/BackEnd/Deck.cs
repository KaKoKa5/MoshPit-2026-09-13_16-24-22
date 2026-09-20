using System;
using System.Collections.Generic;
using GamePlay.Card;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.DeckSysteme.BackEnd
{
	public class Deck
	{
		public List<CardInfoData> Cards { get; } = new List<CardInfoData>();
		private List<CardInfoData> Discard { get; } = new List<CardInfoData>();
		public event Action DeckChanged;
		public Deck(List<CardInfoData> masterDeck)
		{
			Cards.AddRange(masterDeck);
			Shuffle();
		}

		public void Shuffle()
		{
			for (int i = Cards.Count - 1; i > 0; i--)
			{
				int j = Random.Range(0, i + 1);
				(Cards[i], Cards[j]) = (Cards[j], Cards[i]);
			}
			DeckChanged?.Invoke();
		}

		public CardInfoData DrawTopCard()
		{
			if (Cards.Count == 0)
				RecycleDiscard();

			if (Cards.Count == 0)
				return null;

			CardInfoData topCard = Cards[0];
			Cards.RemoveAt(0);
			DeckChanged?.Invoke();
			Debug.Log(Cards.Count);
			return topCard;
		}

		public void AddToDiscard(List<CardInfoData> cards)
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