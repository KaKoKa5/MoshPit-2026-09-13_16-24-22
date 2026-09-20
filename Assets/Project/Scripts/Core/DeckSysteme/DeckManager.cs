using System.Collections.Generic;
using Core.DeckSysteme.BackEnd;
using Core.Utilities;
using GamePlay.Card;
using UnityEngine;

namespace Core.DeckSysteme
{
	public class DeckManager : Singleton<DeckManager>
	{
		[field: SerializeField] public List<CardInfoData> MasterDeck { get; private set; } = new List<CardInfoData>();
		[field: SerializeField] public Deck CardsDeck { get; private set; }

		protected override void Awake()
		{
			base.Awake();
			CardsDeck = new Deck(MasterDeck);
		}
	}
}