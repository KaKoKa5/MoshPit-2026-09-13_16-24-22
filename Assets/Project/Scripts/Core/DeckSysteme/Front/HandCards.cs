using System.Collections.Generic;
using Core.DeckSysteme.Cards;
using Core.Utilities;
using UnityEngine;

namespace Core.DeckSysteme
{
	public class HandCards : MonoBehaviour
	{
		[SerializeField] private RectTransform[] handSlots;
		[SerializeField] private PlayedField playedField;

		public Hand Hand { get; private set; }

		private void Start()
		{
			Hand = new Hand(DeckManager.Instance.CardsDeck);

			Hand.CardDrawn += OnCardDrawn;
			Hand.CardRemoved += OnCardRemoved;

			Hand.FillHand();
		}

		private void OnDestroy()
		{
			Hand.CardDrawn -= OnCardDrawn;
			Hand.CardRemoved -= OnCardRemoved;
		}

		private void OnCardDrawn(CardInstance card, int slotIndex)
		{
			RectTransform slot = handSlots[slotIndex];

			CardView view = CardPool.Instance.Get(slot);
			view.Setup(card);
			CardPool.Instance.Bind(card, view);

			RectTransform viewRect = (RectTransform)view.transform;
			viewRect.anchoredPosition = Vector2.zero;

			CardInteraction interaction = view.GetComponent<CardInteraction>();
			interaction.Setup(view, this, Vector2.zero);
		}

		private void OnCardRemoved(CardInstance card)
		{
			CardPool.Instance.ReleaseByInstance(card);
		}

		public void SelectCard(CardInstance card) => Hand.TrySelectCard(card);
		public void DeselectCard(CardInstance card) => Hand.TryDeselectCard(card);

		public void PlaySelection()
		{
			List<CardInstance> orderedSelection = Hand.ConfirmSelectionToField();

			if (orderedSelection != null)
			{
				playedField.PlayCards(orderedSelection);
			}
		}

		public void DiscardSelection() => Hand.DiscardSelection();
	}
}