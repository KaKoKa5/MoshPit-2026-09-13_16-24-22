using System.Collections.Generic;
using Core.Utilities;
using GamePlay.Card;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.DeckSysteme
{
	public class DeckManager : Singleton<DeckManager>
	{
		
		[SerializeField] private List<CardInfoData> masterDeck = new List<CardInfoData>();

		public List<CardInfoData> MasterDeck => masterDeck;
		public Deck Deck { get; private set; }

		protected override void Awake()
		{
			base.Awake();
			Deck = new Deck(MasterDeck);
		}
	}
}