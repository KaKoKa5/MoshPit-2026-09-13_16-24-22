using UnityEngine;

namespace Core.Utilities
{
	public static class GameMetrix
	{
		[field: SerializeField] public static int MaxCardOnHand { get; private set; } = 6;
		[field: SerializeField] public static int StartDeckCard { get; private set; } = 15;
		
		[field: SerializeField] public static int MinSelectable = 1;
		[field: SerializeField] public static int MaxSelectable = 4;
	}
}