using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class DissolveEffect : MonoBehaviour
{
    [SerializeField] private bool appearOnEnable;
    [SerializeField] private DissolveEffectSO dissolveEffectSo;
    [Tooltip("Render groups that will have a dissolve material applied to them.")]
    [SerializeField] private List<DissolveGroup> dissolveGroups = new List<DissolveGroup>();
    
    [Space(10)] [Header("Optional")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SimpleAudioEvent simpleAudioEvent;

    private bool _isInitialized;
    private float _currentDissolveValue;
    private Coroutine _coroutine;

    public Action FinishedDissolving;
    public Action FinishedAppearing;

    [SerializableAttribute] internal class DissolveGroup
    {
        [Tooltip("Specific Dissolve Material that is being placed on renderers")]
        [SerializeField] private Material customDissolveMaterial;
        [Tooltip("Renderers that will have Dissolve Effect applied to them")]
        [SerializeField] private List<Renderer> renderersToDissolve = new List<Renderer>();
        
        private Material _materialInstance;
        private Material[] _resetMaterialInstances;

        internal void PopulateRenderers(GameObject gameObjectToDestroy)
        {
            renderersToDissolve = new List<Renderer>();
            
            var potentialRenderers = gameObjectToDestroy.GetComponentsInChildren<Renderer>();
            foreach (var potentialRenderer in potentialRenderers)
            {
                if (potentialRenderer is MeshRenderer ||
                    potentialRenderer is SkinnedMeshRenderer)
                {
                    renderersToDissolve.Add(potentialRenderer);
                }
            }
        }

        internal void Initialize()
        {
            SetResetMaterialInstance();
            InstantiateMaterial();
        }

        internal void SetResetMaterialInstance()
        {
            foreach (var renderer in renderersToDissolve)
            {
                if (renderer)
                {
                    var count = renderer.materials.Length;
                    _resetMaterialInstances = new Material[count];
                    for (var index = 0; index < count; index++)
                    {
                        var currentMaterial = renderer.materials[index];
                        if (DebugLogger.IsNullError(currentMaterial, $"{nameof(currentMaterial)} is null.")) return;
                        
                        _resetMaterialInstances[index] = currentMaterial;
                    }
                }
            }
        }

        internal void InstantiateMaterial()
        {
            _materialInstance = new Material(customDissolveMaterial);
        }

        internal void ReplaceMaterialsInRenderers()
        {
            foreach (var renderer in renderersToDissolve)
            {
                if (!renderer) continue;

                var count = renderer.materials.Length;
                var dissolveMaterials = new Material[count];
                for (var index = 0; index < count; index++)
                {
                    dissolveMaterials[index] = _materialInstance;
                }

                renderer.materials = dissolveMaterials;
            }
        }

        internal void Reset(int dissolveId)
        {
            ResetMaterialsInRenderers();
            SetDissolveValue(0, dissolveId);
        }
        
        internal void ResetMaterialsInRenderers()
        {
            if (_resetMaterialInstances == null)
            {
                DebugLogger.Error(MethodBase.GetCurrentMethod().Name, $"{nameof(_resetMaterialInstances)} is null.");
                return;
            }
            
            foreach (var renderer in renderersToDissolve)
            {
                if (!renderer)
                {
                    DebugLogger.Error(MethodBase.GetCurrentMethod().Name, $"{nameof(renderer)} is null.");
                    return;
                }
                
                if (renderer.materials == null)
                {
                    DebugLogger.Error(MethodBase.GetCurrentMethod().Name, $"{nameof(renderer.materials)} is null.");
                    return;
                }

                renderer.materials = _resetMaterialInstances;
            }
        }

        internal void SetDissolveValue(float newValue, int dissolveId)
        {
            _materialInstance.SetFloat(dissolveId, newValue);
        }
    }


    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        if (appearOnEnable) Appear();
    }

    private void OnDisable()
    {
        if (_coroutine != null) CoroutineCaller.Instance.StopCoroutine(_coroutine);
        _coroutine = null;
    }

    [Button]
    public void PopulateParameters()
    {
        foreach (var dissolveGroup in dissolveGroups)
        {
            if (DebugLogger.IsNullError(dissolveGroup, this)) return;
            
            dissolveGroup.PopulateRenderers(gameObject);
        }
    }

    public void ResetObject()
    {
        if (_coroutine != null) CoroutineCaller.Instance.StopCoroutine(_coroutine);
        _coroutine = null;
        
        FinishedDissolving = null;
        
        if (_isInitialized) ResetMaterials();
    }

    private void ResetMaterials()
    {
        foreach (var dissolveGroup in dissolveGroups)
        {
            dissolveGroup.Reset(dissolveEffectSo.DissolveId);
        }
    }

    private void Initialize()
    {
        if (_isInitialized) return;
        
        _isInitialized = true;

        foreach (var dissolveGroup in dissolveGroups)
        {
            dissolveGroup.Initialize();
        }

        if (DebugLogger.IsNullError(dissolveEffectSo, this, "Must be set in editor.")) return;
        
        dissolveEffectSo.SetNameIds();
    }

    [Button]
    public void Dissolve()
    {
        if (!gameObject.activeSelf) return;
        
        if (_coroutine != null) CoroutineCaller.Instance.StopCoroutine(_coroutine);
        _coroutine = CoroutineCaller.Instance.StartCoroutine(DissolveCoroutine());
    }

    private IEnumerator DissolveCoroutine()
    {
        yield return new WaitForSeconds(dissolveEffectSo.dissolveDelay);
        
        if (simpleAudioEvent) simpleAudioEvent.Play(audioSource);
        
        SetDissolveMaterials();
        
        CoroutineCaller.Instance.StartCoroutine(DissolveMaterialsCoroutine());
    }

    private void SetDissolveMaterials()
    {
        foreach (var dissolveGroup in dissolveGroups)
        {
            dissolveGroup.ReplaceMaterialsInRenderers();
        }
    }

    private IEnumerator DissolveMaterialsCoroutine()
    {
        _currentDissolveValue = dissolveEffectSo.MinDissolveValue;
        while (_currentDissolveValue < dissolveEffectSo.MaxDissolveValue)
        {
            _currentDissolveValue += dissolveEffectSo.dissolveSpeed;
            foreach (var dissolveGroup in dissolveGroups)
            {
                dissolveGroup.SetDissolveValue(_currentDissolveValue, dissolveEffectSo.DissolveId);
            }
            yield return new WaitForSeconds(dissolveEffectSo.dissolveTimeStep);
        }

        foreach (var dissolveGroup in dissolveGroups)
        {
            dissolveGroup.SetDissolveValue(dissolveEffectSo.MaxDissolveValue, dissolveEffectSo.DissolveId);
        }
        
        if (simpleAudioEvent) simpleAudioEvent.Stop(audioSource);
        
        FinishedDissolving?.Invoke();
        FinishedDissolving = null;
    }
    
    [Button]
    public void Appear()
    {
        if (!gameObject.activeSelf) return;
        
        SetDissolveMaterials();
        foreach (var dissolveGroup in dissolveGroups)
        {
            dissolveGroup.SetDissolveValue(dissolveEffectSo.MaxDissolveValue, dissolveEffectSo.DissolveId);
        }

        if (_coroutine != null) CoroutineCaller.Instance.StopCoroutine(_coroutine);
        _coroutine = CoroutineCaller.Instance.StartCoroutine(AppearCoroutine());
    }
    
    private IEnumerator AppearCoroutine()
    {
        yield return null;
        
        if (simpleAudioEvent) simpleAudioEvent.Play(audioSource);
        
        SetDissolveMaterials();
        
        CoroutineCaller.Instance.StartCoroutine(AppearMaterialsCoroutine());
    }

    private IEnumerator AppearMaterialsCoroutine()
    {
        _currentDissolveValue = dissolveEffectSo.MaxDissolveValue;
        while (_currentDissolveValue > dissolveEffectSo.MinDissolveValue)
        {
            _currentDissolveValue -= dissolveEffectSo.appearSpeed;
            foreach (var dissolveGroup in dissolveGroups)
            {
                dissolveGroup.SetDissolveValue(_currentDissolveValue, dissolveEffectSo.DissolveId);
            }
            yield return new WaitForSeconds(dissolveEffectSo.appearTimeStep);
        }

        foreach (var dissolveGroup in dissolveGroups)
        {
            dissolveGroup.SetDissolveValue(dissolveEffectSo.MinDissolveValue, dissolveEffectSo.DissolveId);
        }

        if (simpleAudioEvent) simpleAudioEvent.Stop(audioSource);

        ResetMaterials();
        
        FinishedAppearing?.Invoke();
        FinishedAppearing = null;
    }
}
