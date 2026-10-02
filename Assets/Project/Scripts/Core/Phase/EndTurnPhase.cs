using System;
using System.Collections.Generic;
using Core.DeckSysteme.Cards;
using Core.Phase;
using Core.Utilities;

namespace Core.DeckSysteme.Phases
{
	public class EndTurnPhase : IPhases
	{

		private RoundContext context;
		private readonly List<CardInstance> discardBuffer = new List<CardInstance>(GameMetrix.MaxSelectable);

		public event Action OnPhaseCompleted;

		public void Initialize(RoundContext context)
		{
			this.context = context;
		}

		public void Execute()
		{
			//TODO changer ici
			if (context.TotalScore >= 100)
			{
				context.EndReason = RoundContext.FightEndReason.TargetReached;
				OnPhaseCompleted?.Invoke();
				return;
			}

			if (context.PlayerHealth <= 0)
			{
				context.EndReason = RoundContext.FightEndReason.OutOfHealth;
				OnPhaseCompleted?.Invoke();
				return;
			}

			if (context.CurrentTurn >= GameMetrix.MaxConcert)
			{
				context.EndReason = RoundContext.FightEndReason.OutOfTurns;
				OnPhaseCompleted?.Invoke();
				return;
			}

			DiscardField(context.PlayerField, DeckManager.Instance.Deck);
			DiscardField(context.Enemy.EnemyField, context.Enemy.Deck);

			OnPhaseCompleted?.Invoke(); // combat non terminé, l'orchestrateur reboucle vers DrawPhase
		}

		private void DiscardField(PlayedField field, Deck deck)
		{
			discardBuffer.Clear();
			discardBuffer.AddRange(field.Field.PlayedCards); // copie avant Clear()
			field.Field.Clear();
			deck.AddToDiscard(discardBuffer);
		}
	}
}