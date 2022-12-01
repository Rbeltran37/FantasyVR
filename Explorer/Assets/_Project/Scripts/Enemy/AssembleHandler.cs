using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class AssembleHandler : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private DissolveEffect dissolveEffect;
    [SerializeField] private PuppetMaster puppetMaster;
    [SerializeField] private SimpleAI simpleAi;
    [SerializeField] private BehaviorTree behaviorTree;
    [SerializeField] private GameObject assembledModel;
    [SerializeField] private GameObject disassembledModel;
    [SerializeField] private Animator disassembledAnimator;

    private Transform _defaultDisassembledModelParent;
    private int _isAssembledId;

    private const string IS_ASSEMBLED = "isAssembled";
    private const float ASSEMBLE_TIME = 7.33f;


    private void Awake()
    {
        _isAssembledId = Animator.StringToHash(IS_ASSEMBLED);

        if (!disassembledModel)
        {
            DebugLogger.Error(nameof(Awake), $"{nameof(disassembledModel)} is null. Must be set in editor.", this);
            return;
        }

        _defaultDisassembledModelParent = disassembledModel.transform.parent;
    }

    private void OnEnable()
    {
        Assemble();
        
        if (!health)
        {
            DebugLogger.Error(nameof(OnEnable), $"{nameof(health)} is null. Must be set in editor.", this);
            return;
        }

        health.WasKilled += Disassemble;

        if (!dissolveEffect)
        {
            DebugLogger.Error(nameof(OnEnable), $"{nameof(dissolveEffect)} is null. Must be set in editor.", this);
            return;
        }

        dissolveEffect.FinishedDissolving += Disable;
    }

    private void Assemble()
    {
        ToggleEnemyActivation(false);
        
        StartCoroutine(AssembleCoroutine());
    }
    
    private void ToggleEnemyActivation(bool activate)
    {
        if (!simpleAi)
        {
            DebugLogger.Error(nameof(ToggleEnemyActivation), $"{nameof(simpleAi)} is null. Must be set in editor.", this);
            return;
        }
        
        simpleAi.enabled = activate;
        
        if (!behaviorTree)
        {
            DebugLogger.Error(nameof(ToggleEnemyActivation), $"{nameof(behaviorTree)} is null. Must be set in editor.", this);
            return;
        }
        
        behaviorTree.enabled = activate;
        
        if (!puppetMaster)
        {
            DebugLogger.Error(nameof(ToggleEnemyActivation), $"{nameof(puppetMaster)} is null. Must be set in editor.", this);
            return;
        }

        var puppetMode = activate ? PuppetMaster.Mode.Active : PuppetMaster.Mode.Disabled;
        puppetMaster.mode = puppetMode;
        
        ToggleModel(activate);
        
        if (activate) return;

        if (!disassembledAnimator)
        {
            DebugLogger.Error(nameof(ToggleEnemyActivation), $"{nameof(disassembledAnimator)} is null. Must be set in editor.", this);
            return;
        }
        
        disassembledAnimator.SetBool(_isAssembledId, true);
    }

    private IEnumerator AssembleCoroutine()
    {
        yield return new WaitForSeconds(ASSEMBLE_TIME);

        ToggleEnemyActivation(true);
    }

    private void ToggleModel(bool isAssembled)
    {
        if (!assembledModel)
        {
            DebugLogger.Error(nameof(ToggleModel), $"{nameof(assembledModel)} is null. Must be set in editor.", this);
            return;
        }
        
        assembledModel.SetActive(isAssembled);
        
        if (!disassembledModel)
        {
            DebugLogger.Error(nameof(ToggleModel), $"{nameof(disassembledModel)} is null. Must be set in editor.", this);
            return;
        }
        
        disassembledModel.SetActive(!isAssembled);
    }

    private void Disassemble()
    {
        ToggleModel(false);
        
        if (!disassembledModel)
        {
            DebugLogger.Error(nameof(Disassemble), $"{nameof(disassembledModel)} is null. Must be set in editor.", this);
            return;
        }
        
        disassembledModel.transform.SetParent(null);
        
        if (!disassembledAnimator)
        {
            DebugLogger.Error(nameof(Disassemble), $"{nameof(disassembledAnimator)} is null. Must be set in editor.", this);
            return;
        }
        
        disassembledAnimator.SetBool(_isAssembledId, false);
    }

    private void Disable()
    {
        if (!disassembledModel)
        {
            DebugLogger.Error(nameof(Disable), $"{nameof(disassembledModel)} is null. Must be set in editor.", this);
            return;
        }
        
        disassembledModel.transform.SetParent(_defaultDisassembledModelParent);
        disassembledModel.transform.position = _defaultDisassembledModelParent.position;
        disassembledAnimator.transform.rotation = _defaultDisassembledModelParent.rotation;
        disassembledModel.SetActive(false);
    }
}
