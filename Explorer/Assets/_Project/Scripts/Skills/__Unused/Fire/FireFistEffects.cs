using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFistEffects : MonoBehaviour
{
    //public WeaponHitHandler fistHitHandler;
    public int addedDamage = 1;
    public float duration = 30;

    private float timer = 0;


    // Start is called before the first frame update
    void Awake() {
        //transform.SetParent(fistHitHandler.transform);
    }


    private void OnEnable() {

        //fistHitHandler.bodyDamage += addedDamage;
        //fistHitHandler.headDamage += addedDamage;
    }

    private void OnDisable() {

        //fistHitHandler.bodyDamage -= addedDamage;
        //fistHitHandler.headDamage -= addedDamage;
    }


    // Update is called once per frame
    void Update() {
        if (timer > 0) {

            timer -= Time.deltaTime;
            if (timer < 0) {
                deactivate();
            }
        }
    }


    public void activate() {

        gameObject.SetActive(true);
        timer = duration;
    }

    private void deactivate() {

        timer = 0;
        gameObject.SetActive(false);
    }
}
