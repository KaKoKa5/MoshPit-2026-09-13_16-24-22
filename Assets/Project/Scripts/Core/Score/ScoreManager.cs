using System;
using UnityEngine;

namespace Core.Scripts
{
	public struct DuelResult
	{
		public float DamageDealt;
		public float DamageTaken;
		public float DamageAbsorbed;
		public bool IsPerfectFit;
	}

	public class ScoreManager
	{
		public float PlayerHP { get; private set; }
		public float PlayerMaxHP { get; private set; }
		public float EnemyHP { get; private set; }
		public float EnemyMaxHP { get; private set; }

		public float CurrentTurnScore { get; private set; }
		public float SongTotalScore { get; private set; }
		public float ScoreQuota { get; private set; }

		public event Action<float> OnPlayerHPChanged;
		public event Action<float> OnEnemyHPChanged;
		public event Action<float> OnTurnScoreChanged;
		public event Action<float> OnSongScoreChanged;
		public event Action OnQuotaReached;
		public event Action OnPlayerDefeated;

		private bool _quotaAlreadyNotified;
		private bool _defeatAlreadyNotified;

		public ScoreManager(float playerMaxHP, float enemyMaxHP, float scoreQuota)
		{
			PlayerMaxHP = playerMaxHP;
			EnemyMaxHP = enemyMaxHP;
			PlayerHP = playerMaxHP;
			EnemyHP = enemyMaxHP;
			ScoreQuota = scoreQuota;

			CurrentTurnScore = 0f;
			SongTotalScore = 0f;
		}

		public void StartNewTurn()
		{
			CurrentTurnScore = 0f;
		}
		
		public DuelResult ResolveParadeDuel(float enemyAttackValue, float playerParadeValue, float playerDamageDealt)
		{
			float damageAbsorbed = ChaosCalculator.CalculateParade(
				enemyAttackValue, playerParadeValue,
				out bool isPerfectFit,
				out float damageRemaining);

			var result = new DuelResult
			{
				DamageDealt = playerDamageDealt,
				DamageTaken = damageRemaining,
				DamageAbsorbed = damageAbsorbed,
				IsPerfectFit = isPerfectFit
			};

			ApplyDuelResult(in result);
			return result;
		}

		public DuelResult ResolveAttackDuel(float playerDamageDealt, float enemyDamageDealt)
		{
			var result = new DuelResult
			{
				DamageDealt = playerDamageDealt,
				DamageTaken = enemyDamageDealt,
				DamageAbsorbed = 0f,
				IsPerfectFit = false
			};

			ApplyDuelResult(in result);
			return result;
		}

		private void ApplyDuelResult(in DuelResult result)
		{
			if (result.DamageDealt > 0f)
			{
				EnemyHP = Mathf.Max(0f, EnemyHP - result.DamageDealt);
				OnEnemyHPChanged?.Invoke(EnemyHP);
			}

			if (result.DamageTaken > 0f)
			{
				PlayerHP = Mathf.Max(0f, PlayerHP - result.DamageTaken);
				OnPlayerHPChanged?.Invoke(PlayerHP);
			}
			
			float rawPoints = result.DamageDealt + result.DamageTaken;
			CurrentTurnScore += rawPoints;
			SongTotalScore += rawPoints;

			OnTurnScoreChanged?.Invoke(CurrentTurnScore);
			OnSongScoreChanged?.Invoke(SongTotalScore);
		}
	}
}