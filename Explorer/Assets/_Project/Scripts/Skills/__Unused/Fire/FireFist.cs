using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFist : MonoBehaviour
{
    public FireFistEffects fireFistEffects;
    public int addedDamage = 1;
    public float duration = 30;


    public void activate() {
        
        fireFistEffects.activate();
    }

    public void updateFireFistEffects() {

        fireFistEffects.addedDamage = addedDamage;
        fireFistEffects.duration = duration;
    }
}
