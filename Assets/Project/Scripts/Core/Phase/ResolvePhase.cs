using System;
using Core.Phase;
using Core.Utilities;
using Helteix.Tools.Phases;
using PrimeTween;

namespace Core.DeckSysteme.Phases
{
    public class ResolvePhase : IPhases
    {
        private RoundContext context;
        private readonly float resolveDelay = 0.8f;

        public event Action OnPhaseCompleted;

        public void Initialize(RoundContext context)
        {
            this.context = context;
        }

        public void Execute()
        {
            int baseScore = context.Resolver.Resolve(context.PlayerField.Field.PlayedCards, context.Enemy.EnemyField.Field.PlayedCards);

            // Bonus de combo (gestes) et de discipline, sur la main jouée
            var namedCombos = ComboDetector.DetectNamedCombos(
                context.PlayerField.Field.PlayedCards, context.ComboLibrary.Combos);

            foreach (var combo in namedCombos)
            {
	            context.Resolver.AddBonus(combo.ChipsBonus, combo.MultBonus);
	            context.Hud.ShowCombo(combo.Name, combo.ChipsBonus, combo.MultBonus); // nouveau
            }

            int disciplineMult = ComboDetector.DetectDisciplineMult(context.PlayerField.Field.PlayedCards);
            if (disciplineMult > 0)
                context.Resolver.AddBonus(0, disciplineMult);

            // Multiplicateur PV bas, appliqué en dernier
            ApplyLowHealthMult();

            int turnScore = context.Resolver.Total;
            context.TotalScore += turnScore;
            context.PlayerHealth = UnityEngine.Mathf.Max(0, context.PlayerHealth - context.Resolver.DamageToPlayer);

            UnityEngine.Debug.Log($"[Tour {context.CurrentTurn}] {context.Resolver.Chips} x {context.Resolver.Mult} = {turnScore} | " +
                                  $"Score {context.TotalScore} | PV {context.PlayerHealth}");

            context.Hud.OnTurnResolved(context.TotalScore, (int)context.PlayerHealth); 

            Tween.Delay(resolveDelay, () => OnPhaseCompleted?.Invoke());
        }

        private void ApplyLowHealthMult()
        {
            float healthPercent = 100f * context.PlayerHealth / GameMetrix.MaxHP;

            if (healthPercent < 25f)
                context.Resolver.ApplyMultFactor(GameMetrix.FirstBuff);
            else if (healthPercent < 50f)
                context.Resolver.ApplyMultFactor(GameMetrix.SecondBuff);
            else if (healthPercent < 75f)
                context.Resolver.ApplyMultFactor(GameMetrix.ThirdBuff);
        }
    }
}