using Core.DeckSysteme;
using Core.DeckSysteme.Enemy;
using Core.DeckSysteme.UI;

namespace Core.Phase
{
	public class RoundContext
	{
		public HandCards HandCards;
		public PlayedField PlayerField;
		public EnemyController Enemy;
		public TurnResolver Resolver;
		public ComboLibrary ComboLibrary;
		public CombatHUD Hud;
		
		public enum FightEndReason { None, TargetReached, OutOfTurns, OutOfHealth }

		public FightEndReason EndReason;
		public bool IsFightOver => EndReason != FightEndReason.None;

		public int TotalScore;
		public float PlayerHealth;
		public int CurrentTurn;
	}
}