using System;
using System.Collections;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class ArenaEvent : MonoBehaviour
{
    [SerializeField] protected float WaitTime = -1f;

    private bool _hasBeenInitialized;
    
    public Action HasStarted;
    public Action HasEnded;
    

    protected virtual void Awake()
    {
        gameObject.SetActive(false);
    }

    [Button]
    public virtual void Begin()
    {
        Initialize();
        
        CoroutineCaller.Instance.StartCoroutine(WaitCoroutine());
    }

    protected void Initialize()
    {
        gameObject.SetActive(true);
        
        HasStarted?.Invoke();
    }

    protected virtual void End()
    {
        HasEnded?.Invoke();

        Disable();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator WaitCoroutine()
    {
        yield return null;

        if (WaitTime > Mathf.Epsilon) yield return new WaitForSeconds(WaitTime);
        
        End();
    }
}
