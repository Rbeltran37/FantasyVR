using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO Unfinished
[CreateAssetMenu(fileName = "ForwardArcCastOld_TargetAcquisition", menuName = "ScriptableObjects/TargetAcquisition/ForwardArcCastOld", order = 1)]
public class ForwardArcCastOld : TargetAcquisition
{
	[Tooltip("The MinimumElevation is relative to the AimPosition.")]
    public float MinimumElevation = -100;
    [Tooltip("The Gravity is used in conjunction with AimVelocity and the aim direction to simulate a projectile.")]
    public float Gravity = -9.8f;

    [Tooltip("The AimVelocity is the initial speed of the faked projectile.")]
    [Range(0.001f, 50.0f)]
    public float AimVelocity = 1;

    [Tooltip("The AimStep is the how much to subdivide the iteration.")]
    [Range(0.001f, 1.0f)]
    public float AimStep = 1;


    public override Transform AcquireTarget(Transform transform)
    {
        RaycastHit hit;
        var aimPosition = transform.position;
        var aimDirection = transform.forward * AimVelocity;
        Ray startRay = new Ray(aimPosition, aimDirection);
        
        var rangeSquared = Range * Range;
        do
        {
	        if (Physics.Raycast(aimPosition, aimDirection, out hit, AimStep, TargetLayerMask))
	        {
		        var target = hit.transform;
		        return target;
	        }
	        
	        var aimVector = aimDirection;
	        aimVector.y = aimVector.y + Gravity * 0.0111111111f * AimStep;
	        aimDirection = aimVector;
	        aimPosition += aimVector * AimStep;

        } while ((aimPosition.y - startRay.origin.y > MinimumElevation) && ((startRay.origin - aimPosition).sqrMagnitude <= rangeSquared));

        return null;
    }
    
    public override Vector3 AcquireHitPoint(Transform transform)
    {
	    RaycastHit hit;
	    var aimPosition = transform.position;
	    var aimDirection = transform.forward * AimVelocity;
	    Ray startRay = new Ray(aimPosition, aimDirection);
        
	    var rangeSquared = Range * Range;
	    do
	    {
		    if (Physics.Raycast(aimPosition, aimDirection, out hit, Range / AimStep, TargetLayerMask))
		    {
			    return hit.point;
		    }
	        
		    var aimVector = aimDirection;
		    aimVector.y = aimVector.y + Gravity * 0.0111111111f * AimStep;
		    aimDirection = aimVector;
		    aimPosition += aimVector * AimStep;

	    } while ((aimPosition.y - startRay.origin.y > MinimumElevation) && ((startRay.origin - aimPosition).sqrMagnitude <= rangeSquared));

	    return Vector3.zero;
    }
}
