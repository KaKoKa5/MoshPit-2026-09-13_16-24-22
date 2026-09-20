using System.Collections.Generic;
using Core.Utilities;
using GamePlay.Card;
using UnityEngine;

namespace Core.DeckSysteme
{
	public class PlayedField : MonoBehaviour
	{
		[SerializeField] private Transform fieldContainerTransform;

		public Field Field { get; private set; }

		// Le champ garde ses propres vues actives, séparées de celles de la main,
		// pour pouvoir toutes les libérer d'un coup sur ClearedField.
		private readonly List<CardView> activeViews = new List<CardView>(4);

		private void Awake()
		{
			Field = new Field();
			Field.CardAdded += OnCardAdded;
			Field.ClearedField += OnClearedField;
		}

		private void OnDestroy()
		{
			Field.CardAdded -= OnCardAdded;
			Field.ClearedField -= OnClearedField;
		}

		private void OnCardAdded(CardInfoData card)
		{
			CardView view = CardPool.Instance.Get(fieldContainerTransform);
			view.Setup(card);
			CardPool.Instance.Bind(card, view);
			activeViews.Add(view);
		}

		private void OnClearedField()
		{
			for (int i = 0; i < activeViews.Count; i++)
			{
				CardPool.Instance.Release(activeViews[i]);
			}

			activeViews.Clear();
		}

		public void PlayCards(List<CardInfoData> orderedCards)
		{
			Field.PlayCards(orderedCards);
		}
	}
}