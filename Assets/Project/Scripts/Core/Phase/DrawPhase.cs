using System;
using UnityEngine;

namespace Core.Phase
{
	public class DrawPhase : IPhases
	{
		public event Action OnPhaseCompleted;
		private RoundContext context;
		
		public void Initialize(RoundContext ctx)
		{
			this.context = ctx;
		}

		public void Execute()
		{
			context.CurrentTurn++;
			context.HandCards.Hand.FillHand();
			context.Enemy.PlayEnemyTurn();
    
			OnPhaseCompleted?.Invoke();
		}

	}
}