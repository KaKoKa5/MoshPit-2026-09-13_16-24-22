using System;
using System.Collections.Generic;
using Core.DeckSysteme.Cards;

namespace Core.DeckSysteme
{
	public class Field
	{
		public List<CardInstance> PlayedCards { get; } = new List<CardInstance>(4);

		public event Action<CardInstance> CardAdded;
		public event Action ClearedField;

		public void PlayCards(List<CardInstance> orderedCards)
		{
			if (orderedCards == null)
				return;

			for (int i = 0; i < orderedCards.Count; i++)
			{
				PlayedCards.Add(orderedCards[i]);
				CardAdded?.Invoke(orderedCards[i]);
			}
		}

		public void Clear()
		{
			PlayedCards.Clear();
			ClearedField?.Invoke();
		}
	}
}