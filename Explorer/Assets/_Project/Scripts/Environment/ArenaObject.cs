using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ArenaObject : MonoBehaviour
{
    public abstract IEnumerator Activate(Action callback);
    public abstract IEnumerator Deactivate(Action callback);
    public abstract void SetToActivatedPosition();
    public abstract void SetToDeactivatedPosition();
    public abstract void PopulateParameters();
}
