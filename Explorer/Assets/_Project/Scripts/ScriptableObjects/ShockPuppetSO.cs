using System.Collections;
using RootMotion.Dynamics;
using UnityEngine;

[CreateAssetMenu(fileName = "ShockPuppet", menuName = "ScriptableObjects/Effects/ShockPuppet", order = 1)]
public class ShockPuppetSO : ScriptableObject
{
    public float shockLifetime = 2;
    [Range(.1f, 1)] public float shockInterval = .1f;
    [MinMaxFloatRange(0, 1000)] public RangedFloat shockForce;
    public float shockUnpin = 25;

    
    public void ApplyShock(MuscleCollisionBroadcaster muscleCollisionBroadcaster)
    {
        CoroutineCaller.Instance.StartCoroutine(ApplyShockCoroutine(muscleCollisionBroadcaster));
    }

    private IEnumerator ApplyShockCoroutine(MuscleCollisionBroadcaster muscleCollisionBroadcaster)
    {
        var puppetMaster = muscleCollisionBroadcaster.puppetMaster;
        if (!puppetMaster) yield break;

        var timeShocked = 0f;
        while (puppetMaster && timeShocked < shockLifetime)
        {
            foreach (var muscle in puppetMaster.muscles)
            {
                ShockMuscle(muscle);
            }

            timeShocked += shockInterval;
            yield return new WaitForSeconds(shockInterval);
        }
    }

    private void ShockMuscle(Muscle muscle)
    {
        if (muscle == null) return;
        
        var randomVector = Random.onUnitSphere;
        var randomForce = Random.Range(shockForce.minValue, shockForce.maxValue);
        var position = muscle.targetMappedPosition;
        muscle.broadcaster.Hit(shockUnpin, randomVector * randomForce, position);
    }
}