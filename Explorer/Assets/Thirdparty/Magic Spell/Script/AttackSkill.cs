using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class AttackSkill : MonoBehaviour {
 
    public GameObject theBullet;
    public Transform barrelEnd;
 
    public int bulletSpeed;
    public float despawnTime = 3.0f;
 
    public bool shootAble = true;
    public float waitBeforeNextShot = 0.25f;
 
    private void Update ()
    {
        if (Input.GetKeyDown ("space"))
        {
            if (shootAble)
            {
                shootAble = false;
                Shoot ();
                StartCoroutine (ShootingYield ());
            }
        }
    }
 
    IEnumerator ShootingYield ()
    {
        yield return new WaitForSeconds (waitBeforeNextShot);
        shootAble = true;
    }
    void Shoot ()
    {
        var bullet = Instantiate (theBullet, barrelEnd.position, barrelEnd.rotation);
        bullet.GetComponent<Rigidbody> ().velocity = bullet.transform.forward * bulletSpeed;
 
        Destroy (bullet, despawnTime);
    }
}