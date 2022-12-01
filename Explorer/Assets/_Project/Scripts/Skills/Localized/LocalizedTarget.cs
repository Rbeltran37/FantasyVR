using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizedTarget : PooledObject
{
    [SerializeField] private GameObject castingModel;
    [SerializeField] private GameObject castModel;


    protected override void OnEnable()
    {
        base.OnEnable();
        
        castingModel.SetActive(true);
        castModel.SetActive(false);
    }

    public void CanCast()
    {
        castingModel.SetActive(false);
        castModel.SetActive(true);
    }
}
