using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class PotionInteractor : Interactable
{
    public Renderer renderer;
    private bool _isGrabbed = false;
    public Rigidbody rb;
    public List<Transform> brokenPiecesList = new List<Transform>();
    public List<Vector3> brokenPiecesStartPosition = new List<Vector3>();
    public GameObject liquidParticleSplash;
    public GameObject solidBottle;
    public Transform brokenBottle;
    public float velocityThreshold;
    public float explosionForce;
    public float explosionRadius;
    public Transform explosionPoint;
    public bool glassIsBroken = false;
    public AudioSource audio;
    public GameObject cork;
    public GameObject liquidGO;
    public float corkForce;
    private bool corkIsRemoved = false;
    public ParticleSystem liquid;
    public bool isPouring;
    public SimpleAudioEvent corkPopEvent;
    public SimpleAudioEvent pourEvent;
    public SimpleAudioEvent glassBreakEvent;
    public float POUR_INTERVAL = 0.25f;
    public float bottleLiquidAmount;
    public float _currentLiquid = 1f; 
    private const float POUR_AMOUNT = .05f;
    private bool isEmpty = false;
    public PhotonView photonView;
    public Vector3 corkStartPosition;
    

    private void Start()
    {
        bottleLiquidAmount = renderer.material.GetFloat("LiquidFill");
        GetBrokenPiecesStartPosition();
        GetCorkStartPosition();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PourLiquid();
    }

    public override void UnGrab()
    {
        ThisRigidbody.velocity = GetAverageVelocity();
        ThisRigidbody.angularVelocity = GetAverageAngularVelocity();
        base.UnGrab();
    }

    private void DetectImpact()
    {
        if (Mathf.Abs(ThisRigidbody.velocity.magnitude) > velocityThreshold && IsGrabbed())
        {
            if (photonView.IsMine)
            {
                photonView.RPC("BreakGlass", RpcTarget.Others);
                BreakGlass();
            }
        }
        
        if (Mathf.Abs(ThisRigidbody.velocity.magnitude) > velocityThreshold && !IsGrabbed())
        {
            if (photonView.IsMine)
            {
                photonView.RPC("BreakGlass", RpcTarget.Others);
                BreakGlass();
            }
        }
    }
    
    [Button]
    [PunRPC]
    private void BreakGlass()
    {
        solidBottle.SetActive(false);
        brokenBottle.gameObject.SetActive(true);
        liquidParticleSplash.SetActive(!isEmpty);
        glassBreakEvent.Play(audio);
        ExplodeGlass();
        LaunchCork();
    }
    
    private void ExplodeGlass()
    {
        foreach (var brokenPiece in brokenPiecesList)
        {
            Rigidbody rb = brokenPiece.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            
            if(rb)
                rb.AddExplosionForce(explosionForce, explosionPoint.position, explosionRadius);
        }

        glassIsBroken = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Fist"))
            return;
        
        DetectImpact();
    }

    public void LaunchCork()
    {
        if (corkIsRemoved)
            return;
        
        var corkRB = cork.GetComponent<Rigidbody>();
        var corkCollider = cork.GetComponent<Collider>();
        corkCollider.isTrigger = false;
        corkRB.useGravity = true;
        corkRB.isKinematic = false;
        corkRB.AddForce(transform.up * corkForce, ForceMode.Impulse);
        corkPopEvent.Play(audio);
        corkIsRemoved = true;
    }
    
    private void PourLiquid()
    {
        if (corkIsRemoved && !glassIsBroken)
        {
            if (VectorMath.IsVectorFacingSameDirection(transform.up, Vector3.down))
            {
                Pour();
            }
            else
            {
                StopPour();
            }
        }
    }

    private void Pour()
    {
        if (isPouring || _currentLiquid <= 0f) return;

        isPouring = true;
        liquidGO.SetActive(true);
        liquid.Play();
        StartCoroutine(PourCoroutine());
        StartCoroutine(PourBottleLiquid());
    }

    private IEnumerator PourCoroutine()
    {
        _currentLiquid = bottleLiquidAmount;
        
        while (_currentLiquid > 0 && isPouring)
        {
            yield return new WaitForSeconds(POUR_INTERVAL);
            pourEvent.Play(audio, _currentLiquid);
            _currentLiquid -= POUR_AMOUNT;

            if (bottleLiquidAmount <= 0)
                yield return null;
        }

        if (_currentLiquid <= 0f)
        {
            StopPour();
        }
    }

    private IEnumerator PourBottleLiquid()
    {
        while (bottleLiquidAmount > 0 && isPouring)
        {
            yield return new WaitForSeconds(POUR_INTERVAL);
            float value = Mathf.Clamp(bottleLiquidAmount, 0, 1);
            bottleLiquidAmount -= POUR_AMOUNT;
            renderer.material.SetFloat("LiquidFill", value);
        }

        if (bottleLiquidAmount <= 0)
        {
            isEmpty = true;
            liquidGO.SetActive(false);
        }
            
    }

    private void StopPour()
    {
        if (!isPouring) return;

        isPouring = false;
        liquid.Stop();
        pourEvent.Stop(audio);
    }

    [Button]
    private void ResetObject()
    {
        solidBottle.SetActive(true);
        liquidParticleSplash.SetActive(false);
        bottleLiquidAmount = 1f;
        glassIsBroken = false;
        transform.position = Vector3.one;
        transform.rotation = Quaternion.identity;
        ResetCork();
        ResetBrokenPieces();
    }

    private void GetBrokenPiecesStartPosition()
    {
        foreach (Transform brokenPiece in brokenBottle)
        {
            brokenPiecesList.Add(brokenPiece);
            brokenPiecesStartPosition.Add(brokenPiece.position);
        }
            
    }

    private void ResetBrokenPieces()
    {
        for (int i = 0; i < brokenPiecesList.Count; i++)
        {
            var rb = brokenPiecesList[i].GetComponent<Rigidbody>();
            rb.isKinematic = true;
            brokenPiecesList[i].position = brokenPiecesStartPosition[i];
            brokenPiecesList[i].rotation = Quaternion.identity;
        }
        
        brokenBottle.gameObject.SetActive(false);
    }

    private void GetCorkStartPosition()
    {
        corkStartPosition = cork.transform.position;
    }

    private void ResetCork()
    {
        var rb = cork.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        var corkCollider = cork.GetComponent<Collider>();
        corkCollider.isTrigger = true;
        cork.transform.localPosition = Vector3.zero;
        cork.transform.localRotation = Quaternion.identity;
        corkIsRemoved = false;
    }

    public float GetLiquidAmountConsumed()
    {
        return bottleLiquidAmount - _currentLiquid;
    }

}
