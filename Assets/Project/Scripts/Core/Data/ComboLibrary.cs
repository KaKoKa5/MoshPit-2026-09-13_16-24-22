using System.Collections.Generic;
using UnityEngine;

namespace Core.DeckSysteme
{
	public class ComboLibrary : MonoBehaviour
	{
		[SerializeField] private List<ComboData> combos = new List<ComboData>();

		public IReadOnlyList<ComboData> Combos => combos;
	}
}