using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleActivator : MonoBehaviour
{
    private const string DESTRUCTIBLE = "Destructible";
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag(DESTRUCTIBLE))
        {
            var rb = other.transform.GetComponent<Rigidbody>();
            if(rb)
                rb.isKinematic = false;
        }
        
    }
}
