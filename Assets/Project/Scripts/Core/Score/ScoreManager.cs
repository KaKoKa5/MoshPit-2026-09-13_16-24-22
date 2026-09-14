using System;
using UnityEngine;

namespace Core.Scripts
{
	public struct DuelResult
	{
		public float damageDealt;
		public float damageTaken;
		public float damageAbsorbed;
		public bool isPerfectFit;
	}

	public class ScoreManager
	{
		public float PlayerHp { get; private set; }
		public float PlayerMaxHp { get; private set; }
		public float EnemyHp { get; private set; }
		public float EnemyMaxHp { get; private set; }

		private float currentTurnScore;
		public float SongTotalScore { get; private set; }
		public float ScoreQuota { get; private set; }

		public float CurrentMultiplier { get; private set; } = 1f;
		
		public event Action<float> OnPlayerHpChanged;
		public event Action<float> OnEnemyHpChanged;
		public event Action<float> OnTurnScoreChanged;
		public event Action<float> OnSongScoreChanged;
		public event Action OnQuotaReached;
		public event Action OnPlayerDefeated;

		private bool quotaAlreadyNotified;
		private bool defeatAlreadyNotified;

		public ScoreManager(float playerMaxHp, float enemyMaxHp, float scoreQuota)
		{
			PlayerMaxHp = playerMaxHp;
			EnemyMaxHp = enemyMaxHp;
			PlayerHp = playerMaxHp;
			EnemyHp = enemyMaxHp;
			ScoreQuota = scoreQuota;

			currentTurnScore = 0f;
			SongTotalScore = 0f;
		}

		public void StartNewTurn()
		{
			currentTurnScore = 0f;
		}
		
		public DuelResult ResolveParadeDuel(float enemyAttackValue, float playerParadeValue, float playerDamageDealt)
		{
			float damageAbsorbed = ChaosCalculator.CalculateParade(enemyAttackValue, playerParadeValue, out bool isPerfectFit, out float damageRemaining);

			var result = new DuelResult { damageDealt = playerDamageDealt, damageTaken = damageRemaining, damageAbsorbed = damageAbsorbed, isPerfectFit = isPerfectFit };

			ApplyDuelResult(in result);
			return result;
		}

		public DuelResult ResolveAttackDuel(float playerDamageDealt, float enemyDamageDealt)
		{
			var result = new DuelResult { damageDealt = playerDamageDealt, damageTaken = enemyDamageDealt, damageAbsorbed = 0f, isPerfectFit = false };

			ApplyDuelResult(in result);
			return result;
		}

		private void ApplyDuelResult(in DuelResult result)
		{
			if (result.damageDealt > 0f)
			{
				EnemyHp = Mathf.Max(0f, EnemyHp - result.damageDealt);
				OnEnemyHpChanged?.Invoke(EnemyHp);
			}

			if (result.damageTaken > 0f)
			{
				PlayerHp = Mathf.Max(0f, PlayerHp - result.damageTaken);
				OnPlayerHpChanged?.Invoke(PlayerHp);
			}

			float totalTurnMultiplier = CurrentMultiplier + result.damageAbsorbed;
			float duelScore = ChaosCalculator.ChaosValue(result.damageDealt, result.damageTaken, result.damageAbsorbed ,totalTurnMultiplier);

			currentTurnScore += duelScore;
			SongTotalScore += duelScore;

			OnTurnScoreChanged?.Invoke(currentTurnScore);
			OnSongScoreChanged?.Invoke(SongTotalScore);

			CheckGameState();
		}

		private void CheckGameState()
		{
			if (!defeatAlreadyNotified && PlayerHp <= 0f)
			{
				defeatAlreadyNotified = true;
				OnPlayerDefeated?.Invoke();
			}

			if (!quotaAlreadyNotified && SongTotalScore >= ScoreQuota)
			{
				quotaAlreadyNotified = true;
				OnQuotaReached?.Invoke();
			}
		}
	}
}