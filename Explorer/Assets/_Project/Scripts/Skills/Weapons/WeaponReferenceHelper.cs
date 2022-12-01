using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponReferenceHelper : MonoBehaviour
{
    public ConfigurableJoint configurableJoint;
    public WeaponFeedback weaponFeedback;
    public WeaponSetupHandler weaponSetupHandler;

    public void SetupHandAndJoint(Transform puppetHandTransform, Rigidbody puppetHandRigid, bool isLeft)
    {
        if (DebugLogger.IsNullError(weaponSetupHandler, this)) return;
        
        weaponSetupHandler.SetupHandAndJoint(puppetHandTransform, puppetHandRigid, configurableJoint, isLeft);
    }
}
