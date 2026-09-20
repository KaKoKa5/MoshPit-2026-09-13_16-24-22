using System;
using System.Collections.Generic;
using GamePlay.Card;

namespace Core.DeckSysteme
{
	public class Field
	{
		public List<CardInfoData> PlayedCards { get; } = new List<CardInfoData>(4);

		public event Action<CardInfoData> CardAdded;
		public event Action ClearedField;

		/// <summary>
		/// Pose les cartes reçues dans l'ordre exact où elles arrivent
		/// (Hand garantit déjà cet ordre via ConfirmSelectionToField).
		/// </summary>
		public void PlayCards(List<CardInfoData> orderedCards)
		{
			if (orderedCards == null)
				return;

			for (int i = 0; i < orderedCards.Count; i++)
			{
				PlayedCards.Add(orderedCards[i]);
				CardAdded?.Invoke(orderedCards[i]);
			}
		}

		/// <summary>
		/// Vide le terrain, typiquement une fois la manche résolue (score calculé, etc.).
		/// </summary>
		public void Clear()
		{
			PlayedCards.Clear();
			ClearedField?.Invoke();
		}
	}
}