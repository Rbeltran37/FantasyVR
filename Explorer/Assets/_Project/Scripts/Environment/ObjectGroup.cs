using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ObjectGroup : MonoBehaviour
{
    [SerializeField] private List<ArenaObject> arenaObjects = new List<ArenaObject>();

    private int _objectsActivated;
    private int _objectsDeactivated;

    public Action HasBeenActivated;
    public Action HasBeenDeactivated;


    public void Enable()
    {
        foreach (var arenaObject in arenaObjects)
        {
            arenaObject.gameObject.SetActive(true);
        }
    }
    
    public void Disable()
    {
        foreach (var arenaObject in arenaObjects)
        {
            arenaObject.gameObject.SetActive(false);
        }
    }
    
    public virtual void Activate()
    {
        foreach (var arenaObject in arenaObjects)
        {
            _objectsActivated++;
            StartCoroutine(arenaObject.Activate(RegisterActivation));
        }
    }

    public virtual void Deactivate()
    {
        foreach (var arenaObject in arenaObjects)
        {
            _objectsDeactivated++;
            StartCoroutine(arenaObject.Deactivate(RegisterDeactivation));
        }
    }
    
    [Button]
    public void SetGroupToActivatedPosition()
    {
        foreach (var arenaObject in arenaObjects)
        {
            arenaObject.SetToActivatedPosition();
        }
    }
    
    [Button]
    public void SetGroupToDeactivatedPosition()
    {
        foreach (var arenaObject in arenaObjects)
        {
            arenaObject.SetToDeactivatedPosition();
        }
    }
    
    [Button]
    public void PopulateParameters()
    {
        arenaObjects = new List<ArenaObject>();
        var tempArenaObjects = gameObject.GetComponentsInChildren<ArenaObject>();
        foreach (var arenaObject in tempArenaObjects)
        {
            arenaObject.PopulateParameters();
            arenaObjects.Add(arenaObject);
        }
    }

    private void RegisterActivation()
    {
        _objectsActivated--;

        if (_objectsActivated <= 0)
        {
            _objectsActivated = 0;

            HasBeenActivated?.Invoke();
        }
    }
    
    private void RegisterDeactivation()
    {
        _objectsDeactivated--;

        if (_objectsDeactivated <= 0)
        {
            _objectsDeactivated = 0;

            HasBeenDeactivated?.Invoke();
        }
    }
}
