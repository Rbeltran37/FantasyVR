using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMouth : MonoBehaviour
{
    [SerializeField] private AudioSource mouthAudioSource;
    [SerializeField] private Health health;
    [SerializeField] private ManaPool manaPool;
    [SerializeField] private SimpleAudioEvent gulpAudioEvent;

    private Coroutine _gulpCoroutine;

    private const int GULP_WAIT = 1;

    
    public void AddHealth(float amount)
    {
        if (DebugLogger.IsNullError(health, this, "Must be set in editor.")) return;
        
        health.Add(amount);
        PlayGulpAudio();
    }
    
    public void AddMana(float amount)
    {
        if (DebugLogger.IsNullError(manaPool, this, "Must be set in editor.")) return;
        
        manaPool.Add(amount);
        PlayGulpAudio();
    }

    private void PlayGulpAudio()
    {
        if (_gulpCoroutine != null) return;

        _gulpCoroutine = CoroutineCaller.Instance.StartCoroutine(GulpCoroutine());
    }

    private IEnumerator GulpCoroutine()
    {
        if (DebugLogger.IsNullError(gulpAudioEvent, this, "Must be set in editor.")) yield break;

        gulpAudioEvent.Play(mouthAudioSource);

        yield return new WaitForSeconds(GULP_WAIT);

        _gulpCoroutine = null;
    }
}
