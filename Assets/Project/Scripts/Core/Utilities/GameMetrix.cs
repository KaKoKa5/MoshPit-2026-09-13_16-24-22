using UnityEngine;

namespace Core.Utilities
{
	public class GameMetrix : ScriptableObject
	{
		[field: SerializeField] public int MaxCardOnHand { get; private set; } = 6;
		[field: SerializeField] public int StartDeckCard { get; private set; } = 15;

		[field: SerializeField] public int MinSelectable { get; private set; } = 1;
		[field: SerializeField] public int MaxSelectable { get; private set; } = 4;

		[field: SerializeField] public int InitialPoolSize { get; private set; } = 10;

		private static GameMetrix _instance;

		public static GameMetrix Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Resources.Load<GameMetrix>("GameMetrix");

					if (_instance == null)
					{
						Debug.LogError("[GameMetrix] Aucun asset 'GameMetrix' trouvé dans un dossier Resources ! " +
						               "Crée-le via Assets > Create > Mosh Pit > Game Metrix, et place-le dans un dossier nommé 'Resources'.");
					}
				}

				return _instance;
			}
		}
	}
}