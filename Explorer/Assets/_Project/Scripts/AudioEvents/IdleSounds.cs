using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;
using Random = UnityEngine.Random;

public class IdleSounds : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private CharacterAudioManager characterAudioManager;
    [SerializeField] [MinMaxFloatRange(0, 10)] private RangedFloat delay;
    

    private const float IDLE_CHECK_INTERVAL = .5f;

    private void OnEnable()
    {
        StartCoroutine(CheckIfIdle());
    }

    private IEnumerator PlayIdleSoundCoroutine()
    {
        characterAudioManager.PlayIdleSound(audioSource);
        
        yield return new WaitForSeconds(audioSource.clip.length);

    }

    private IEnumerator CheckIfIdle()
    {
        if (!health || !audioSource)
        {
            Debug.Log("No health of audioSource given to AudioController script");
            yield break;
        }
        
        while (true)
        {
            yield return new WaitForSeconds(IDLE_CHECK_INTERVAL);
            if (!audioSource.isPlaying && 
                health.isAlive)
            {
                yield return PlayIdleSoundCoroutine();
                yield return new WaitForSeconds(Random.Range(delay.minValue, delay.maxValue));

            }
        }
    }
}
