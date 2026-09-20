using GamePlay.Card;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.DeckSysteme
{
	public class CardView : MonoBehaviour
	{
		[SerializeField] private Image artworkImage;
		[SerializeField] private TMP_Text valueText;

		public CardInfoData Data { get; private set; }

		public void Setup(CardInfoData data)
		{
			Data = data;
			artworkImage.sprite = data.cardIcon;
			valueText.text = data.baseValue.ToString();
		}
	}
}