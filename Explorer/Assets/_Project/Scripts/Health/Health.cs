using System;
using System.Collections;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private HealthData healthData;
    
    public bool isAlive = true;

    private float _maxHealth;
    public float _currentHealth;
    private bool _isInvulnerable ;
    private bool _wasDeathInvoked;
    
    public Action<float> WasAdded;
    public Action<float> WasHit;
    public Action WasHitButNotKilled;
    public Action WasKilled;

    private void Awake()
    {
        _maxHealth = healthData.maxHealth;
        _currentHealth = _maxHealth;
        StartCoroutine(ActivateInvulnerability());
    }

    [Button]
    public virtual void Subtract(float amount)
    {
        if (_isInvulnerable || _wasDeathInvoked) return;

        _currentHealth -= amount;
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            isAlive = false;
        }
        WasHit?.Invoke(amount);

        if (!isAlive)
        {
            _wasDeathInvoked = true;
            WasKilled?.Invoke();
            return;
        }
        
        WasHitButNotKilled?.Invoke();
        
        if (healthData.hitInvulnerabilityTime > Mathf.Epsilon)
        {
            StartCoroutine(ActivateInvulnerability());
        }
    }

    [Button]
    public virtual void Add(float amount)
    {
        if (!isAlive) return;
        
        _currentHealth += amount;
        if (_currentHealth > _maxHealth) _currentHealth = _maxHealth;
        
        WasAdded?.Invoke(amount);
    }

    public float GetHealthPercentage()
    {
        return _currentHealth / _maxHealth;
    }

    public void Kill()
    {
        Subtract(healthData.maxHealth);
    }

    private IEnumerator ActivateInvulnerability()
    {
        _isInvulnerable = true;
        yield return new WaitForSeconds(healthData.hitInvulnerabilityTime);
        _isInvulnerable = false;
    }

    public void ResetObject()
    {
        Unsubscribe();
        Revive();
    }

    public void Revive()
    {
        _currentHealth = _maxHealth;
        isAlive = true;
        _wasDeathInvoked = false;
        _isInvulnerable = false;
    }

    private void Unsubscribe()
    {
        WasHit = null;
        WasHitButNotKilled = null;
        WasKilled = null;
    }
}
