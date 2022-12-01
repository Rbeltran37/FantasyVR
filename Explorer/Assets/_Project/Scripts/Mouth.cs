using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouth : MonoBehaviour
{
    private const string DRINKABLE = "Drinkable";
    private PotionInteractor potionInteractor;

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag(DRINKABLE))
        {
            if(!potionInteractor)
                GetPotionInteractor(other.transform);
            
            DebugLogger.Info("triggerEnter", potionInteractor.GetLiquidAmountConsumed().ToString(), this);
        }
            
        
    }

    private void GetPotionInteractor(Transform potion)
    {
        potionInteractor = potion.GetComponent<PotionInteractor>();
    }
}
