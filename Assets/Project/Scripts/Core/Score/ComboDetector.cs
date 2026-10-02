using System.Collections.Generic;
using Core.DeckSysteme.Cards;
using Core.Utilities;

namespace Core.DeckSysteme
{
    public readonly struct ComboMatch
    {
        public readonly string Name;
        public readonly int ChipsBonus;
        public readonly int MultBonus;

        public ComboMatch(string name, int chipsBonus, int multBonus)
        {
            Name = name;
            ChipsBonus = chipsBonus;
            MultBonus = multBonus;
        }
    }

    public static class ComboDetector
    {
        /// <summary>
        /// Détecte les combos de gestes sur une ligne de 4 cartes (uniquement sur le MoveType,
        /// indépendamment de la discipline).
        /// </summary>
        public static List<ComboMatch> DetectNamedCombos(List<CardInstance> playedCards, IReadOnlyList<ComboData> library)
        {
            var matches = new List<ComboMatch>();

            if (playedCards.Count != GameMetrix.MaxSelectable)
                return matches;

            for (int c = 0; c < library.Count; c++)
            {
                ComboData combo = library[c];

                if (MatchesPattern(playedCards, combo.Pattern))
                    matches.Add(new ComboMatch(combo.ComboName, combo.ChipsBonus, combo.MultBonus));
            }

            return matches;
        }

        /// <summary>
        /// Bonus de Mult si les 4 cartes jouées partagent la même discipline,
        /// indépendamment de si un combo de gestes a été reconnu.
        /// </summary>
        public static int DetectDisciplineMult(List<CardInstance> playedCards)
        {
            if (playedCards.Count != GameMetrix.MaxSelectable)
                return 0;

            Discipline first = playedCards[0].Data.Discipline;

            for (int i = 1; i < playedCards.Count; i++)
            {
                if (playedCards[i].Data.Discipline != first)
                    return 0;
            }

            return GameMetrix.DisciplineBonusMult;
        }

        private static bool MatchesPattern(List<CardInstance> playedCards, CardMoveType[] pattern)
        {
            if (pattern.Length != playedCards.Count)
                return false;

            for (int i = 0; i < pattern.Length; i++)
            {
                if (playedCards[i].Data.MoveType != pattern[i])
                    return false;
            }

            return true;
        }
    }
}