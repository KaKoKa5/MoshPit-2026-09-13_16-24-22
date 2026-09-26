using System.Collections.Generic;
using Core.DeckSysteme.Cards;
using Core.Utilities;
using UnityEngine;

namespace Core.DeckSysteme
{
	public class PlayedField : MonoBehaviour
	{
		[SerializeField] private RectTransform[] fieldSlots;
		public Field Field { get; private set; }

		private List<CardView> activeViews;

		private void Awake()
		{
			int capacity = GameMetrix.Instance != null ? GameMetrix.Instance.MaxSelectable : 4;
			activeViews = new List<CardView>(capacity);

			Field = new Field();
			Field.CardAdded += OnCardAdded;
			Field.ClearedField += OnClearedField;
		}
		private void OnDestroy()
		{
			Field.CardAdded -= OnCardAdded;
			Field.ClearedField -= OnClearedField;
		}

		private void OnCardAdded(CardInstance card, int slotIndex)
		{

			RectTransform slot = fieldSlots[slotIndex];
			CardView view = CardPool.Instance.Get(slot);
			
			view.Setup(card);
			CardPool.Instance.Bind(card, view);
			activeViews.Add(view);

			RectTransform viewRect = (RectTransform)view.transform;
			viewRect.anchoredPosition = Vector2.zero;

			CardInteraction interaction = view.GetComponent<CardInteraction>();
			
			interaction.PlayDrawAnimation();
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