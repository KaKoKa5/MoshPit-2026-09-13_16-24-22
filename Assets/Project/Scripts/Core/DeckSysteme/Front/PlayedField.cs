using System.Collections.Generic;
using Core.DeckSysteme.Cards;
using Core.Utilities;
using UnityEngine;

namespace Core.DeckSysteme
{
	public class PlayedField : MonoBehaviour
	{
		[SerializeField] private Transform fieldContainerTransform;

		public Field Field { get; private set; }

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

		private void OnCardAdded(CardInstance card)
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

		public void PlayCards(List<CardInstance> orderedCards)
		{
			Field.PlayCards(orderedCards);
		}
	}
}