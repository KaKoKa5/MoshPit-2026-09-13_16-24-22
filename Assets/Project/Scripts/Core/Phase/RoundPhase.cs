using System.Collections.Generic;
using Core.DeckSysteme;
using Core.DeckSysteme.Enemy;
using Core.DeckSysteme.Phases;
using Core.DeckSysteme.UI;
using Core.Utilities;
using UnityEngine;

namespace Core.Phase
{
	public class RoundPhase :  MonoBehaviour
	{
		[SerializeField] private HandCards handCards;
		[SerializeField] private PlayedField playerField;
		[SerializeField] private EnemyController enemy;
		[SerializeField] private ComboLibrary comboLibrary;
		[SerializeField] private CombatHUD hud;
		

		private RoundContext context;
		private List<IPhases> phases;
		private int currentIndex;
		private IPhases activePhase;
		private void Start()
		{
			context = new RoundContext
			{
				HandCards = handCards,
				PlayerField = playerField,
				Enemy = enemy,
				PlayerHealth = GameMetrix.MaxHP,
				Resolver = new TurnResolver(),
				ComboLibrary = comboLibrary,
				Hud = hud
			};

			phases = new List<IPhases>
			{
				new DrawPhase(),
				new InputPhase(),
				new ResolvePhase(),
				new EndTurnPhase(),
			};

			foreach (IPhases phase in phases)
				phase.Initialize(context);
			
			hud.OnTurnStarted(currentIndex,GameMetrix.MaxHP);

			currentIndex = 0;
			RunCurrentPhase();
		}

		private void RunCurrentPhase()
		{
			activePhase = phases[currentIndex];
			activePhase.OnPhaseCompleted += OnPhaseCompleted;
			activePhase.Execute();
		}

		private void OnPhaseCompleted()
		{
			activePhase.OnPhaseCompleted -= OnPhaseCompleted;

			if (context.IsFightOver)
			{
				Debug.Log($"[Fight] Terminé : {context.EndReason}");
				return;
			}

			currentIndex++;
			
			if (currentIndex >= phases.Count)
				currentIndex = 0;

			RunCurrentPhase();
		}
		
		private void OnDestroy()
		{
			if (activePhase != null)
				activePhase.OnPhaseCompleted -= OnPhaseCompleted;
		}
	}
}