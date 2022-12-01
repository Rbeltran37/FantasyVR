using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PotionBottle : PooledInteractable
{
    [SerializeField] private Transform bottleOpening;
    [SerializeField] private Transform corkTransform;
    [SerializeField] private Collider corkCollider;
    [SerializeField] private Rigidbody corkRigidbody;
    [SerializeField] private Renderer liquidRenderer;
    [SerializeField] private GameObject popLiquidGameObject;
    [SerializeField] private ParticleSystem pourLiquid;
    [SerializeField] private ParticleSystem splashLiquid;
    [SerializeField] private DestructibleShell destructibleShell;
    [SerializeField] private FloatingObject floatingObject;
    [SerializeField] private float popCorkForce = 10;
    [SerializeField] private float floatSpeed = .002f;
    [SerializeField] private float floatSlerpSpeed = .02f;
    [SerializeField] private float floatHeight = 1.25f;
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private SimpleAudioEvent popCorkAudioEvent;
    [SerializeField] private SimpleAudioEvent pourEvent;
    [SerializeField] private SimpleAudioEvent glassBreakEvent;
    
    private bool _isCorkPopped;
    private bool _isPouring;
    private bool _hasBeenGrabbed;
    private bool _isInFloatPosition;
    private float _currentLiquidAmount = 1f;
    private float _defaultLiquidFill;
    private int _liquidFillId;
    private Vector3 _corkDefaultLocalPosition;
    private Quaternion _corkDefaultLocalRotation;
    private Transform _corkDefaultParent;
    private Vector3 _velocity;
    private Vector3 _floatPosition;
    
    private bool CanFloat => !_hasBeenGrabbed && !IsGrabbed();

    
    private bool CanPour => _isCorkPopped && !destructibleShell.IsDestroyed;
    private bool IsEmpty => _currentLiquidAmount <= EMPTY;
    
    private const int EMPTY = 0;
    private const float POUR_INTERVAL = 0.25f;
    private const float POUR_AMOUNT = .05f;
    private const string LIQUID_FILL = "LiquidFill";
    private const float FLOAT_POSITION_THRESHOLD = .01f;


    protected override void Awake()
    {
        base.Awake();

        _liquidFillId = Shader.PropertyToID(LIQUID_FILL);
        _defaultLiquidFill = liquidRenderer.material.GetFloat(_liquidFillId);

        _corkDefaultLocalPosition = corkTransform.localPosition;
        _corkDefaultLocalRotation = corkTransform.localRotation;
        _corkDefaultParent = corkTransform.parent;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        _hasBeenGrabbed = false;
        _isInFloatPosition = false;
        floatingObject.enabled = false;
        
        RaycastHit raycastHit;
        if (Physics.Raycast(ThisTransform.position, Vector3.down, out raycastHit, floatHeight, groundLayers))
        {
            _floatPosition = raycastHit.point + Vector3.up * floatHeight;
        }
    }

    private void FixedUpdate()
    {
        if (CanFloat)
        {
            if (!_isInFloatPosition) FloatToPositionAndRotation();
            else FloatUpAndDown();
        }
        
        PourLiquid();
    }

    public override void PopulateParameters()
    {
        base.PopulateParameters();

        if (!destructibleShell)
        {
            destructibleShell = GetComponent<DestructibleShell>();
            if (!destructibleShell) destructibleShell = ThisGameObject.AddComponent<DestructibleShell>();
        }
        
        destructibleShell.PopulateParameters();
    }

    protected override void ResetObject()
    {
        _currentLiquidAmount = _defaultLiquidFill;
        liquidRenderer.material.SetFloat(_liquidFillId, _defaultLiquidFill);
        
        ResetCork();
        
        destructibleShell.ResetObject();
        
        base.ResetObject();
    }
    
    private void FloatToPositionAndRotation()
    {
        ThisTransform.position = Vector3.SmoothDamp(ThisTransform.position, _floatPosition, ref _velocity, floatSpeed);
        ThisTransform.rotation = Quaternion.Slerp(ThisTransform.rotation, Quaternion.identity, floatSlerpSpeed);

        if (Vector3.Distance(ThisTransform.position, _floatPosition) < FLOAT_POSITION_THRESHOLD) _isInFloatPosition = true;
    }

    private void FloatUpAndDown()
    {
        if (floatingObject.enabled) return;

        floatingObject.enabled = true;
    }

    public void PopCork()
    {
        if (_isCorkPopped) return;
        
        _isCorkPopped = true;

        corkTransform.SetParent(null);
        
        corkCollider.isTrigger = false;
        
        corkRigidbody.useGravity = true;
        corkRigidbody.isKinematic = false;
        corkRigidbody.AddForce(bottleOpening.forward * popCorkForce, ForceMode.VelocityChange);
        
        popLiquidGameObject.SetActive(true);
        popCorkAudioEvent.Play(ThisAudioSource);
    }

    private void PourLiquid()
    {
        if (!CanPour) return;
        
        if (VectorMath.IsVectorFacingSameDirection(bottleOpening.forward, Vector3.down))
        {
            Pour();
        }
        else
        {
            StopPour();
        }
    }
    
    private void Pour()
    {
        if (_isPouring || IsEmpty) return;

        _isPouring = true;
        
        pourLiquid.Play();
        
        StartCoroutine(PourCoroutine());
        StartCoroutine(PourBottleLiquid());
    }

    private IEnumerator PourCoroutine()
    {
        while (!IsEmpty && _isPouring)
        {
            yield return new WaitForSeconds(POUR_INTERVAL);
            pourEvent.Play(ThisAudioSource, _currentLiquidAmount);
            _currentLiquidAmount -= POUR_AMOUNT;

            if (_currentLiquidAmount <= EMPTY)
                yield return null;
        }

        if (_currentLiquidAmount <= 0f)
        {
            StopPour();
        }
    }

    private IEnumerator PourBottleLiquid()
    {
        while (!IsEmpty && _isPouring)
        {
            yield return new WaitForSeconds(POUR_INTERVAL);
            var value = Mathf.Clamp(_currentLiquidAmount, EMPTY, 1);
            _currentLiquidAmount -= POUR_AMOUNT;
            liquidRenderer.material.SetFloat(_liquidFillId, value);
        }

        if (IsEmpty) pourLiquid.Stop();
    }

    private void StopPour()
    {
        if (!_isPouring) return;

        _isPouring = false;
        
        pourLiquid.Stop();
        pourEvent.Stop(ThisAudioSource);
    }

    private void ResetCork()
    {
        corkRigidbody.isKinematic = true;
        corkCollider.isTrigger = true;

        corkTransform.SetParent(_corkDefaultParent);
        corkTransform.localPosition = _corkDefaultLocalPosition;
        corkTransform.localRotation = _corkDefaultLocalRotation;
        
        popLiquidGameObject.SetActive(false);

        _isCorkPopped = false;
    }

    public void BreakGlass()
    {
        if (!IsEmpty) splashLiquid.Play();
        
        glassBreakEvent.Play(ThisAudioSource);
    }

    protected override void CompleteGrab()
    {
        base.CompleteGrab();

        ThisRigidbody.isKinematic = false;
        _hasBeenGrabbed = true;
        floatingObject.enabled = false;
    }
}
