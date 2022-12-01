using System;
using System.Collections;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemySpawnerStructure : PooledRigidbody
{
    [SerializeField] private GameObject activationFx;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private DissolveEffect dissolveEffect;
    [SerializeField] private SimpleAudioEvent explosionAudioEvent;
    [SerializeField] private SimpleAudioEvent loopingAudioEvent;

    private EnemyWaveGenerator _enemyWaveGenerator;

    private const int LOOP_AUDIO_DELAY = 3;


    protected override void OnEnable()
    {
        base.OnEnable();
        
        if (DebugLogger.IsNullError(dissolveEffect, "Must be set in editor.", this)) return;
        
        dissolveEffect.FinishedAppearing += Activate;
        dissolveEffect.FinishedDissolving += Despawn;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        dissolveEffect.FinishedAppearing = null;
        dissolveEffect.FinishedDissolving = null;
        
        Deactivate();
    }

    [Button]
    public override void PopulateParameters()
    {
        base.PopulateParameters();
        
        if (!dissolveEffect)
        {
            dissolveEffect = GetComponent<DissolveEffect>();
            if (!dissolveEffect) dissolveEffect = ThisGameObject.AddComponent<DissolveEffect>();
        }

        if (!audioSource) audioSource = GetComponentInChildren<AudioSource>();
    }

    public void SetEnemyWaveGenerator(EnemyWaveGenerator enemyWaveGenerator)
    {
        _enemyWaveGenerator = enemyWaveGenerator;
    }
    
    private void Activate()
    {
        if (PhotonNetwork.IsMasterClient) StartSpawning();

        if (DebugLogger.IsNullError(activationFx, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(audioSource, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(explosionAudioEvent, this, "Must be set in editor.")) return;
        
        activationFx.SetActive(true);
        
        audioSource.loop = false;
        explosionAudioEvent.Play(audioSource);
        
        CoroutineCaller.Instance.StartCoroutine(LoopAudioCoroutine());
    }

    private void StartSpawning()
    {
        if (DebugLogger.IsNullError(_enemyWaveGenerator, this, "Must be set in editor.")) return;

        _enemyWaveGenerator.StartSpawning();
    }

    private IEnumerator LoopAudioCoroutine()
    {
        if (DebugLogger.IsNullError(loopingAudioEvent, this, "Must be set in editor.")) yield break;

        yield return new WaitForSeconds(LOOP_AUDIO_DELAY);
        
        audioSource.loop = true;
        loopingAudioEvent.Play(audioSource);
    }
    
    public void Deactivate()
    {
        activationFx.SetActive(false);
        
        audioSource.Stop();

        if (PhotonNetwork.IsMasterClient) StopSpawningSpawning();
    }
    
    private void StopSpawningSpawning()
    {
        if (_enemyWaveGenerator) _enemyWaveGenerator.Deactivate();
    }
}
