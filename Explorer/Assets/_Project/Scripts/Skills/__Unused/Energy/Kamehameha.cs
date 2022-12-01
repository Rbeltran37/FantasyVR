using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kamehameha : MonoBehaviour
{
    public GameObject beamPrefab;
    public GameObject chargeFX;
    public GameObject chargeSphere;
    public int minBodyDamage = 5;
    public int minHeadDamage = 5;
    public int maxBodyDamage = 10;
    public int maxHeadDamage = 10;
    public float minForce = 700;
    public float maxForce = 1200;
    public float minChargeSphereRadius = .05f;
    public float maxChargeSphereRadius = .25f;
    public float minBeamRadius = .5f;
    public float maxBeamRadius = 2;
    public float minExplosionRadius = 2;
    public float maxExplosionRadius = 5;
    public float minChargeTime = 2;
    public float maxChargeTime = 5;
    public float launchForce = 20;
    public float lifetime = 3;
    public float explosionTime = .3f;
    public Transform emitter;
    public KamehamehaCollider beamScript;

    private bool isCharging = false;
    private float chargeTimer = 0;


    private void Awake() {
        updateBeam();
    }

    // Update is called once per frame
    void Update()
    {
        if (isCharging && chargeTimer < maxChargeTime) {

            chargeTimer += Time.deltaTime;
            if (chargeTimer > minChargeTime) {

                if (!chargeSphere.activeSelf) {

                    chargeSphere.SetActive(true);
                }
                else {
                    var charge = chargeTimer;
                    if (chargeTimer > maxChargeTime)
                        charge = maxChargeTime;
                    var significantChargeTime = charge - minChargeTime;
                    var chargeFactor = significantChargeTime / (maxChargeTime - minChargeTime);

                    var chargeSphereRadius = chargeFactor * (maxChargeSphereRadius - minChargeSphereRadius);
                    var scale = minChargeSphereRadius + chargeSphereRadius;
                    chargeSphere.transform.localScale = new Vector3(scale, scale, scale);
                }
            }
        }
    }


    public void startCharge() {

        isCharging = true;
        chargeFX.SetActive(true);
    }

    public void releaseCharge() {

        isCharging = false;

        if (chargeTimer > minChargeTime) {

            chargeSphere.SetActive(false);
            fireBeam();
        }
        chargeTimer = 0;
        chargeFX.SetActive(false);
    }

    private void fireBeam() {

        if (beamPrefab && emitter) {

            var beam = Instantiate(beamPrefab, emitter.transform.position, emitter.transform.rotation);
            if (beam) {

                beam.SetActive(true);
                Destroy(beam, lifetime);

                var charge = chargeTimer;
                if (chargeTimer > maxChargeTime)
                    charge = maxChargeTime;
                var significantChargeTime = charge - minChargeTime;
                var chargeFactor = significantChargeTime / (maxChargeTime - minChargeTime);

                var beamScript = beam.GetComponent<KamehamehaCollider>();
                if (beamScript) {

                    beamScript.setBeamParameters(chargeFactor);
                }

                var beamRigid = beam.GetComponent<Rigidbody>();
                if (beamRigid) {

                    beamRigid.AddForce(emitter.transform.forward * launchForce, ForceMode.Impulse);
                }
            }
        }
    }

    private void updateBeam() {

        if (beamPrefab && beamScript) {

            beamScript.minBodyDamage = minBodyDamage;
            beamScript.minHeadDamage = minHeadDamage;
            beamScript.maxBodyDamage = maxBodyDamage;
            beamScript.maxHeadDamage = maxHeadDamage;
            beamScript.minForce = minForce;
            beamScript.maxForce = maxForce;
            beamScript.minBeamRadius = minBeamRadius;
            beamScript.maxBeamRadius = maxBeamRadius;
            beamScript.minExplosionRadius = minExplosionRadius;
            beamScript.maxExplosionRadius = maxExplosionRadius;
            beamScript.minChargeTime = minChargeTime;
            beamScript.maxChargeTime = maxChargeTime;
            beamScript.explosionTime = explosionTime;
        }
    }
}
