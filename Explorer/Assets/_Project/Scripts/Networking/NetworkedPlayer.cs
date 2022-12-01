using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class NetworkedPlayer : MonoBehaviourPun
{
    public GameObject playerPrefab;
    private PhotonView photonView;
    
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine)
        {
            //Changed for testing
            playerPrefab = PhotonNetwork.Instantiate(playerPrefab.name,
                new Vector3(0f, .1f, 0f), Quaternion.identity, 0);
            SetPlayer();
        }
    }

    private void SetPlayer()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == 1)
            {
                ReferenceManager.PlayerOne = playerPrefab.transform;
            }
            else
            {
                ReferenceManager.PlayerTwo = playerPrefab.transform;
            }
        }
    }
}