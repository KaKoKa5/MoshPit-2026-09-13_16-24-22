using Core.DeckSysteme.Cards;
using UnityEngine;

namespace Core.DeckSysteme
{
	[CreateAssetMenu(fileName = "Combo", menuName = "Mosh Pit/Combo")]
	public class ComboData : ScriptableObject
	{
		public string ComboName;
		public Discipline RequiredDiscipline;
		
		public CardMoveType[] Pattern = new CardMoveType[4];

		public int ChipsBonus;
		public int MultBonus;
	}
}