using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class DestructibleContainer : MonoBehaviour
{
    [SerializeField] private Transform demolishOrigin;
    [SerializeField] private float demolishForce = 100;
    [SerializeField] private List<Destructible> destructibles;


    private void OnDisable()
    {
        ResetObject();
    }

    [Button]
    public void PopulateParameters()
    {
        var destructibleArray = GetComponentsInChildren<Destructible>();
        destructibles = destructibleArray.ToList();
        foreach (var destructible in destructibles)
        {
            if (destructibles.Contains(destructible)) continue;
            
            destructibles.Add(destructible);
            destructible.PopulateParameters();      //TODO may need to be called on enable
        }

        if (!demolishOrigin) demolishOrigin = transform;
    }

    [Button]
    public void ResetObject()
    {
        foreach (var destructible in destructibles)
        {
            destructible.ResetObject();
        }
    }

    public void DemolishAll()
    {
        foreach (var destructible in destructibles)
        {
            destructible.Demolish(demolishOrigin.position, demolishForce);
        }
    }
}
