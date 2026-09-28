using System.Collections.Generic;
using Core.DeckSysteme.Cards;
using Core.Utilities;
using GamePlay.Card;
using UnityEngine;

namespace Core.DeckSysteme.Enemy
{
    public class EnemyController : MonoBehaviour
    {
	    [SerializeField] private PlayedField enemyField;
	    [SerializeField] private List<CardInfoData> masterDeck = new List<CardInfoData>();
	    [SerializeField] private int cardsToPlay = GameMetrix.MaxSelectable;
	    public Deck Deck { get; private set; }
	    private List<CardInstance> drawBuffer;	    
	    private void Awake()
	    {
		    drawBuffer = new List<CardInstance>(GameMetrix.MaxSelectable);
		    Deck = new Deck(masterDeck);
	    }

	    private void Start()
	    {
		    PlayEnemyTurn();
	    }
	    public void PlayEnemyTurn()
	    {
		    drawBuffer.Clear();

		    for (int i = 0; i < cardsToPlay; i++)
		    {
			    CardInstance drawn = Deck.DrawTopCard();

			    if (drawn == null)
			    {
				    Debug.LogWarning("[EnemyController] Deck ennemi vide, impossible de compléter le terrain.");
				    break;
			    }

			    drawBuffer.Add(drawn);
		    }

		    enemyField.PlayCards(drawBuffer);
	    }
    }
}
