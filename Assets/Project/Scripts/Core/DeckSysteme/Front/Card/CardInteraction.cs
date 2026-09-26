using System;
using Core.DeckSysteme.Cards;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.DeckSysteme
{
    public class CardInteraction : MonoBehaviour,
        IPointerDownHandler, IPointerUpHandler,
        IPointerExitHandler, IPointerEnterHandler,
        IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] private CardView cardView;
        [SerializeField] private HandCards handCards;
        [SerializeField] private Selectable selectableUI;

        [Header("Tap vs Drag")]
        [SerializeField] private float tapMaxDuration = 0.2f;

        [Header("Sélection visuelle")]
        [SerializeField] private float selectionOffset = 50f;

        private float pointerDownTime;
        private bool wasDragged;
        private bool isSelected;

        private RectTransform rectTransform;
        private Vector2 originalAnchoredPosition;
        private Vector2 dragOffset;
        
        public event Action<Transform> PointerEnter;
        public event Action<Transform> PointerExit;
        public event Action<Transform> BeginDrag;
        public event Action<Transform> EndDrag;
        public event Action<Transform> CardSelect;
        public event Action<Transform> CardDeselect;
        

        private void Awake()
        {
            rectTransform = (RectTransform)transform;
        }

        public void Setup(CardView view, HandCards owner, Vector2 originalPosition)
        {
            if (handCards != null)
            {
                handCards.Hand.CardSelected -= OnHandCardSelected;
                handCards.Hand.CardDeselected -= OnHandCardDeselected;
            }

            cardView = view;
            handCards = owner;
            originalAnchoredPosition = originalPosition;
            isSelected = false;
            wasDragged = false;

            handCards.Hand.CardSelected += OnHandCardSelected;
            handCards.Hand.CardDeselected += OnHandCardDeselected;
        }

        private void OnDisable()
        {
            if (handCards != null)
            {
                handCards.Hand.CardSelected -= OnHandCardSelected;
                handCards.Hand.CardDeselected -= OnHandCardDeselected;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
	        PointerEnter?.Invoke(transform);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
	        PointerExit?.Invoke(transform);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            pointerDownTime = Time.time;
            wasDragged = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            float heldDuration = Time.time - pointerDownTime;

            if (heldDuration > tapMaxDuration || wasDragged)
                return;

            if (isSelected)
                handCards.DeselectCard(cardView.Instance);
            else
                handCards.SelectCard(cardView.Instance);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
	        BeginDrag?.Invoke(transform);
            wasDragged = true;
            dragOffset = rectTransform.anchoredPosition - eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            rectTransform.anchoredPosition = eventData.position + dragOffset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
	        EndDrag?.Invoke(transform);
            rectTransform.anchoredPosition = isSelected
                ? originalAnchoredPosition + Vector2.up * selectionOffset
                : originalAnchoredPosition;
        }

        private void OnHandCardSelected(CardInstance card)
        {
            if (card != cardView.Instance)
                return;
            rectTransform.anchoredPosition = originalAnchoredPosition + Vector2.up * selectionOffset;
            isSelected = true;
            CardSelect?.Invoke(transform);
        }

        private void OnHandCardDeselected(CardInstance card)
        {
            if (card != cardView.Instance)
                return;
            rectTransform.anchoredPosition = originalAnchoredPosition;
            isSelected = false;
            CardDeselect?.Invoke(transform);
        }
    }
}