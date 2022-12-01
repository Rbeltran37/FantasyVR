using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ShockEffect : MonoBehaviour
{
    [SerializeField] private ShockEffectSO shockEffectSo;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private PuppetDeathHandler puppetDeathHandler;
    
    
    private Material _materialInstance;
    private bool _isShocked;

    private const float TOLERANCE = .01f;
    
    private void Start()
    {
        if (shockEffectSo)
        {
            shockEffectSo.SetNameIds();
            _materialInstance = new Material(shockEffectSo.shockEffectMaterial);
        }

        if (puppetDeathHandler)
            puppetDeathHandler.WasTurnedOff += EndShock;
    }

    private void OnDestroy()
    {
        if (puppetDeathHandler)
            puppetDeathHandler.WasTurnedOff -= EndShock;
    }

    public void ApplyShockEffect(ShockPuppetSO shockPuppetSo)
    {
        if (!shockPuppetSo || !shockEffectSo) return;
        
        StartCoroutine(ApplyShockEffectCoroutine(shockPuppetSo));
    }

    private IEnumerator ApplyShockEffectCoroutine(ShockPuppetSO shockPuppetSo)
    {
        if (!_isShocked)
        {
            StartShock();
        }
        
        //Add shock
        var currentShock = _materialInstance.GetFloat(shockEffectSo.amountId);
        _materialInstance.SetFloat(shockEffectSo.amountId, currentShock + shockEffectSo.electricityAmount);

        var shockIncrement = shockPuppetSo.shockInterval * shockEffectSo.electricityAmount / shockPuppetSo.shockLifetime;
        float shockAmountApplied = 0;
        while (_isShocked && shockAmountApplied < shockEffectSo.electricityAmount)    //Subtract shock
        {
            currentShock = _materialInstance.GetFloat(shockEffectSo.amountId);
            _materialInstance.SetFloat(shockEffectSo.amountId, currentShock - shockIncrement);

            yield return new WaitForSeconds(shockPuppetSo.shockInterval);
            shockAmountApplied += shockIncrement;
        }
        
        currentShock = _materialInstance.GetFloat(shockEffectSo.amountId);

        if (Math.Abs(currentShock - shockEffectSo.electricityAmountMin) < TOLERANCE) EndShock();
    }

    private void StartShock()
    {
        _isShocked = true;
        ApplyShockMaterial();
    }

    private void EndShock()
    {
        RemoveShockMaterial();
        _isShocked = false;
    }

    [Button]
    private void ApplyShockMaterial()
    {
        var materialsOnRenderer = skinnedMeshRenderer.materials;
        var materialList = new List<Material>();
        foreach (var material in materialsOnRenderer)
        {
            if (!material.shader.Equals(_materialInstance.shader)) materialList.Add(material);
        }
        materialList.Add(_materialInstance);
        
        skinnedMeshRenderer.materials = materialList.ToArray();
        _materialInstance.SetFloat(shockEffectSo.amountId, shockEffectSo.electricityAmountMin);
    }

    private void RemoveShockMaterial()
    {
        if (DebugLogger.IsNullError(shockEffectSo, this, "Must be set in editor.")) return;
        
        _materialInstance.SetFloat(shockEffectSo.amountId, shockEffectSo.electricityAmountMin);
        
        var materialsOnRenderer = skinnedMeshRenderer.materials;
        var materialList = new List<Material>();
        foreach (var material in materialsOnRenderer)
        {
            if (!material.shader.Equals(_materialInstance.shader)) materialList.Add(material);
        }
        
        skinnedMeshRenderer.materials = materialList.ToArray();
    }
}