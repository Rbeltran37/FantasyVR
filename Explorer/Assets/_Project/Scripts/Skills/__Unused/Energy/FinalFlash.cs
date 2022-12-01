using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalFlash : MonoBehaviour
{
    public FinalFlashCollider finalFlashCollider;
    public GameObject chargeSphere;
    public GameObject chargeFX;
    public float range = 10;
    public float radius = 5;
    public int headDamage = 5;
    public int bodyDamage = 5;
    public float hitForce = 800;
    public float chargeTime = 3;
    public float duration = 1;
    public AudioSource audioSource;
    public AudioClip chargeStart;
    public AudioClip chargeEnd;

    private float timer = 0;
    private bool isCharging = false;


    private void Update() {
        
        if (isCharging) {

            timer -= Time.deltaTime;
            if (timer < 0) {

                if (!chargeSphere.activeSelf) {

                    audioSource.clip = chargeEnd;
                    audioSource.Play();
                    chargeSphere.SetActive(true);
                }
            }
        }
    }


    public void startCharge() {

        timer = chargeTime;
        isCharging = true;
        audioSource.clip = chargeStart;
        audioSource.Play();
        chargeFX.SetActive(true);
    }

    public void releaseCharge() {

        if (timer < 0) {

            chargeSphere.SetActive(false);
            fireBeam();
        }

        isCharging = false;
        audioSource.Stop();
        chargeFX.SetActive(false);
    }

    public void updateCollider() {

        finalFlashCollider.headDamage = headDamage;
        finalFlashCollider.bodyDamage = bodyDamage;
        finalFlashCollider.duration = duration;
        finalFlashCollider.range = range;
        finalFlashCollider.radius = radius;
    }


    private void fireBeam() {

        finalFlashCollider.gameObject.SetActive(true);
    }
}
