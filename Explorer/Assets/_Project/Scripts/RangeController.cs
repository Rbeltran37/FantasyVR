using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class RangeController : MonoBehaviour
{
    [SerializeField] private List<RangeTarget> rangeTargets;
    [SerializeField] private bool targetIsActive = true;
    [SerializeField] private PhotonView photonView;
    
    private const float MIN_TIME = .5f;
    private const float MAX_TIME = 1f;
    
    private const float MIN_TIME_DOWN = 3f;
    private const float MAX_TIME_DOWN = 5f;

    // Start is called before the first frame update
    [Button]
    void Start()
    {
        //StartCoroutine(SynchronizeTargets());
    }

    private IEnumerator SynchronizeTargets()
    {
        yield return new WaitForSeconds(3f);

        while (targetIsActive)
        {
            float randomTimeBetweenTargets = Random.Range(MIN_TIME, MAX_TIME);
            yield return new WaitForSeconds(randomTimeBetweenTargets);
                
            int index = Random.Range(0, rangeTargets.Count);
            float randomTimeToWait = Random.Range(MIN_TIME_DOWN, MAX_TIME_DOWN);
            photonView.RPC("CallPopUpTarget", RpcTarget.All, index, randomTimeToWait);
        }
    }

    public void SynchronizeTargetHit()
    {
        int index = Random.Range(0, rangeTargets.Count);
        float randomTimeToWait = Random.Range(MIN_TIME_DOWN, MAX_TIME_DOWN);
        DebugLogger.Info("SynchronizeTargetHit", index.ToString(), this);
        photonView.RPC("CallPopUpTarget", RpcTarget.All, index, randomTimeToWait);
    }

    [PunRPC]
    private void CallPopUpTarget(int index, float randomTime)
    {
        var rangeTarget = rangeTargets[index];
        DebugLogger.Info("CallPopUpTarget", index.ToString(), this);
        rangeTarget.CallTargetUp(randomTime);
    }
}
