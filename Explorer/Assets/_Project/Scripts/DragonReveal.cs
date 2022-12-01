using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonReveal : MonoBehaviour
{

    public Transform dragon;
    public Transform startPos;
    public Transform endPos;
    public float speed = 1f;
    public AudioSource dragonBreathingAudio;
    public AudioSource dragonWalkingAudio;
    public AudioClip fireShot;
    public GameObject fireBreath;
    public Animator animator;
    private static readonly int _IsBreathingFire = Animator.StringToHash("isBreathingFire");

    public IEnumerator MoveFromTo() {
        dragonBreathingAudio.Play();
        dragonWalkingAudio.Play();
        
        float step = (speed / (startPos.position - endPos.position).magnitude) * Time.fixedDeltaTime;
        float t = 0;
        while (t <= 1.0f) {
            t += step; // Goes from 0 to 1, incrementing by step each time
            dragon.position = Vector3.Lerp(startPos.position, endPos.position, Mathf.SmoothStep(0.0f, 1.0f, t)); // Move objectToMove closer to b
            yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
        }
        dragon.position = endPos.position;
    }

    public IEnumerator StartFireBreath()
    {
        animator.SetBool(_IsBreathingFire, true);
        dragonBreathingAudio.clip = fireShot;
        dragonBreathingAudio.loop = false;
        dragonBreathingAudio.volume = 1f; 
        dragonBreathingAudio.Play();
        yield return null;
    }

    public void EnableFireBreathGameObject()
    {
        DebugLogger.Info("Enabling fire now.");
        fireBreath.SetActive(true);
    }
}
