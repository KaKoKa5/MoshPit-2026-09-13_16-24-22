using System.Collections.Generic;
using Core.DeckSysteme.BackEnd;
using Core.Utilities;
using GamePlay.Card;
using UnityEngine;

namespace Core.DeckSysteme
{
	public class HandCards : MonoBehaviour
	{
		[SerializeField] private Transform handContainerTransform;
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

		private void OnCardDrawn(CardInfoData card)
		{
			CardView view = CardPool.Instance.Get(handContainerTransform);
			view.Setup(card);
			CardPool.Instance.Bind(card, view);
		}

		private void OnCardRemoved(CardInfoData card)
		{
			CardPool.Instance.ReleaseByData(card);
		}
		
		public void SelectCard(CardInfoData card) => Hand.TrySelectCard(card);
		public void DeselectCard(CardInfoData card) => Hand.TryDeselectCard(card);

		public void PlaySelection()
		{
			List<CardInfoData> orderedSelection = Hand.ConfirmSelectionToField();

			if (orderedSelection != null)
			{
				playedField.PlayCards(orderedSelection);
			}
		}

		public void DiscardSelection() => Hand.DiscardSelection();
	}
}