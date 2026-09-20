using Core.DeckSysteme.BackEnd;
using Core.Utilities;
using TMPro; 
using UnityEngine;
using UnityEngine.UI;

namespace Core.DeckSysteme
{
	public class DeckView : MonoBehaviour
	{
		[SerializeField] private Image cardBackImage; 
		[SerializeField] private TMP_Text countText;  

		private Deck deck;

		private void Start()
		{
			deck = DeckManager.Instance.CardsDeck;
			deck.DeckChanged += Refresh;

			Refresh(); // état initial
		}

		private void OnDestroy()
		{
			if (deck != null)
				deck.DeckChanged -= Refresh;
		}

		private void Refresh()
		{
			int remaining = deck.Cards.Count;

			countText.text = remaining.ToString() + "/" + GameMetrix.Instance.InitialPoolSize;
			cardBackImage.enabled = remaining > 0; 
		}
	}
}