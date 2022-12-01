using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBreath : MonoBehaviour
{
    public FireBreathCollider fireBreathCollider;
    public Transform spawnPosition;
    public AudioSource audioSource;
    public float flameSpawnTime = .5f;
    public float flameLaunchForce = 10;
    public float flameColliderRadius = .5f;
    public int headDamage = 2;
    public int bodyDamage = 2;
    public float hitForce = 0;
    public float lifetime = 1.5f;
    
    private bool isActivated = false;
    private float timer = 0;


    private void Awake() {
        updateFireBreathCollider();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (isActivated) {

            timer -= Time.fixedDeltaTime;
            if (timer < 0) {

                spawnFlame();
            }
        }
    }


    public void updateFireBreathCollider() {

        fireBreathCollider.headDamage = headDamage;
        fireBreathCollider.bodyDamage = bodyDamage;
        fireBreathCollider.hitForce = hitForce;
        fireBreathCollider.transform.localScale = new Vector3(flameColliderRadius, flameColliderRadius, flameColliderRadius);
    }


    public void activate() {

        isActivated = true;
        timer = flameSpawnTime;
        audioSource.Play();
    }

    public void deactivate() {

        isActivated = false;
        audioSource.Stop();
    }

    private void spawnFlame() {

        var flame = Instantiate(fireBreathCollider.gameObject, spawnPosition);
        flame.transform.localPosition = new Vector3();
        flame.transform.SetParent(null);
        flame.SetActive(true);

        var flameRigid = flame.GetComponent<Rigidbody>();
        flameRigid.AddForce(spawnPosition.forward * flameLaunchForce, ForceMode.Impulse);

        Destroy(flame, lifetime);

        timer = flameSpawnTime;
    }
}
