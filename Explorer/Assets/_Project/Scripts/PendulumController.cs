using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumController : MonoBehaviour
{
    public List<Animator> animators = new List<Animator>();
    private float timeToWait = 0;
    public float minTime;
    public float maxTime; 
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartAnimators());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StartAnimators()
    {
        foreach (Animator animator in animators)
        {
            timeToWait = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(timeToWait);
            animator.enabled = true;
        }
    }
}
