using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using RootMotion.FinalIK;
using UnityEngine;
using VRTK.Prefabs.CameraRig.CameraRigSwitcher;
using VRTK.Prefabs.CameraRig.UnityXRCameraRig;

public class Totem : MonoBehaviour
{
    [SerializeField] private Transform firingPoint;
    [SerializeField] private PUNPlayerTargetManager playerTargetManager;
    [SerializeField] private ObjectPool objectPool;
    
    //Can be made private. Currently for testing purposes.
    [SerializeField] private float timeBetweenSpawn;
    [SerializeField] private List<PlayerTarget> playerList = new List<PlayerTarget>();
    
    //Can be changed to const but depends on gameplay. Ex. Difficulty could change these values.
    [SerializeField] private float minTimeBetweenSpawn;
    [SerializeField] private float maxTimeBetweenSpawn;
    
    [SerializeField] private float difficultyModifier;
    [SerializeField] private float lineOfSightMaxDistance;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private PhotonView photonView;
    private Vector3 _aimPosition;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(IsActivated());
    }

    private Vector3 SelectAimTarget(PlayerTarget playerTarget)
    {
        if (Random.value < difficultyModifier)
            return playerTarget.GetPuppetHead().position;

        return playerTarget.GetModelHipsPosition();
    }

    private void Fire()
    {
        if (photonView.IsMine)
        {
            playerList = playerTargetManager.GetPlayerTargets();

            foreach (var player in playerList)
            {
                _aimPosition = SelectAimTarget(player);
                firingPoint.transform.LookAt(_aimPosition);
                
                if (LineOfSightIsOpen(firingPoint.transform.forward, lineOfSightMaxDistance))
                {
                    
                    objectPool.GetPooledObject().Spawn(firingPoint, firingPoint.transform.position, firingPoint.rotation, true);
                }
                
                //objectPool.GetPooledObject().Spawn(firingPoint, firingPoint.transform.position, firingPoint.rotation, true);
            }
        }
    }

    private IEnumerator IsActivated()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenSpawn);
            Fire();
            timeBetweenSpawn = Random.Range(minTimeBetweenSpawn, maxTimeBetweenSpawn);
        }
    }

    private bool LineOfSightIsOpen(Vector3 aimPosition, float maxSearchDistance)
    {
        RaycastHit hit;
        
        if (Physics.Raycast(firingPoint.position, aimPosition, out hit, maxSearchDistance, layerMask))
        {
            DebugLogger.Info("LineOfSightOpen", hit.transform.name, this);
            //Checking if we hit the body or head. Not the most elegant and probably needs to be changed.
            return hit.transform.GetComponent<VRIK>() || hit.transform.GetComponent<XRSettingsConfigurator>() || hit.transform.GetComponent<UnityXRConfigurator>();
        }
        
        return false;
    }
}
