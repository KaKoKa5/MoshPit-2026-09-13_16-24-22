using System;
using GamePlay.Card;

namespace Core.DeckSysteme.Cards
{
	public class CardInstance
	{
		public CardInfoData Data { get; }
		public Guid InstanceId { get; }

		public CardInstance(CardInfoData data)
		{
			Data = data;
			InstanceId = Guid.NewGuid();
		}
	}
}