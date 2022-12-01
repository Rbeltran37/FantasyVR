using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjects : ArenaEvent
{
    [SerializeField] private ObjectGroup objectGroup;
    [SerializeField] private bool activate;


    protected override void Awake()
    {
        if (DebugLogger.IsNullError(objectGroup, this, "Must be set in editor.")) return;

        if (activate)
        {
            objectGroup.HasBeenActivated += End;
        }
        else
        {
            objectGroup.HasBeenDeactivated += End;
        }
        
        base.Awake();
    }

    public override void Begin()
    {
        if (DebugLogger.IsNullError(objectGroup, this, "Must be set in editor.")) return;
        
        if (activate)
        {
            objectGroup.Activate();
        }
        else
        {
            objectGroup.Deactivate();
        }
        
        base.Begin();
    }
}
