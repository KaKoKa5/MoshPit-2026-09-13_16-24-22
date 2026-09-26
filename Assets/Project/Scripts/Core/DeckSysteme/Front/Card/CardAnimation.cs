using PrimeTween;
using UnityEngine;

namespace Core.DeckSysteme
{
	public class CardAnimation : MonoBehaviour
	{
		[SerializeField] private CardInteraction cardInteraction;

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
		}

		private void OnDisable()
		{
			cardInteraction.EndDrag -= EndDragTween;
			cardInteraction.BeginDrag -= BeginDragTween;
			cardInteraction.CardDeselect -= CardDeselectTween;
			cardInteraction.CardSelect -= CardSelectTween;
			cardInteraction.PointerEnter -= PointerEnterTween;
			cardInteraction.PointerExit-= PointerExitTween;
		}

		private void PointerEnterTween(Transform cardAnim)
		{
			Tween.ShakeLocalRotation(cardAnim,  new Vector3(0f, 0f, 15f),0.2f,15f);
			Tween.Scale(cardAnim, new Vector3(1.2f,1.2f, 1.2f), 0.2f);
		}

		private void PointerExitTween(Transform cardAnim)
		{
			Tween.Scale(cardAnim, new Vector3(1,1, 1), 0.1f);
		}

		private void CardSelectTween(Transform cardAnim)
		{
			RectTransform rt = (RectTransform)cardAnim;
			Tween.Scale(cardAnim, new Vector3(1.5f, 1.5f, 1.5f), 0.2f);
			Tween.UIAnchoredPositionY(rt, rt.anchoredPosition.y + 2.5f, 0.2f);
		}

		private void CardDeselectTween(Transform cardAnim)
		{
			Tween.Scale(cardAnim, new Vector3(1,1, 1), 0.1f);
			Tween.UIAnchoredPositionY((RectTransform)cardAnim, 0, 0.2f);
		}

		private void BeginDragTween(Transform cardAnim)
		{
		}

		private void EndDragTween(Transform cardAnim)
		{
			Tween.ShakeLocalRotation(cardAnim,  new Vector3(0f, 0f, 15f),0.2f,25f);
		}
	}
}
