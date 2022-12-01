using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;

public class PUNPlayerPropertyUpdate : MonoBehaviourPunCallbacks
{
    public UnityEvent playerPropertiesUpdated;
    
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        
        playerPropertiesUpdated?.Invoke();
        //CheckIfAllPlayersInRoomAreReady();
    }
}
