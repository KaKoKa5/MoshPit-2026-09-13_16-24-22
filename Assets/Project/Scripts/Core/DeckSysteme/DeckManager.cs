using System.Collections.Generic;
using GamePlay.Card;
using UnityEngine;

namespace Core.DeckSysteme
{
	public class DeckManager
	{
		[field: SerializeField] public List<CardInfoData> MasterDeck { get;private  set;} =  new List<CardInfoData>();
		
		[field: SerializeField] public List<CardInfoData> Deck { get;private set;}  =  new List<CardInfoData>();
		[field: SerializeField] public List<CardInfoData> Hands { get;private  set;}  =  new List<CardInfoData>();
		[field: SerializeField] public List<CardInfoData> Discard { get;private  set;}   =  new List<CardInfoData>();
		
		
	}
}