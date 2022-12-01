using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class Arrow : PooledRigidbody
{
    [SerializeField] private ArrowData arrowData;
    [SerializeField] private ElementPropertyContainer arrowTipElementPropertyContainer;
    [SerializeField] private ElementPropertyContainer arrowShaftElementPropertyContainer;
    [SerializeField] private DamageDealt damageDealt;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform arrowTip;
    [SerializeField] private Transform arrowTailEnd;
    [SerializeField] private Collider damageCollider;
    [SerializeField] private GameObject trailFxGameObject;
    [SerializeField] private SkillModifierContainer skillModifierContainer;
    
    [System.Serializable]
    public class ActivationEvent : UnityEvent<Vector3> { }
    
    public ActivationEvent HasActivated;

    private int _deflectionsRemaining;    // Used to prevent arrows getting stuck on colliders
    private float _arrowLength;
    private bool _isStopped;
    private Dictionary<Element.Effectiveness, Action<GameObject>> _hitDictionary;
    private float _speed = DEFAULT_SPEED;
    private float _currentFireValue;

    public Action HasStopped;
    public Action HasHit;

    private const float TOLERANCE = .1f;
    private const float DEFAULT_SPEED = 50;


    protected override void Awake()
    {
        base.Awake();
        
        if (DebugLogger.IsNullError(arrowData, "Must be set in editor.", this)) return;
        if (DebugLogger.IsNullError(arrowTipElementPropertyContainer, "Must be set in editor.", this)) return;
        if (DebugLogger.IsNullError(arrowTailEnd, "Must be set in editor.", this)) return;
        if (DebugLogger.IsNullError(arrowTip, "Must be set in editor.", this)) return;
        if (DebugLogger.IsNullError(damageCollider, "Must be set in editor.", this)) return;

        _arrowLength = Vector3.Distance(arrowTailEnd.position, arrowTip.position);
        _deflectionsRemaining = arrowData.numDeflections;
        
        damageCollider.enabled = false;
        damageCollider.isTrigger = true;
        _isStopped = true;
        
        _hitDictionary = new Dictionary<Element.Effectiveness, Action<GameObject>>
        {
            {Element.Effectiveness.Ineffective, Shatter },
            {Element.Effectiveness.Normal, Deflect },
            {Element.Effectiveness.Effective, Stop },
        };
    }

    private void FixedUpdate()
    {
        // If we've hit something, don't update
        if (_isStopped)
            return;

        //trying to add rotation to arrow up vector
        var rotatedUpVector = Quaternion.AngleAxis(arrowData.rotationSpeed, Vector3.up) * transform.up;
        
        //if arrow starts to fall, rotate downward.
        if (_deflectionsRemaining >= 0)
        {
            if (ThisRigidbody.velocity != Vector3.zero)
            {
                ThisRigidbody.MoveRotation(Quaternion.LookRotation(ThisRigidbody.velocity, rotatedUpVector));
            }
        }

        // Collision check
        RaycastHit hit;
        if (Physics.Raycast(arrowTailEnd.position, arrowTailEnd.forward, out hit, _arrowLength, 
            arrowData.hittableLayers, QueryTriggerInteraction.Ignore))
        {
            var hitGameObject = hit.collider.gameObject;
            var otherElementProperty = hitGameObject.GetComponent<ElementPropertyContainer>();
            if (!otherElementProperty) return;
            
            otherElementProperty.ActivateFx(arrowTipElementPropertyContainer, hit.point, hit.normal);
            arrowTipElementPropertyContainer.ActivateFx(otherElementProperty, hit.point, hit.normal);

            var element = arrowTipElementPropertyContainer.GetElement();
            var otherElement = otherElementProperty.GetElement();
            var effectiveness = element.GetEffectiveness(otherElement);
            
            if (effectiveness != Element.Effectiveness.Effective)
            {
                PlayShatterFx(otherElementProperty, hit);
                Shatter(hitGameObject);
                return;
            }
            
            _hitDictionary[effectiveness].Invoke(hitGameObject);

            if (damageDealt)
            {
                var damageReceived = hitGameObject.GetComponent<DamageReceived>();
                if (damageReceived) damageDealt.DealDamage(damageReceived);

                damageDealt.canDealDamage = false;
            }
        }
    }

    private void PlayShatterFx(ElementPropertyContainer otherElementPropertyContainer, RaycastHit hit)
    {
        var physicsObjectHit = hit.collider.GetComponent<PhysicsObject>();
        if (!physicsObjectHit) return;
        
        arrowShaftElementPropertyContainer.ActivateFx(otherElementPropertyContainer, hit.point, hit.normal);
    }

    private void Stop(GameObject hitObject)
    {
        if (!hitObject) return;

        if (_isStopped) return;
         
        // Flag
        _isStopped = true;
        
        //Disable Trail FX
        trailFxGameObject.SetActive(false);

        var hitTransform = hitObject.transform;
        var localScale = hitTransform.localScale;
        var x = Math.Abs(localScale.x);
        var y = Math.Abs(localScale.y);
        var z = Math.Abs(localScale.z);
        if (Math.Abs(x - y) < TOLERANCE &&
            Math.Abs(x - z) < TOLERANCE)
        {
            // Parent
            ThisTransform.SetParent(hitTransform);
            SetParent(hitTransform, ThisTransform.localPosition, ThisTransform.localRotation, false);        //TODO may be obsolete, belong in PooedObject
        }
        
        // Disable Collider
        damageCollider.isTrigger = true;
        damageCollider.enabled = false;

        // Disable physics
        ThisRigidbody.isKinematic = true;
        ThisRigidbody.useGravity = false;
        
        //Deactivate Trail FX
        if (trailFxGameObject) trailFxGameObject.SetActive(false);

        //Plays the noise on impact of object
        arrowData.simpleAudioEvent.Play(audioSource);
        
        HasStopped?.Invoke();
        HasStopped = null;
        
        HasActivated?.Invoke(ThisTransform.position);
        HasHit?.Invoke();
    }

    public void Fire(float pullValue)
    {
        if (damageCollider) damageCollider.enabled = true;
        
        // Activate Trail FX
       if (trailFxGameObject) trailFxGameObject.SetActive(true);
        
        // Flag
        _isStopped = false;

        // Unparent
        SetParent(null, ThisTransform.position, ThisTransform.rotation, true);

        // Physics
        ThisRigidbody.isKinematic = false;
        ThisRigidbody.useGravity = true;
        ThisRigidbody.AddForce(ThisTransform.forward * (pullValue * _speed), ForceMode.Impulse);
        
        
        Despawn(arrowData.arrowLifetime);
    }

    private void Shatter(GameObject hitObject)
    {
        damageCollider.enabled = false;
        damageCollider.isTrigger = true;
        _isStopped = true;
        
        HasHit?.Invoke();
        
        Despawn();
    }

    private void Deflect(GameObject hitObject)
    {
        if (_deflectionsRemaining <= 0) Shatter(hitObject);
        
        Deflect();
        
        HasHit?.Invoke();
    }
    
    private void Deflect()
    {
        StartCoroutine(DeflectCoroutine());
    }
    
    private IEnumerator DeflectCoroutine()
    {
        damageCollider.isTrigger = false;
        _deflectionsRemaining--;
        
        yield return new WaitForFixedUpdate();
        
        damageCollider.isTrigger = true;
    }

    protected override void ResetObject()
    {
        base.ResetObject();
        
        _deflectionsRemaining = arrowData.numDeflections;
        damageCollider.enabled = false;
        damageCollider.isTrigger = true;
        _isStopped = true;
        
        if (trailFxGameObject) trailFxGameObject.SetActive(false);
    }

    public SkillModifierContainer GetSkillModifierContainer()
    {
        return skillModifierContainer;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }
}
