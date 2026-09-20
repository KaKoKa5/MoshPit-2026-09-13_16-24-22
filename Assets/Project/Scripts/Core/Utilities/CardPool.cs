using System.Collections.Generic;
using Core.DeckSysteme;
using GamePlay.Card;
using UnityEngine;

namespace Core.Utilities
{
	public class CardPool : Singleton<CardPool>
	{
		[SerializeField] private CardView cardViewPrefab;
		[SerializeField] private Transform poolParent;

		private readonly Dictionary<CardInfoData, CardView> activeByData = new Dictionary<CardInfoData, CardView>(10);

		public void Bind(CardInfoData data, CardView view) => activeByData[data] = view;

		private readonly Stack<CardView> availableCards = new Stack<CardView>();

		protected override void Awake()
		{
			base.Awake();
			Prewarm(GameMetrix.InitialPoolSize);
		}

		private void Prewarm(int count)
		{
			for (int i = 0; i < count; i++)
			{
				CardView newCard = Instantiate(cardViewPrefab, poolParent);
				newCard.gameObject.SetActive(false);
				availableCards.Push(newCard);
			}
		}

		public CardView Get(Transform parent)
		{
			CardView card = availableCards.Count > 0
				? availableCards.Pop()
				: Instantiate(cardViewPrefab, poolParent);

			card.transform.SetParent(parent, false);
			card.gameObject.SetActive(true);
			return card;
		}

		public void Release(CardView card)
		{
			card.gameObject.SetActive(false);
			card.transform.SetParent(poolParent, false);
			availableCards.Push(card);
		}

		public void ReleaseByData(CardInfoData data)
		{
			if (activeByData.TryGetValue(data, out CardView view))
			{
				Release(view);
				activeByData.Remove(data);
			}
		}
	}
}