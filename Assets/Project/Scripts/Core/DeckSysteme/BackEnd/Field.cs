using System;
using System.Collections.Generic;
using Core.DeckSysteme.Cards;
using Core.Utilities;

namespace Core.DeckSysteme
{
	public class Field
	{
		public List<CardInstance> PlayedCards { get; } = new List<CardInstance>(GameMetrix.Instance.MaxSelectable);

		public event Action<CardInstance,int> CardAdded;
		public event Action ClearedField;

		public void PlayCards(List<CardInstance> orderedCards)
		{
			if (orderedCards == null)
				return;

			for (int i = 0; i < orderedCards.Count; i++)
			{
				PlayedCards.Add(orderedCards[i]);
				CardAdded?.Invoke(orderedCards[i], PlayedCards.Count - 1);
			}
		}

		public void Clear()
		{
			PlayedCards.Clear();
			ClearedField?.Invoke();
		}
	}
}