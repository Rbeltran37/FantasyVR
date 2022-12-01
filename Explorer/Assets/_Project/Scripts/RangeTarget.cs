using System.Collections;
using UnityEngine;

public class RangeTarget : MonoBehaviour
{
    public Animator animator;
    public Health health;
    private const float MIN_TIME_DOWN = 1f;
    private const float MIN_TIME_UP = 3f;
    private const float MAX_TIME_DOWN = 3f;
    private const float MAX_TIME_UP = 5f;

    private void Awake()
    {
        health.WasHit += TargetDown;
        
        //TargetDown();
    }

    private void OnDisable()
    {
        health.WasHit -= TargetDown;
    }

    private void TargetDown(float value = 0)
    {
        animator.SetBool("HasHit", true);
        //Reset();
    }

    public void Reset()
    {
        health.ResetObject();
    }
    
    private void TargetUp()
    {
        animator.SetBool("HasHit", false);
        health.WasHit += TargetDown;
    }

    public void CallTargetUp(float timeToWait)
    {
        StartCoroutine(RandomizeTimeUp(timeToWait));
    }

    private IEnumerator RandomizeTimeUp(float timeToWait)
    {
        var time = 0f;

        TargetUp();

        while (time < timeToWait)
        {
            time += Time.deltaTime;
            yield return null;
        }
        
        //TargetDown();
    }
}
