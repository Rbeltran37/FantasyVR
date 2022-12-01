using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zinnia.Extension;

public class PotExplosion : MonoBehaviour
{

    public GameObject intact;
    public GameObject broken;
    public Transform explosionPoint;
    public float force;
    public float radius;

    public List<Rigidbody> brokenPieces = new List<Rigidbody>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Button]
    public void Explode()
    {
        intact.SetActive(false);
        broken.SetActive(true);

        foreach (var brokenPiece in brokenPieces)
        {
            brokenPiece.isKinematic = false;
            brokenPiece.AddExplosionForce(force, explosionPoint.position, radius);
        }
    }
}
