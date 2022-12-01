using Photon.Pun;
using UnityEngine;
using Zinnia.Data.Operation.Extraction;

//TODO Kevin: should be decoupled from ArmoryPlatform, maybe from PunCallbacks. If class is called 'ReadyUp' should be generic
public class ReadyUp : MonoBehaviourPunCallbacks
{
    //TODO variables should be private, most likely all. If other classes need access, use getters. If many, may be a code smell
    public int playerCount = 0;        //TODO logic relies on this value being 0, but variable is public and exposed in editor, never set in class
    public int readyUpCount = 0;        //TODO logic relies on this value being 0, but variable is public and exposed in editor, never set in class
    private PhotonView photonView;        //TODO Kevin: calling RPC function on another script 'ArmoryPlatform', broke when on seperate GameObjects
    public int levelToLoad;
    public ArmoryPlatform platform;
    private string playerTagName = "Player";
    public bool hasRaised = false;
    public bool isReady = false;

    private PhotonView _armoryPhotonView;        //TODO temp, should be used in armory
    
    private const string IS_READY = nameof(IsReady);


    //TODO add debugging messages
    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    //TODO avoid OnTriggerStay, expensive
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag(playerTagName))
        {
            if (!isReady)
            {
                //photonView.RPC("IsReady", RpcTarget.All);    //TODO names shouldn't be hardcoded. Constant variables are more performant
                photonView.RPC(IS_READY, RpcTarget.All);
                isReady = true;
            }

            //TODO can be checked in RPC RaisePlatform call. Refactor
            if (PhotonNetwork.IsMasterClient)
            {
                playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
                
                if (readyUpCount == playerCount && !hasRaised)
                {
                    if (!_armoryPhotonView)
                    {
                        _armoryPhotonView = platform.GetComponent<PhotonView>();
                    }
                    
                    //TODO temp, not ideal
                    if (_armoryPhotonView)
                    {
                        //photonView.RPC("RaisePlatform", RpcTarget.All);        //TODO Shouldn't be calling RPC in another class, relying on wrong photonView
                        _armoryPhotonView.RPC("RaisePlatform", RpcTarget.All);    //TODO temp, should be called from class method belongs to
                        hasRaised = true;        //TODO should be in platform logic, not related to readying up
                    }
                }
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag(playerTagName))
        {
            photonView.RPC("IsNotReady", RpcTarget.All);
            isReady = false;
        }
    }

    //TODO Should not be in this class, possibly in a manage class. 
    public override void OnJoinedRoom()
    {
        photonView.RPC("AddPlayerCount", RpcTarget.All);
    }    

    //TODO RPC functions should follow naming convention to let us know they are RPC functions, ie. RPCAddPlayerCount
    //TODO should this be called by players who aren't master client ie. Can't load?
    [PunRPC]
    public void AddPlayerCount()
    {
        playerCount++;
    }
    
    //TODO should this be called by players who aren't master client ie. Can't load?
    [PunRPC]
    public void MinusPlayerCount()
    {
        playerCount--;
    }
    
    public override void OnLeftRoom()
    {
        photonView.RPC("MinusPlayerCount", RpcTarget.All);
    }

    //TODO should this be called by players who aren't master client ie. Can't load?
    [PunRPC]
    public void IsReady()
    {
        readyUpCount++;
    }
    
    //TODO should this be called by players who aren't master client ie. Can't load?
    [PunRPC]
    public void IsNotReady()
    {
        readyUpCount--;
    }

    //TODO obsolete? Should not be in this class
    public void LoadLevel()
    {
        PhotonNetwork.LoadLevel(levelToLoad);
    }
}
