using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Photon.Pun;
using System.Runtime.InteropServices;

public class NetworkedEnemyController : MonoBehaviour
{
    public Transform spawnPoint;

    [SerializeField] private PhotonView photonView;
    [SerializeField] private CustomUserControlAI aiControl;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }
    
    [Button]
    public void InstantiateEnemy()
    {
        var enemy = PhotonNetwork.InstantiateSceneObject("[Networked Enemy]", spawnPoint.position, spawnPoint.rotation);
        ReferenceManager.enemyAi = enemy.GetComponentInChildren<CustomUserControlAI>();
        photonView.RPC("SyncEnemyMoveTarget", RpcTarget.All, 1);
    }

    [PunRPC]
    public void SyncEnemyMoveTarget(int player)
    {
        var playerTarget = player.Equals(1) ? ReferenceManager.PlayerOne : ReferenceManager.PlayerTwo;
        ReferenceManager.enemyAi.navTarget = playerTarget;
    }
    
    
}
