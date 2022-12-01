using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPool : MonoBehaviour
{
    [SerializeField] private HolsterManager holsterManager;
    [SerializeField] private AudioSource audioSource;

    private float _maxMana = 100;
    private float _currentMana;
    private float _rechargeRate;

    private const float DEFAULT_RECHARGE_RATE = 1f;


    private void Start()
    {
        _rechargeRate = DEFAULT_RECHARGE_RATE;
        Refill();
    }

    private void Update()
    {
        Recharge();
    }

    public void SetMaxMana(float maxMana)
    {
        _maxMana = maxMana;
    }

    public void SetRechargeRate(float rechargeRate)
    {
        _rechargeRate = rechargeRate;
    }

    public void Refill()
    {
        _currentMana = _maxMana;
    }

    public bool CanCast(float manaCost)
    {
        if (_currentMana < manaCost)
        {
            NotEnoughManaFx();
            return false;
        }
        
        return true;
    }
    
    public void PayCastingCost(float manaCost)
    {
        _currentMana -= manaCost;
    }

    public float GetManaPercentage()
    {
        return _currentMana / _maxMana;
    }

    public void Add(float amountToAdd)
    {
        _currentMana += amountToAdd;

        if (_currentMana > _maxMana) _currentMana = _maxMana;
    }

    private void Recharge()
    {
        if (_currentMana >= _maxMana) return;

        _currentMana += Time.deltaTime * _rechargeRate;

        if (_currentMana > _maxMana)
        {
            _currentMana = _maxMana;
        }
    }

    private void NotEnoughManaFx()
    {
        if (!audioSource)
        {
            DebugLogger.Error("NotEnoughManaFx", "audioSource is null. Must be set in editor.", this);
            return;
        }
        
        if (!holsterManager)
        {
            DebugLogger.Error("NotEnoughManaFx", "holsterManager is null. Must be set in editor.", this);
            return;
        }
        
        audioSource.PlayOneShot(holsterManager.invalidClip);
    }
}
