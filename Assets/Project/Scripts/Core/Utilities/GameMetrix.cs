using UnityEngine;

namespace Core.Utilities
{
	public static class GameMetrix
	{
		[field: SerializeField] public static int MaxCardOnHand { get; private set; } = 6;
		[field: SerializeField] public static int StartDeckCard { get; private set; } = 15;
		
		[field: SerializeField] public static int MinSelectable { get; private set; } = 1;
		[field: SerializeField] public static int MaxSelectable { get; private set; } = 4;
		
		[field: SerializeField] public static int InitialPoolSize { get; private set; } = 10;
	}
}