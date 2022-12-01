using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun;
using UnityEngine;

public class ArcherAi : MonoBehaviour
{
    [SerializeField] private PhotonView photonView;
    [SerializeField] private EnemyBowInstance enemyBowInstance;
    
    private bool IsOnlineButNotMine => !PhotonNetwork.OfflineMode && photonView && !photonView.IsMine;
    

    public void ReleaseArrow()
    {
        if (IsOnlineButNotMine) return;
        
        if (DebugLogger.IsNullError(enemyBowInstance, this)) return;

        enemyBowInstance.FireArrow();
    }
}
