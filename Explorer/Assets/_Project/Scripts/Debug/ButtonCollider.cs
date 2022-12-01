using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonCollider : MonoBehaviour
{
    [SerializeField] private CustomEnums.SearchParameter searchType;
    [SerializeField] private string targetName;
    [SerializeField] private string targetTag;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private string targetComponent;

    private Dictionary<CustomEnums.SearchParameter, Action<Transform>> _searchDictionary = new Dictionary<CustomEnums.SearchParameter, Action<Transform>>();


    private void Awake()
    {
        _searchDictionary.Add(CustomEnums.SearchParameter.Name, SearchName);
        _searchDictionary.Add(CustomEnums.SearchParameter.Tag, SearchTag);
        _searchDictionary.Add(CustomEnums.SearchParameter.Layer, SearchLayer);
        _searchDictionary.Add(CustomEnums.SearchParameter.Component, SearchComponent);
    }


    [System.Serializable]
    public class ButtonEvent : UnityEvent { }
    
    public ButtonEvent OnButtonPressed;


    private void OnCollisionEnter(Collision other)
    {
        _searchDictionary[searchType]?.Invoke(other.transform);
    }

    private void SearchName(Transform other)
    {
        if (other.name.Contains(targetName))
        {
            OnButtonPressed.Invoke();
        }
    }

    private void SearchTag(Transform other)
    {
        if (other.CompareTag(targetTag))
        {
            OnButtonPressed?.Invoke();
        }
    }

    private void SearchLayer(Transform other)
    {
        if (IsLayerInTargetLayers(other))
        {
            OnButtonPressed?.Invoke();
        }
    }

    private void SearchComponent(Transform other)
    {
        //TODO
    }
    
    private bool IsLayerInTargetLayers(Transform other)
    {
        return (targetLayers.value & 1 << other.gameObject.layer) > 0;
    }
}
