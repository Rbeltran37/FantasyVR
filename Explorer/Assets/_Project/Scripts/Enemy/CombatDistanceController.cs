using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatDistanceController : MonoBehaviour
{
    [SerializeField] private EnemyTargetAcquisition enemyTargetAcquisition;
    [SerializeField] private Animator animator;
    [SerializeField] private float rootMotionStopTime = .2f;
    
    [SerializeField] private float step = .1f;


    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Player")) StartCoroutine(StopRootMotion());
    }

    public void StartLeap(float seconds)
    {
        var playerModelHipsTransform = enemyTargetAcquisition.GetCurrentTarget().GetModelHips();

        StartCoroutine(MoveToTargetCoroutine(playerModelHipsTransform.position));
    }

    public void EndLeap()
    {
        animator.applyRootMotion = true;
    }

    private IEnumerator MoveToTargetCoroutine(Vector3 targetPosition)
    {
        animator.applyRootMotion = false;

        while (Vector3.Distance(animator.rootPosition, targetPosition) > 1)
        {
            Vector3.MoveTowards(animator.rootPosition, targetPosition, step);
            yield return new WaitForFixedUpdate();
        }

        animator.applyRootMotion = true;
    }

    private IEnumerator StopRootMotion()
    {
        animator.applyRootMotion = false;
        
        yield return new WaitForSeconds(rootMotionStopTime);

        animator.applyRootMotion = true;
    }
}
