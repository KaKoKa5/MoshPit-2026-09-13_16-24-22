using System.Collections.Generic;
using Core.DeckSysteme;
using Core.DeckSysteme.Cards;
using UnityEngine;

namespace Core.Utilities
{
	public class CardPool : Singleton<CardPool>
	{
		[SerializeField] private CardView cardViewPrefab;
		[SerializeField] private Transform poolParent;

		private readonly Dictionary<CardInstance, CardView> activeByInstance = new Dictionary<CardInstance, CardView>(10);

		public void Bind(CardInstance instance, CardView view) => activeByInstance[instance] = view;

		private readonly Stack<CardView> availableCards = new Stack<CardView>();

		protected override void Awake()
		{
			base.Awake();
			Prewarm(GameMetrix.Instance.InitialPoolSize);
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

		public void ReleaseByInstance(CardInstance instance)
		{
			if (activeByInstance.TryGetValue(instance, out CardView view))
			{
				Release(view);
				activeByInstance.Remove(instance);
			}
		}
	}
}