using System;
using System.Collections.Generic;
using Core.DeckSysteme.Cards;
using Core.Utilities;
using UnityEngine;

namespace Core.DeckSysteme
{
    public enum CardType { Attack, Grab, Defense, Dodge }

    public enum TypeOfResult { Nothing, Exchange, Dominance, Guard, PerfectGuard, DodgeSuccess, Malus }

    public readonly struct CollisionStep
    {
        public readonly int Index;
        public readonly TypeOfResult Outcome;
        public readonly int ChipsDelta;
        public readonly int MultDelta;
        public readonly int DamageToPlayer;
        public readonly int ChipsAfter;
        public readonly int MultAfter;

        public CollisionStep(int index, TypeOfResult outcome, int chipsDelta, int multDelta,
            int damageToPlayer, int chipsAfter, int multAfter)
        {
            Index = index;
            Outcome = outcome;
            ChipsDelta = chipsDelta;
            MultDelta = multDelta;
            DamageToPlayer = damageToPlayer;
            ChipsAfter = chipsAfter;
            MultAfter = multAfter;
        }
    }

    public class TurnResolver
    {
        // Réutilisée à chaque tour : aucune allocation en jeu
        public List<CollisionStep> Steps { get; } = new List<CollisionStep>(GameMetrix.MaxSelectable);

        public int Chips { get; private set; }
        public int Mult { get; private set; }
        public int DamageToPlayer { get; private set; }
        public int Total => Chips * Mult;
        public  event Action<CollisionStep> CollisionResolved;

        public int Resolve(List<CardInstance> playerCards, List<CardInstance> enemyCards)
        {
            Steps.Clear();
            Chips = 0;
            Mult = GameMetrix.BaseMult;
            DamageToPlayer = 0;

            int count = Mathf.Min(playerCards.Count, enemyCards.Count);

            for (int i = 0; i < count; i++)
            {
                ResolveCollision(
                    GetKind(playerCards[i]), playerCards[i].Data.baseValue,
                    GetKind(enemyCards[i]), enemyCards[i].Data.baseValue,
                    out int chipsDelta, out int multDelta, out int damage, out TypeOfResult outcome);

                Chips = Mathf.Max(0, Chips + chipsDelta);
                Mult += multDelta;
                DamageToPlayer += damage;

                Steps.Add(new CollisionStep(i, outcome, chipsDelta, multDelta, damage, Chips, Mult));
                CollisionResolved?.Invoke(Steps[Steps.Count - 1]);
            }
            
            return Total;
        }

        /// <summary>
        /// Ajoute un bonus fixe de Chips et/ou Mult après la résolution des collisions
        /// (combos de gestes, bonus de discipline, futurs Jokers).
        /// </summary>
        public void AddBonus(int chipsBonus, int multBonus)
        {
            Chips = Mathf.Max(0, Chips + chipsBonus);
            Mult += multBonus;
        }

        /// <summary>
        /// Applique un facteur multiplicatif au Mult actuel (PV bas, futurs Jokers).
        /// </summary>
        public void ApplyMultFactor(float factor)
        {
            Mult = Mathf.RoundToInt(Mult * factor);
        }

        private void ResolveCollision(CardType pType, int pValue, CardType eType, int eValue,
            out int chips, out int mult, out int damage, out TypeOfResult outcome)
        {
            chips = 0;
            mult = 0;
            damage = 0;
            outcome = TypeOfResult.Nothing;

            bool pOffensive = IsOffensive(pType);
            bool eOffensive = IsOffensive(eType);

            if (pOffensive && eOffensive)
            {
                if (pType == CardType.Attack && eType == CardType.Attack)
                {
                    chips = pValue + eValue;
                    mult = GameMetrix.ExchangeMult;
                    damage = eValue;
                    outcome = TypeOfResult.Exchange;
                }
                else
                {
                    chips = pValue == eValue ? pValue + eValue : Mathf.Max(pValue, eValue);
                    damage = eValue >= pValue ? eValue : 0;
                    outcome = TypeOfResult.Dominance;
                }
                return;
            }

            if (pOffensive && eType == CardType.Defense)
            {
                Guard(pValue, eValue, out chips, out mult, out outcome);
                return;
            }

            if (eOffensive && pType == CardType.Defense)
            {
                Guard(eValue, pValue, out chips, out mult, out outcome);
                damage = chips;
                return;
            }

            if ((pOffensive && eType == CardType.Dodge) || (eOffensive && pType == CardType.Dodge))
            {
                mult = GameMetrix.DodgeMult;
                outcome = mult > 0 ? TypeOfResult.DodgeSuccess : TypeOfResult.Nothing;
                return;
            }

            // DEF/DEF, DEF/ESQ, ESQ/ESQ : personne ne s'engage, le morceau retombe
            chips = -(pValue + eValue);
            outcome = TypeOfResult.Malus;
        }

        private static void Guard(int offense, int defense, out int chips, out int mult, out TypeOfResult outcome)
        {
            chips = Mathf.Max(0, offense - defense);
            int absorbed = Mathf.Min(offense, defense);

            if (offense == defense)
            {
                mult = absorbed * 2; // parade parfaite
                outcome = TypeOfResult.PerfectGuard;
            }
            else
            {
                mult = absorbed;
                outcome = TypeOfResult.Guard;
            }
        }

        private static bool IsOffensive(CardType type) => type == CardType.Attack || type == CardType.Grab;

        private static CardType GetKind(CardInstance card)
        {
            switch (card.Data.MoveType)
            {
                case CardMoveType.Punch:
                case CardMoveType.Kick:
                case CardMoveType.Head:
                case CardMoveType.Elbow:
                    return CardType.Attack;
                case CardMoveType.Grab:
                    return CardType.Grab;
                case CardMoveType.Parade:
                    return CardType.Defense;
                default:
                    return CardType.Dodge;
            }
        }
    }
}