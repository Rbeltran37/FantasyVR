using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ChangeEnvironment : ArenaEvent
{
    [SerializeField] private ObjectGroup objectGroup;
    [SerializeField] private bool activate;


    protected override void Awake()
    {
        base.Awake();
        
        if (!objectGroup)
        {
            DebugLogger.Error("Awake", "objectGroup has not been assigned in editor.", this);
            return;
        }

        if (activate)
        {
            objectGroup.HasBeenActivated += End;
        }
        else
        {
            objectGroup.HasBeenDeactivated += End;
        }
    }

    public override void Begin()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (DebugLogger.IsNullError(objectGroup, "Must be set in editor.", this)) return;

            if (activate)
            {
                objectGroup.Enable();
            }
            else
            {
                objectGroup.Disable();
            }
        }
        
        base.Begin();
    }
}