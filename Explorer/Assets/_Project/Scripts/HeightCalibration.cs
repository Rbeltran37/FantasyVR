using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class HeightCalibration : MonoBehaviour
{
    [SerializeField] private Transform headAnchor;
    [SerializeField] private Transform playArea;
    [SerializeField] private UserSO userSo;

    
    [Button]
    public void SetUserHeight()
    {
        if (DebugLogger.IsNullError(headAnchor, "Must be set in editor.", this)) return;
        if (DebugLogger.IsNullError(playArea, "Must be set in editor.", this)) return;

        var positionDifference = headAnchor.position.y - playArea.position.y;
        userSo.SetHeight(positionDifference);
    }
}
