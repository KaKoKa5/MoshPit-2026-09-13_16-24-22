using System.Collections.Generic;
using Core.Utilities;
using GamePlay.Card;
using UnityEngine;

namespace Core.DeckSysteme
{
	public class DeckManager : Singleton<DeckManager>
	{
		[field: SerializeField] public List<CardInfoData> MasterDeck { get; private set; } = new List<CardInfoData>(GameMetrix.Instance.StartDeckCard);
		[field: SerializeField] public Deck Deck { get; private set; }

		protected override void Awake()
		{
			base.Awake();
			Deck = new Deck(MasterDeck);
		}
	}
}