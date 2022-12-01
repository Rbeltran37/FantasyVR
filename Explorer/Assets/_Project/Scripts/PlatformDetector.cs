using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.Demos;
using RootMotion.FinalIK;
using UnityEngine;

public class PlatformDetector : MonoBehaviour
{
    private RaycastHit hit;
    public VRIKPlatformController platformController;
    public bool hasFoundPlatform = false;
    public VRIK ik;
    public Transform trackingSpace;
    public Transform raycastOrigin;
    public LayerMask layerMask = new LayerMask();
    public float raycastLength;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Physics.Raycast(raycastOrigin.position, Vector3.down, out hit, layerMask)) 
        {
            //Debug.DrawLine (raycastOrigin.position, hit.point, Color.white);
            if (hit.collider.CompareTag("Platform"))
            {
                if (!hasFoundPlatform)
                {
                    platformController = hit.transform.GetComponent<VRIKPlatformController>();
                    //Debug.Log("Enable platform controller.");
                    //enable platform controller
                    platformController.enabled = true;
                    hasFoundPlatform = true;
                }

                return;
            }

            if (hasFoundPlatform)
            {
                //Debug.Log("Disable platform controller.");
                ik.transform.parent = null;
                trackingSpace.parent = null;
                
                platformController.enabled = false;
                hasFoundPlatform = false;
            }
        }
    }
}
