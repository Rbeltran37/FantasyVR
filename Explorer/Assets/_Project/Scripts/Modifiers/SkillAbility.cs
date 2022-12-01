using System;
using System.Collections;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class SkillAbility : MonoBehaviour
{
    [SerializeField] protected Transform ThisTransform;
    
    protected float Value;
    protected int Level;
    protected Vector3 SpawnPosition;
    
    protected const int NOT_APPLIED = 0;


    protected virtual void Awake()
    {
        Initialize();
    }

    [Button]
    protected virtual void Initialize()
    {
        if (DebugLogger.IsNullWarning(ThisTransform, "Should be set in editor.", this))
        {
            ThisTransform = transform;
        }
    }

    public virtual void SetValue(float value)
    {
        Value = value;
    }
    
    public virtual void SetLevel(int level)
    {
        Level = level;
    }

    public virtual float GetValue()
    {
        return Value;
    }

    protected abstract void Activate();
}
