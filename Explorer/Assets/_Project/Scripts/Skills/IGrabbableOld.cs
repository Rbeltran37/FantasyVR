using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrabbableOld
{
    void AttemptGrab(Transform grabParent);
    void Grab(Transform grabParent);
    void AttemptUnGrab();
    void UnGrab();
    bool IsGrabbed();
    bool IsGrabbedBy(Transform grabParent);
}
