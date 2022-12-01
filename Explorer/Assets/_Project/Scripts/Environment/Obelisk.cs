using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

public class Obelisk : PooledObject
{
    [SerializeField] private GameObject activationFx;
    [SerializeField] private GameObject[] spawnFxParents;
    [SerializeField] private Transform[] spawnParents;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator animator;
    [SerializeField] private DissolveEffect dissolveEffect;
    [SerializeField] private PUNPlayerTargetManager punPlayerTargetManager;
    [SerializeField] private SimpleAudioEvent explosionAudioEvent;
    [SerializeField] private SimpleAudioEvent loopingAudioEvent;

    private int _isOpenId;
    
    private const int DEFAULT_NUM_PLAYERS = 4;
    private const int REWARD_FX_DELAY = 3;
    private const string IS_OPEN = "isOpen";


    protected override void Awake()
    {
        base.Awake();
        
        _isOpenId = Animator.StringToHash(IS_OPEN);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        if (DebugLogger.IsNullError(dissolveEffect, "Must be set in editor.", this)) return;
        
        dissolveEffect.FinishedAppearing += Activate;
        dissolveEffect.FinishedDissolving += Despawn;
        
        if (DebugLogger.IsNullWarning(punPlayerTargetManager, this, "Should be set in editor. Attempting to find."))
        {
            punPlayerTargetManager = FindObjectOfType<PUNPlayerTargetManager>();
            if (DebugLogger.IsNullError(punPlayerTargetManager, this, "Should be set in editor. Unable to find.")) return;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        animator.SetBool(_isOpenId, false);

        Deactivate();
    }

    [Button]
    public override void PopulateParameters()
    {
        base.PopulateParameters();
        
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!audioSource)
        {
            audioSource = GetComponentInChildren<AudioSource>();
        }
        if (!dissolveEffect)
        {
            dissolveEffect = GetComponent<DissolveEffect>();
            if (!dissolveEffect) dissolveEffect = ThisGameObject.AddComponent<DissolveEffect>();
        }
        if (!punPlayerTargetManager) punPlayerTargetManager = FindObjectOfType<PUNPlayerTargetManager>();
    }

    public Transform[] GetSpawnParents()
    {
        return spawnParents;
    }

    private void Activate()
    {
        if (DebugLogger.IsNullError(animator, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(activationFx, this, "Must be set in editor.")) return;

        activationFx.SetActive(true);
        
        animator.SetBool(_isOpenId, true);

        audioSource.loop = false;
        explosionAudioEvent.Play(audioSource);
        
        CoroutineCaller.Instance.StartCoroutine(ActivateRewardFxCoroutine());
    }
    
    public void Deactivate()
    {
        activationFx.SetActive(false);
        
        audioSource.Stop();

        SetRewardFx(false);
        
        dissolveEffect.Dissolve();
    }

    private IEnumerator ActivateRewardFxCoroutine()
    {
        yield return new WaitForSeconds(REWARD_FX_DELAY);
        
        audioSource.loop = true;
        loopingAudioEvent.Play(audioSource);
        
        SetRewardFx(true);
    }

    private void SetRewardFx(bool state)
    {
        var spawnFxParentCount = spawnFxParents.Length;
        var numPlayers = punPlayerTargetManager ? punPlayerTargetManager.GetNumPlayers() : DEFAULT_NUM_PLAYERS;
        if (spawnFxParentCount < numPlayers)
        {
            DebugLogger.Debug(MethodBase.GetCurrentMethod().Name,
                $"{nameof(spawnFxParentCount)}={spawnFxParentCount} < {nameof(numPlayers)}={numPlayers}");
            return;
        }

        for (var playerIndex = 0; playerIndex < numPlayers; playerIndex++)
        {
            var spawnFxParent = spawnFxParents[playerIndex];
            var childCount = spawnFxParent.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var childFx = spawnFxParent.transform.GetChild(i).gameObject;
                childFx.SetActive(state);
            }
        }
    }
}
