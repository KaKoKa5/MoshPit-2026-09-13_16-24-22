using UnityEngine;

namespace Core.Utilities
{
	public static class GameMetrix
	{
		[Header("Player")]
		[Header("PV & Buff")]
		public static float MaxHP { get; private set; } = 100;

		public static float FirstBuff { get; private set; } = 0.25f;
		public static float SecondBuff { get; private set; } = 0.75f;
		public static float ThirdBuff { get; private set; } = 1.25f;
		
		
		[Header("Concert")]
		
		public static int MaxConcert { get; private set; } = 4;
		public static float FirstPhase { get; private set; } = 0.25f;
		public static float SecondPhase { get; private set; } = 0.75f;
		public static float ThirdPhase { get; private set; } = 1.25f;
		public static float FourthPhase { get; private set; } = 0.75f;
		
		[Header("GameCardSetting")]
		public static int MaxCardOnHand { get; private set; } = 6;
		public static int StartDeckCard { get; private set; }= 15;
		public static int MinSelectable { get; private set; }= 1;
		public static int MaxSelectable { get; private set; }= 4;
		public static int InitialPoolSize { get; private set; }= 30;
		public static int DodgeMult { get; private set; } = 1;
		public static int ExchangeMult { get; private set; } = 3;
		public static int BaseMult { get; private set; } = 2;
		public static int DisciplineBonusMult { get; private set; } = 5;
	}
}