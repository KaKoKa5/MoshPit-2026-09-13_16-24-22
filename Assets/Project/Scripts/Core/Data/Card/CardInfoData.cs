using UnityEngine;

namespace GamePlay.Card
{
    [CreateAssetMenu(fileName = "CardInfoData", menuName = "CardInfoData")]
    public class CardInfoData : ScriptableObject
    {
	    [Header("Informations Générales")]
	    [field:SerializeField] public string cardName;
	    [field:SerializeField] public Sprite cardIcon;
	    [field:SerializeField] private GameObject cardPrefab;

	    [Header("Attributs de Jeu")]
	    [field:SerializeField] private CardStyle style;
	    [field:SerializeField] private CardRarity rarity;
	    [field:SerializeField] private CardType type;
	    [field:SerializeField, Range(1,8)] public int baseValue;  
    }
}
