using System;
using System.Collections.Generic;
using GamePlay.Card;
using UnityEngine;

namespace Core.DeckSysteme
{
	public class BoardField : MonoBehaviour
	{
		[field: SerializeField] public List<CardInfoData> FieldCards { get; private set; } = new List<CardInfoData>(4);

		public event Action OnFieldUpdated;
		public event Action OnFieldCleared;
		
		public void PlaceCardsOnField(List<CardInfoData> cardsToPlay)
		{
			if (cardsToPlay == null || cardsToPlay.Count == 0) return;

			FieldCards.Clear();
			FieldCards.AddRange(cardsToPlay);
            
			OnFieldUpdated?.Invoke();
		}
		
		public IReadOnlyList<CardInfoData> GetCardsForResolution()
		{
			return FieldCards;
		}
		
		public void ClearAndDiscardField()
		{
			if (FieldCards.Count == 0) return;

			
			DeckManager.Instance.DiscardCards(FieldCards);
            
			FieldCards.Clear();
			OnFieldCleared?.Invoke();

			Debug.Log("[BoardField] Phase 5 : Terrain nettoyé et cartes envoyées à la défausse.");
		}
	}
}