using System;

namespace Core.Phase
{
	public interface IPhases
	{
		event Action OnPhaseCompleted;
		void Initialize(RoundContext ctx);

		void Execute();
		
	}
}