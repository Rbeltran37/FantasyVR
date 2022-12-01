using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DestructibleShellReference : MonoBehaviour
{
    [SerializeField] private DestructibleShell[] destructibleShells;
    
    
    [PunRPC]
    private void RPCDemolish(Vector3 impactOrigin, float force)
    {
        foreach (var destructibleShell in destructibleShells)
        {
            if (!destructibleShell.enabled) continue;
            
            destructibleShell.Demolish(impactOrigin, force);
        }
    }
}
