using Core.DeckSysteme.Cards;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Core.DeckSysteme
{
	public class CardView : MonoBehaviour
	{
		[SerializeField] private Image artworkImage;
		[SerializeField] private TMP_Text valueText;

		public CardInstance Instance { get; private set; }

		public void Setup(CardInstance instance)
		{
			Instance = instance;
			artworkImage.sprite = instance.Data.cardIcon;
			valueText.text = instance.Data.baseValue.ToString();
		}
	}
}