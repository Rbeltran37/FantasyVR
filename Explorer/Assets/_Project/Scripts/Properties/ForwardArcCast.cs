using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

//TODO Unfinished, Range has no effect on arc distance
[CreateAssetMenu(fileName = "ForwardArcCast_TargetAcquisition", menuName = "ScriptableObjects/TargetAcquisition/ForwardArcCast", order = 1)]
public class ForwardArcCast : TargetAcquisition
{
	public float Gravity = -9.8f;
    [Tooltip("The AimStep is the how much to subdivide the iteration.")]
    [Range(0.001f, 1.0f)]
    public float AimStep = 1;

    private const float DISTANCE_BUFFER = .01f;


    public override Transform AcquireTarget(Transform transform)
    {
        RaycastHit hit;
        var aimPosition = transform.position;
        var aimDirection = transform.forward;
        Ray startRay = new Ray(aimPosition, aimDirection);
        var rangeSquared = Range * Range;
        do
        {
	        if (Physics.Raycast(aimPosition, aimDirection, out hit, AimStep, TargetLayerMask, QueryTriggerInteraction))
	        {
		        var target = hit.transform;
		        return target;
	        }
	        
	        var aimVector = aimDirection;
	        aimVector.y = aimVector.y + Gravity * 0.0111111111f * AimStep;
	        aimDirection = aimVector;
	        aimPosition += aimVector * AimStep;

        } while ((aimPosition.y - startRay.origin.y > -100) && ((startRay.origin - aimPosition).sqrMagnitude <= rangeSquared));

        return null;
    }
    
    public override Vector3 AcquireHitPoint(Transform transform)
    {
	    RaycastHit hit;
	    var aimPosition = transform.position;
	    var aimDirection = transform.forward;
	    Ray startRay = new Ray(aimPosition, aimDirection);
	    var adjustedStep = AimStep * 2;
	    var rangeSquared = Range * Range;
	    do
	    {
		    if (Physics.Raycast(aimPosition, aimDirection, out hit, adjustedStep, TargetLayerMask, QueryTriggerInteraction))
		    {
			    return hit.point;
		    }
	        
		    var aimVector = aimDirection;
		    aimVector.y = aimVector.y + Gravity * 0.0111111111f * AimStep;
		    aimDirection = aimVector;
		    aimPosition += aimVector * AimStep;
	    } while ((aimPosition.y - startRay.origin.y > -100) && ((startRay.origin - aimPosition).magnitude <= rangeSquared));

	    return Vector3.zero;
    }
}
