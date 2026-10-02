using System;
using Core.Phase;

namespace Core.DeckSysteme.Phases
{
	public class InputPhase : IPhases
	{
		public event Action OnPhaseCompleted;

		private RoundContext context;

		public void Initialize(RoundContext context)
		{
			this.context = context;
		}

		public void Execute()
		{
			context.HandCards.InputEnabled = true;
			context.HandCards.SelectionPlayed+= OnSelectionPlayed;
		}


		private void OnSelectionPlayed()
		{
			context.HandCards.SelectionPlayed -= OnSelectionPlayed;
			context.HandCards.InputEnabled = false;
			OnPhaseCompleted?.Invoke();
		}
	}
}