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
        [SerializeField] private float maxSwapDistance = 500f;
        [SerializeField] private RectTransform deckOrigin;

        private CardView[] slotViews;

        public Hand Hand { get; private set; }

        private void Awake()
        {
            slotViews = new CardView[handSlots.Length];
        }

        private void Start()
        {
            Hand = new Hand(DeckManager.Instance.Deck);

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
	        viewRect.anchoredPosition = GetLocalPositionInSlot(deckOrigin, slot); 

	        slotViews[slotIndex] = view;

	        CardInteraction interaction = view.GetComponent<CardInteraction>();
	        interaction.Setup(view, this, slotIndex);

	        interaction.PlayDrawAnimation(delay: slotIndex * 0.06f);
        }

        private void OnCardRemoved(CardInstance card)
        {
            for (int i = 0; i < slotViews.Length; i++)
            {
                if (slotViews[i] != null && slotViews[i].Instance == card)
                {
                    slotViews[i] = null;
                    break;
                }
            }

            CardPool.Instance.ReleaseByInstance(card);
        }

        public int GetNearestSlotIndex(Vector2 screenPosition)
        {
            int nearestIndex = 0;
            float minDistance = float.MaxValue;

            for (int i = 0; i < handSlots.Length; i++)
            {
                Vector2 slotScreenPos = RectTransformUtility.WorldToScreenPoint(null, handSlots[i].position);
                float distance = Vector2.Distance(slotScreenPos, screenPosition);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestIndex = i;
                }
            }

            return nearestIndex;
        }

        public void TryReorder(CardInteraction dragged, Vector2 dropScreenPosition)
        {
	        if (dragged == null)
		        return;
            int targetIndex = GetNearestSlotIndex(dropScreenPosition);
            int sourceIndex = dragged.CurrentSlotIndex;

            Vector2 targetSlotScreenPos = RectTransformUtility.WorldToScreenPoint(null, handSlots[targetIndex].position);
            float distanceToTarget = Vector2.Distance(targetSlotScreenPos, dropScreenPosition);

            if (targetIndex == sourceIndex || distanceToTarget > maxSwapDistance)
            {
                dragged.SnapHome();
                return;
            }

            CardView draggedView = slotViews[sourceIndex];

            if (sourceIndex < targetIndex)
            {
                for (int i = sourceIndex; i < targetIndex; i++)
                {
                    slotViews[i] = slotViews[i + 1];
                    MoveViewToSlot(slotViews[i], i);
                }
            }
            else
            {
                for (int i = sourceIndex; i > targetIndex; i--)
                {
                    slotViews[i] = slotViews[i - 1];
                    MoveViewToSlot(slotViews[i], i);
                }
            }

            slotViews[targetIndex] = draggedView;
            MoveViewToSlot(draggedView, targetIndex);
        }

        private void MoveViewToSlot(CardView view, int slotIndex)
        {
	        if (view == null)
		        return;
            RectTransform slot = handSlots[slotIndex];

            view.transform.SetParent(slot, true); // worldPositionStays: true, évite un saut visuel

            CardInteraction interaction = view.GetComponent<CardInteraction>();
            interaction.SetSlotIndex(slotIndex);
            interaction.SnapHome();
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
        
        private Vector2 GetLocalPositionInSlot(RectTransform source, RectTransform targetParent)
        {
	        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, source.position);
	        RectTransformUtility.ScreenPointToLocalPointInRectangle(targetParent, screenPos, null, out Vector2 localPos);
	        return localPos;
        }
    }
}