using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class OculusInput : MonoBehaviour
{
    public BowInstance mBowInstance = null;
    public GameObject m_OppositeController = null;
    public OVRInput.Controller m_Controller = OVRInput.Controller.None;

    private void Update()
    {
        if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, m_Controller))
            mBowInstance.Pull(m_OppositeController.transform);

        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, m_Controller))
            mBowInstance.Release();
    }
}
