using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buster : MonoBehaviour
{
    public Transform emitter;
    public GameObject[] shotPrefabs;
    public GameObject[] chargeParticleSystems;
    public AudioClip[] shotSounds;
    public AudioClip[] chargeSounds;
    public float smallChargeTime = .5f;
    public float mediumChargeTime = 1.3f;
    public float largeChargeTime = 2.2f;
    public float shotForce = 65;
    public float shotLifetime = 3;

    [Header("No Charge")]
    public int noChargeBodyDamage = 1;
    public int noChargeHeadDamage = 1;
    public float noChargeForce = 100;
    public int noChargeNumHits = 1;

    [Header("Small Charge")]
    public int smallChargeBodyDamage = 3;
    public int smallChargeHeadDamage = 3;
    public float smallChargeForce = 400;
    public int smallChargeNumHits = 2;

    [Header("Medium Charge")]
    public int mediumChargeBodyDamage = 5;
    public int mediumChargeHeadDamage = 5;
    public float mediumChargeForce = 600;
    public int mediumChargeNumHits = 3;

    [Header("Large Charge")]
    public int largeChargeBodyDamage = 10;
    public int largeChargeHeadDamage = 10;
    public float largeChargeForce = 1000;
    public int largeChargeNumHits = 4;


    private bool isCharging = false;
    private bool isShooting = false;
    private float chargeTimer = 0;
    private GameObject currentShot;
    private GameObject currentCharge;
    private int numCharges = 3;
    private AudioSource audioSource;
    private AudioClip currentSound;

    // Start is called before the first frame update
    void Awake()
    {
        numCharges = chargeParticleSystems.Length;
        audioSource = GetComponent<AudioSource>();
        updateShotPrefabs();
    }

    // Update is called once per frame
    void Update()
    {
        if (isCharging) {

            chargeTimer += Time.deltaTime;

            setChargeType();
        }

        if (!isShooting)
            return;

        setShotType();

        fire(currentShot);
    }

    private void setChargeType() {

        if (chargeTimer > largeChargeTime) {

            currentCharge = chargeParticleSystems[2];

            audioSource.loop = true;
            audioSource.clip = chargeSounds[1];
            if (!audioSource.isPlaying) {

                audioSource.Play();
            }
        }
        else if (chargeTimer > mediumChargeTime) {

            currentCharge = chargeParticleSystems[1];
        }
        else if (chargeTimer > smallChargeTime) {

            currentCharge = chargeParticleSystems[0];

            if (!audioSource.isPlaying) {

                audioSource.PlayOneShot(chargeSounds[0]);
            }
        }

        if (currentCharge) {

            for (int i = 0; i < numCharges; i++) {

                if (chargeParticleSystems[i] != currentCharge) {

                    chargeParticleSystems[i].SetActive(false);
                }
            }
            currentCharge.SetActive(true);
        }
    }

    private void setShotType() {

        if (chargeTimer < smallChargeTime) {

            currentShot = shotPrefabs[0];
            currentSound = shotSounds[0];
        }
        else if (chargeTimer < mediumChargeTime) {

            currentShot = shotPrefabs[1];
            currentSound = shotSounds[1];
        }
        else if (chargeTimer < largeChargeTime) {

            currentShot = shotPrefabs[2];
            currentSound = shotSounds[2];
        }
        else {

            currentShot = shotPrefabs[3];
            currentSound = shotSounds[3];
        }
    }

    private void fire(GameObject prefab) {

        isCharging = false;
        isShooting = false;
        chargeTimer = 0;

        var tempShot = Instantiate(prefab, null, true);
        tempShot.SetActive(true);
        tempShot.transform.rotation = emitter.rotation;
        tempShot.transform.position = emitter.position + emitter.transform.forward * tempShot.transform.localScale.z;
        tempShot.GetComponent<Rigidbody>().AddForce(emitter.transform.forward * shotForce, ForceMode.Impulse);
        Destroy(tempShot, shotLifetime);

        if (currentCharge) {

            currentCharge.SetActive(false);
            currentCharge = null;
        }

        audioSource.Stop();
        audioSource.loop = false;
        audioSource.PlayOneShot(currentSound);
    }

    public void chargeShot() {

        isCharging = true;
    }

    public void releaseShot() {

        isShooting = true;
    }

    public void updateShotPrefabs() {

        if (shotPrefabs.Length > 3) {

            var noCharge = shotPrefabs[0].GetComponent<BusterShotCollider>();
            if (noCharge) {
                noCharge.bodyDamage = noChargeBodyDamage;
                noCharge.headDamage = noChargeHeadDamage;
                noCharge.force = noChargeForce;
                noCharge.numHits = noChargeNumHits;
            }

            var smallCharge = shotPrefabs[1].GetComponent<BusterShotCollider>();
            if (smallCharge) {
                smallCharge.bodyDamage = smallChargeBodyDamage;
                smallCharge.headDamage = smallChargeHeadDamage;
                smallCharge.force = smallChargeForce;
                smallCharge.numHits = smallChargeNumHits;
            }

            var mediumCharge = shotPrefabs[2].GetComponent<BusterShotCollider>();
            if (mediumCharge) {
                mediumCharge.bodyDamage = mediumChargeBodyDamage;
                mediumCharge.headDamage = mediumChargeHeadDamage;
                mediumCharge.force = mediumChargeForce;
                mediumCharge.numHits = mediumChargeNumHits;
            }

            var largeCharge = shotPrefabs[3].GetComponent<BusterShotCollider>();
            if (largeCharge) {
                largeCharge.bodyDamage = largeChargeBodyDamage;
                largeCharge.headDamage = largeChargeHeadDamage;
                largeCharge.force = largeChargeForce;
                largeCharge.numHits = largeChargeNumHits;
            }
        }
    }
}
