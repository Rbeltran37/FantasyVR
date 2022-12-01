using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOpenable
{
    void Open(Action callback);
    void Close(Action callback);
    IEnumerator OpenCoroutine(Action callback);
    IEnumerator CloseCoroutine(Action callback);
}
