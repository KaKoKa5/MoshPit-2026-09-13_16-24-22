using PrimeTween;
using UnityEngine;

namespace Core.DeckSysteme
{
	public class CardAnimation : MonoBehaviour
	{
		[Header("References")]
		[SerializeField] private CardInteraction cardInteraction;
		
		[Header("Animation Settings")]
		[Range(0f, 2f)]
		[SerializeField] private float timeIn;
		[Range(0f, 2f)]
		[SerializeField] private float timeout;
		[Range(0f, 50f)]
		[SerializeField] private float shake;
		[Range(0f, 50f)]
		[SerializeField] private float endDragShake;
		[Range(0f, 30f)]
		[SerializeField] private int frequency;
		[Range(0f, 30f)]
		[SerializeField] private int frequencyEndDrag;
		[SerializeField] private Vector3 enterScale;
		[SerializeField] private Vector3 selectedScale;
		
		private bool isSelected = false;
		
		private Tween scaleTween;
		private Tween rotationTween;
		private Tween positionTween;
		

		private void Awake()
		{
			cardInteraction =  GetComponent<CardInteraction>();
			if (cardInteraction == null)
				return;
		}

		private void OnEnable()
		{
			cardInteraction.EndDrag+= EndDragTween;
			cardInteraction.BeginDrag+= BeginDragTween;
			cardInteraction.CardDeselect+= CardDeselectTween;
			cardInteraction.CardSelect+= CardSelectTween;
			cardInteraction.PointerExit+= PointerExitTween;
			cardInteraction.PointerEnter+= PointerEnterTween;
			cardInteraction.SlotSnap += SlotSnapTween;
			cardInteraction.DrawFromDeck += DrawFromDeckTween;
		}

		private void SlotSnapTween(Transform cardAnim, float targetY)
		{
			Tween.UIAnchoredPosition((RectTransform)cardAnim, new Vector2(0, targetY), 0.25f, Ease.OutBack);
		}

		private void OnDisable()
		{
			cardInteraction.EndDrag -= EndDragTween;
			cardInteraction.BeginDrag -= BeginDragTween;
			cardInteraction.CardDeselect -= CardDeselectTween;
			cardInteraction.CardSelect -= CardSelectTween;
			cardInteraction.PointerEnter -= PointerEnterTween;
			cardInteraction.PointerExit-= PointerExitTween;
			cardInteraction.SlotSnap -= SlotSnapTween;
			cardInteraction.DrawFromDeck -= DrawFromDeckTween;
			StopAllOwnedTweens();
		}

		private void StopAllOwnedTweens()
		{
			if (scaleTween.isAlive) scaleTween.Stop();
			if (rotationTween.isAlive) rotationTween.Stop();
			if (positionTween.isAlive) positionTween.Stop();
		}

		private void DrawFromDeckTween(Transform cardAnim)
		{
			RectTransform rt = (RectTransform)cardAnim;

			// Petite rotation de départ aléatoire + léger scale-in, pour un effet "pop" plus vivant qu'un simple slide
			rt.localRotation = Quaternion.Euler(0, 0, Random.Range(-12f, 12f));
			rt.localScale = Vector3.one * 0.7f;

			Tween.UIAnchoredPosition(rt, Vector2.zero, 0.15f, Ease.InExpo);
			Tween.Rotation(rt, Quaternion.identity, 0.3f, Ease.OutSine);
			Tween.Scale(rt, Vector3.one, 0.3f, Ease.OutBack);
		}

		private void PointerEnterTween(Transform cardAnim)
		{
			if (isSelected)
				return;
			if (rotationTween.isAlive) rotationTween.Stop();
			if (scaleTween.isAlive) scaleTween.Stop();
			
			rotationTween = Tween.ShakeLocalRotation(cardAnim, new Vector3(0f, 0f, shake), timeIn, frequency);
			scaleTween = Tween.Scale(cardAnim, enterScale, timeIn);
			
		}

		private void PointerExitTween(Transform cardAnim)
		{
			if(isSelected)
				return;
			if (rotationTween.isAlive) rotationTween.Stop();
			if (scaleTween.isAlive) scaleTween.Stop();
			
			cardAnim.localRotation = Quaternion.Euler(0, 0, 0);
			scaleTween = Tween.Scale(cardAnim, Vector3.one, timeout);
		}

		private void CardSelectTween(Transform cardAnim)
		{
			isSelected = true;
			Tween.Scale(cardAnim, selectedScale, timeIn);
		}

		private void CardDeselectTween(Transform cardAnim)
		{
			isSelected = false;
			Tween.Scale(cardAnim, new Vector3(1,1, 1), timeout);
			Tween.UIAnchoredPositionY((RectTransform)cardAnim, 0, timeout);
		}

		private void BeginDragTween(Transform cardAnim)
		{
		}

		private void EndDragTween(Transform cardAnim)
		{
			Tween.ShakeLocalRotation(cardAnim,  new Vector3(0f, 0f, shake),timeout,frequencyEndDrag);
		}
	}
}
