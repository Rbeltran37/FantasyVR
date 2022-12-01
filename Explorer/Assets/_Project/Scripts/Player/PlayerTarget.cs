using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerTarget : MonoBehaviour
{
    [SerializeField] private PhotonView photonView;
    [SerializeField] private GameObject playerControllerGameObject;
    [SerializeField] private Transform playerModelHipsTransform;
    [SerializeField] private Transform playerPuppetHeadTransform;
    [SerializeField] private Transform[] aimTargets;

    private GameObject _thisGameObject;
    private int _actorNumber;
    private int _photonViewId;
    private PUNPlayerTargetManager _punPlayerTargetManager;

    private void Awake()
    {
        if (DebugLogger.IsNullWarning(photonView, this, "Should be set in editor. Attempting to find."))
        {
            photonView = GetComponent<PhotonView>();
            if (DebugLogger.IsNullError(photonView, this, "Should be set in editor. Not found.")) return;
        }

        photonView.Owner.TagObject = this;
        _actorNumber = photonView.OwnerActorNr;
        _photonViewId = photonView.ViewID;
        _thisGameObject = gameObject;

        AddSelfToPlayerTargets();
    }

    private void OnDestroy()
    {
        RemoveSelfFromPlayerTargets();
    }

    private void AddSelfToPlayerTargets()
    {
        _punPlayerTargetManager = FindObjectOfType<PUNPlayerTargetManager>();
        if (DebugLogger.IsNullInfo(_punPlayerTargetManager, this, "Not found in scene.")) return;
        
        _punPlayerTargetManager.AddPlayer(_thisGameObject);
    }
    
    private void RemoveSelfFromPlayerTargets()
    {
        _punPlayerTargetManager = FindObjectOfType<PUNPlayerTargetManager>();
        if (DebugLogger.IsNullInfo(_punPlayerTargetManager, this, "Not found in scene.")) return;
        
        _punPlayerTargetManager.RemovePlayer(this);
    }

    public int GetPhotonViewId()
    {
        return _photonViewId;
    }

    public int GetActorNumber()
    {
        return _actorNumber;
    }
    
    public Transform GetModelHips()
    {
        return playerModelHipsTransform;
    }

    public Vector3 GetModelHipsPosition()
    {
        return playerModelHipsTransform.position;
    }

    public Transform GetPuppetHead()
    {
        return playerPuppetHeadTransform;
    }
    
    public GameObject GetPlayerController()
    {
        return playerControllerGameObject;
    }
    
    public Transform GetNearestPlayerTarget(Transform otherTransform)
    {
        if (aimTargets == null) return null;
        var length = aimTargets.Length;
        var nearestDistance = Mathf.Infinity;
        Transform nearestTarget = null;
        for (var i = 0; i < length; i++)
        {
            var aimTransform = aimTargets[i];
            var distance = Vector3.Distance(aimTransform.position, otherTransform.position);
            if (distance < nearestDistance || nearestTarget == null)
            {
                nearestDistance = distance;
                nearestTarget = aimTransform;
            }
        }

        return nearestTarget;
    }
}
