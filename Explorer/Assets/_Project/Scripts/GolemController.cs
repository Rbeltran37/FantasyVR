using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GolemController : MonoBehaviour
{
    private Animator animator;
    private float timeToSpin = 3f;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(WaitToSpin());
    }

    private IEnumerator WaitToSpin()
    {
        yield return new WaitForSeconds(timeToSpin);
        
        if(PhotonNetwork.IsMasterClient)
            animator.SetTrigger("Spin");
    }
}
