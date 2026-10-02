using System.Text;
using Core.DeckSysteme;
using Core.Phase;
using Core.Utilities;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.DeckSysteme.UI
{
    public class CombatHUD : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RoundPhase roundPhase; // pour accéder au RoundContext
        [SerializeField] private TurnResolver resolver;  // la même instance que celle du RoundContext

        [Header("Round / Score")]
        [SerializeField] private TMP_Text roundText;
        [SerializeField] private TMP_Text totalScoreText;

        [Header("Chips x Mult")]
        [SerializeField] private TMP_Text chipsText;
        [SerializeField] private TMP_Text multText;
        [SerializeField] private TMP_Text turnTotalText;

        [Header("PV")]
        [SerializeField] private Image healthBarFill;

        [Header("Combo")]
        [SerializeField] private GameObject comboPanel;
        [SerializeField] private TMP_Text comboNameText;
        [SerializeField] private TMP_Text comboChipsText;
        [SerializeField] private TMP_Text comboMultText;

        private Tween chipsPunch;
        private Tween multPunch;

        private void OnEnable()
        {
            resolver.CollisionResolved += OnCollisionResolved;
        }

        private void OnDisable()
        {
            resolver.CollisionResolved -= OnCollisionResolved;
        }

        /// <summary>
        /// Appelé par RoundPhase/ResolvePhase en tout début de tour, pour remettre l'affichage à zéro.
        /// </summary>
        public void OnTurnStarted(int currentTurn, float playerHealth)
        {
            roundText.text = currentTurn.ToString();
            chipsText.text = "0";
            multText.text = "0";
            turnTotalText.text = "0";
            comboPanel.SetActive(false);

            UpdateHealthBar(playerHealth);
        }

        /// <summary>
        /// Appelé à chaque collision résolue (via l'event ajouté sur TurnResolver).
        /// </summary>
        private void OnCollisionResolved(CollisionStep step)
        {
            chipsText.text = step.ChipsAfter.ToString();
            multText.text = step.MultAfter.ToString();
            turnTotalText.text = (step.ChipsAfter * step.MultAfter).ToString();

            PunchText(chipsText.transform, ref chipsPunch);
            PunchText(multText.transform, ref multPunch);
        }

        /// <summary>
        /// Appelé une fois les bonus de combo/discipline/PV bas appliqués, juste avant la fin du tour.
        /// </summary>
        public void OnTurnResolved(int totalScore, int playerHealth)
        {
            chipsText.text = resolver.Chips.ToString();
            multText.text = resolver.Mult.ToString();
            turnTotalText.text = resolver.Total.ToString();

            totalScoreText.text = totalScore.ToString();
            UpdateHealthBar(playerHealth);
        }

        public void ShowCombo(string comboName, int chipsBonus, int multBonus)
        {
            comboPanel.SetActive(true);
            comboNameText.text = comboName;
            comboChipsText.text = $"+{chipsBonus}";
            comboMultText.text = $"+{multBonus}";
        }

        private void UpdateHealthBar(float playerHealth)
        {
            float ratio = Mathf.Clamp01(playerHealth / GameMetrix.MaxHP);
            Tween.Custom(healthBarFill.fillAmount, ratio, 0.4f, val => healthBarFill.fillAmount = val);
        }

        private void PunchText(Transform target, ref Tween existing)
        {
            if (existing.isAlive) existing.Stop();
            target.localScale = Vector3.one;
            existing = Tween.Scale(target, Vector3.one * 1.3f, 0.08f, Ease.OutQuad, cycles: 2, cycleMode: CycleMode.Yoyo);
        }
    }
}