using System;
using System.Collections;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerScale : MonoBehaviour
{
    [Header("Variables to Calibrate Height")]
    [SerializeField] private Transform modelHeightReferenceTransform;
    [SerializeField] private Transform playerModelRoot;
    [SerializeField] private Transform puppetMasterTransform;
    [SerializeField] private UserSO userSo;
    
    private float _userHeight;
    private float _modelHeight;
    
    private const float MIN_HEIGHT = 1;
    private const float FALLBACK_HEIGHT = 1.75f;
    
    public Action<Vector3> ScaleHasBeenSet;

    
    private void Awake()
    {
        SetModelHeight();
        SetPlayerScale();
    }

    //The model head location that is being used an additional transform placed where the models eyes are located.
    //This is our reference point for when we scale the height.
    private void SetModelHeight()
    {
        if (DebugLogger.IsNullError(modelHeightReferenceTransform, "Must be set in editor", this)) return;
        if (DebugLogger.IsNullError(playerModelRoot, "Must be set in editor", this)) return;

        var positionDifference = modelHeightReferenceTransform.position.y - playerModelRoot.position.y;
        _modelHeight = positionDifference;
    }

    [Button]
    //To adjust this, play with the modelHeadLocation Transform 
    public void SetPlayerScale()
    {
        SetUserHeight();

        var playerScale = CalculateScale(_userHeight);
        SetScale(playerScale);
    }
    
    private void SetUserHeight()
    {
        if (DebugLogger.IsNullError(userSo, "Must be set in editor.", this)) return;
        
        _userHeight = userSo.UserHeight;
        
        //TODO remember to remove
        if (_userHeight <= MIN_HEIGHT)
        {
            DebugLogger.Info(MethodBase.GetCurrentMethod().Name, $"{nameof(_userHeight)}={_userHeight} <= {nameof(MIN_HEIGHT)}={MIN_HEIGHT}. Using Fallback height.", this);
            _userHeight = FALLBACK_HEIGHT;
        }
    }

    [Button]
    private void OverrideHeight(float height)
    {
        var updatedScale = CalculateScale(height);
        SetScale(updatedScale);
    }

    private Vector3 CalculateScale(float height)
    {
        return Vector3.one * (height / _modelHeight);
    }

    private void SetScale(Vector3 scale)
    {
        playerModelRoot.localScale = scale;
        puppetMasterTransform.localScale = scale;

        ScaleHasBeenSet?.Invoke(scale);
    }
}
