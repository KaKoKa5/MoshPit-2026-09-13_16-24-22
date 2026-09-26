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

        public int CurrentSlotIndex { get; private set; }
        public CardView ViewRef => cardView;

        private RectTransform rectTransform;
        private Vector2 dragOffset;

        public event Action<Transform> PointerEnter;
        public event Action<Transform> PointerExit;
        public event Action<Transform> BeginDrag;
        public event Action<Transform> EndDrag;
        public event Action<Transform> CardSelect;
        public event Action<Transform> CardDeselect;
        public event Action<Transform, float> SlotSnap; 
        public event Action<Transform> DrawFromDeck;

        private void Awake()
        {
            rectTransform = (RectTransform)transform;
        }

        public void Setup(CardView view, HandCards owner, int slotIndex)
        {
            if (handCards != null)
            {
                handCards.Hand.CardSelected -= OnHandCardSelected;
                handCards.Hand.CardDeselected -= OnHandCardDeselected;
            }

            cardView = view;
            handCards = owner;
            CurrentSlotIndex = slotIndex;
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

        public void SetSlotIndex(int index) => CurrentSlotIndex = index;

        /// <summary>
        /// Demande le retour (animé, via CardAnimation) à la position de repos du slot actuel,
        /// en tenant compte de l'état sélectionné. Ne déplace RIEN directement ici.
        /// </summary>
        public void SnapHome()
        {
            SlotSnap?.Invoke(transform, isSelected ? selectionOffset : 0f);
        }

        public void OnPointerEnter(PointerEventData eventData) => PointerEnter?.Invoke(transform);
        public void OnPointerExit(PointerEventData eventData) => PointerExit?.Invoke(transform);

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

            // C'est HandCards qui décide où la carte doit finir (réorganisation ou retour chez elle),
            // et qui appellera SnapHome() en conséquence sur la ou les cartes concernées.
            handCards.TryReorder(this, eventData.position);
        }

        private void OnHandCardSelected(CardInstance card)
        {
            if (card != cardView.Instance)
                return;

            isSelected = true;
            CardSelect?.Invoke(transform);
            SnapHome(); // anime vers la nouvelle position (avec l'offset de sélection)
        }

        private void OnHandCardDeselected(CardInstance card)
        {
            if (card != cardView.Instance)
                return;

            isSelected = false;
            CardDeselect?.Invoke(transform);
            SnapHome(); // anime vers la position normale
        }
        public void PlayDrawAnimation(float delay = 0f)
        {
	        if (delay <= 0f)
	        {
		        DrawFromDeck?.Invoke(transform);
	        }
	        else
	        {
		        Tween.Delay(delay, () => DrawFromDeck?.Invoke(transform));
	        }
        }
    }
}