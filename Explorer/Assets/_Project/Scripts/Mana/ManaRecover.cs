using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaRecover : MonoBehaviour
{
    [SerializeField] private ManaPool manaPool;
    [SerializeField] private float recoverAmount = 5;

    private bool _isActive = true;

    
    public void Recover()
    {
        if (!_isActive) return;
        
        manaPool.Add(recoverAmount);
    }

    public void Activate()
    {
        _isActive = true;
    }

    public void Deactivate()
    {
        _isActive = false;
    }
}
