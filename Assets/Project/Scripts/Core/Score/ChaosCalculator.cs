using UnityEngine;
using System.Collections.Generic;

namespace Core.Scripts
{
    public static class ChaosCalculator
    {

        public static float ChaosValue(float damageDealt, float damageTaken, float damageAbsorbed, float multiplier)
        {
            float rawPoints = damageDealt + damageTaken ;
            return rawPoints * Mathf.Max(1f, multiplier + damageAbsorbed);
        }
        
        public static float CalculateParade(float attackValue, float paradeValue, out bool isPerfectFit,
	        out float damageRemaining)
        {
	        isPerfectFit = false;

	        if (paradeValue <= 0f)
	        {
		        damageRemaining = attackValue;
		        return 0f;
	        }

	        damageRemaining = Mathf.Max(0f, attackValue - paradeValue);
	        float damageAbsorbed = Mathf.Min(attackValue, paradeValue);

	        if (attackValue > 0f && attackValue == paradeValue)
	        {
		        isPerfectFit = true;
		        return damageAbsorbed * 2f;
	        }

	        return damageAbsorbed;
        }
    }
}