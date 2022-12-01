using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyScaleHandler : MonoBehaviour
{
    [SerializeField] private PhotonView photonView;
    [SerializeField] private Transform enemyParentTransform;
    [SerializeField] [MinMaxFloatRange(0, 2)] private RangedFloat variation;

    private float _baseScale;

    
    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (_baseScale > 0) return;
        
        if (!enemyParentTransform)
        {
            DebugLogger.Error(nameof(Awake), $"{nameof(enemyParentTransform)} is null. Must be set in editor.", this);
            return;
        }

        _baseScale = enemyParentTransform.localScale.x;
    }

    public void Setup()
    {
        Initialize();
        
        var randomMultiplier = Random.Range(variation.minValue, variation.maxValue);
        var adjustedScale = _baseScale * randomMultiplier;
        
        SetScale(adjustedScale);
    }

    private void SetScale(float adjustedScale)
    {
        if (!enemyParentTransform)
        {
            DebugLogger.Error(nameof(SetScale), $"{nameof(enemyParentTransform)} is null. Must be set in editor.", this);
            return;
        }
        
        var scaleVector = new Vector3(adjustedScale, adjustedScale, adjustedScale);
        enemyParentTransform.localScale = scaleVector;

        SendSetScale(adjustedScale);
    }

    private void SendSetScale(float adjustedScale)
    {
        if (PhotonNetwork.OfflineMode) return;
        if (!photonView || !photonView.IsMine) return;

        photonView.RPC(nameof(RPCSetScale), RpcTarget.OthersBuffered, adjustedScale);
    }

    [PunRPC]
    private void RPCSetScale(float adjustedScale)
    {
        Initialize();
        SetScale(adjustedScale);
    }

    public void ResetObject()
    {
        enemyParentTransform.localScale = new Vector3(_baseScale, _baseScale, _baseScale);
    }
}
