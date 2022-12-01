using System.Collections;
using Photon.Pun;
using Photon.Voice.PUN;
using Sirenix.OdinInspector;
using UnityEngine;

//TODO Name: is this an ArmoryPlatform, or a platform?
public class ArmoryPlatform : MonoBehaviour
{
    public Transform platform;
    public float totalTimeMoving;
    private Vector3 _targetPos;
    public float travelDistance;
    public float platformMovementDelay;
    [SerializeField] private string scene;
    [SerializeField] private ReadyUpCustomProperty readyUpCustomProperty;
    private PhotonView _photonView;
    private bool hasRaised = false;
    private bool hasCalledRaisePlatform = false;

    // Start is called before the first frame update
    void Start()
    {
        var position = platform.transform.position;
        _targetPos = new Vector3(position.x, position.y + travelDistance, position.z);
        
        _photonView = PhotonView.Get(this);
    }
    
    public void RpcRaisePlatform()
    {
        if(_photonView)
            _photonView.RPC("RaisePlatform", RpcTarget.All);
    }
    

    [PunRPC]
    private void RaisePlatform()
    {
        StartCoroutine(RaisePlatformCoroutine(_targetPos, totalTimeMoving));
    }

    IEnumerator RaisePlatformCoroutine(Vector3 targetPosition, float duration)
    {
        yield return new WaitForSeconds(platformMovementDelay);
        
        float time = 0;
        Vector3 startPosition = platform.transform.position;

        while (time < duration)
        {
            platform.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;

            if (time >= duration / 2 && !hasRaised)
            {
                hasRaised = true;
                SceneHandler.Instance.StartLoadTransition(scene);
            }

            yield return null;
        }
        
        platform.transform.position = targetPosition;
    }
}
