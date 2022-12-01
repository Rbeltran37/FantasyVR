using System;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class ElementPropertyContainer : MonoBehaviour
{
    [SerializeField] private PhysicsObject physicsObject;
    [SerializeField] private ElementProperty elementProperty;
    [SerializeField] private ObjectPool elementFxObjectPool;

    private bool _isMine = true;
    private bool _isActive = true;
    private Transform _thisTransform;
    private Dictionary<Element.Effectiveness, Action<ElementPropertyContainer, Vector3, Vector3>> _effectivenessDictionary;

    private const int ZERO = 0;
    private const string ELEMENT_FX_CONTAINER = "elementfxcontainer";


    private void Awake()
    {
        if (DebugLogger.IsNullError(elementProperty, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(elementFxObjectPool, this, "Must be set in editor.")) return;

        elementFxObjectPool.InitializePool();

        _effectivenessDictionary = new Dictionary<Element.Effectiveness, Action<ElementPropertyContainer, Vector3, Vector3>>
        {
            {Element.Effectiveness.Ineffective, ActivateIneffectiveFx},
            {Element.Effectiveness.Normal, ActivateNormalFx},
            {Element.Effectiveness.Effective, ActivateEffectiveFx},
        };

        var photonView = GetComponent<PhotonView>();
        if (photonView)
        {
            _isMine = photonView.IsMine;
        }

        _thisTransform = transform;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!_isMine) return;
        if (!_isActive) return;
        if (!elementFxObjectPool.IsInitialized) return;
        
        var otherElementPropertyContainer = other.gameObject.GetComponent<ElementPropertyContainer>();
        if (!otherElementPropertyContainer) return;
        if (!otherElementPropertyContainer._isMine) return;

        var contact = other.GetContact(ZERO);
        var collisionNormal = contact.normal;

        var otherTransformPosition = other.transform.position;
        var wrongDirection = elementProperty.emitsOut ? contact.point - otherTransformPosition : otherTransformPosition - contact.point;
        if (VectorMath.IsVectorFacingSameDirection(collisionNormal, wrongDirection))
            collisionNormal *= -1;
        
        ActivateFx(otherElementPropertyContainer, contact.point, collisionNormal);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isMine) return;
        if (!_isActive) return;
        if (!elementFxObjectPool.IsInitialized) return;
        
        var otherElementPropertyContainer = other.gameObject.GetComponent<ElementPropertyContainer>();
        if (!otherElementPropertyContainer) return;
        if (!otherElementPropertyContainer._isMine) return;

        var closestPointOnBounds = other.ClosestPointOnBounds(_thisTransform.position);
        var collisionNormal = closestPointOnBounds.normalized;

        var otherTransformPosition = other.transform.position;
        var wrongDirection = elementProperty.emitsOut ? closestPointOnBounds - otherTransformPosition : otherTransformPosition - closestPointOnBounds;
        if (VectorMath.IsVectorFacingSameDirection(collisionNormal, wrongDirection))
            collisionNormal *= -1;
        
        ActivateFx(otherElementPropertyContainer, closestPointOnBounds, collisionNormal);
    }

    [Button]
    public void PopulateParameters()
    {
        if (!physicsObject)
        {
            physicsObject = GetComponent<PhysicsObject>();
            if (!physicsObject)
            {
                physicsObject = gameObject.AddComponent<PhysicsObject>();
            }
        }
        
        physicsObject.PopulateParameters();

        if (!elementFxObjectPool)
        {
            var objectPools = Resources.FindObjectsOfTypeAll<ObjectPool>();
            foreach (var objectPool in objectPools)
            {
                if (objectPool.name.ToLower().Contains(ELEMENT_FX_CONTAINER))
                {
                    elementFxObjectPool = objectPool;
                    return;
                }
            }
        }
    }
    
    public Element GetElement()
    {
        if (DebugLogger.IsNullError(MethodBase.GetCurrentMethod().Name, this, "Must be set in editor.")) return null;

        return elementProperty.element;
    }

    private PhysicsObject GetPhysicsObject()
    {
        return physicsObject;
    }

    public void ActivateFx(ElementPropertyContainer otherElementPropertyContainer, Vector3 point, Vector3 normal)
    {
        if (!_isMine) return;
        if (!otherElementPropertyContainer._isMine) return;
        
        if (!otherElementPropertyContainer || !elementProperty) return;

        var otherElement = otherElementPropertyContainer.GetElement();
        var effectiveness = elementProperty.GetEffectiveness(otherElement);
        _effectivenessDictionary[effectiveness].Invoke(otherElementPropertyContainer, point, normal);
    }
    
    private void ActivateIneffectiveFx(ElementPropertyContainer otherElementPropertyContainer, Vector3 point, Vector3 normal)
    {
        if (IsMinVelocityMet(otherElementPropertyContainer, out var elementFxContainer, out var velocityToUse)) return;

        var rotation = Quaternion.LookRotation(normal);
        elementFxContainer.Spawn(rotation, point, true);
        elementFxContainer.PlayFx(elementProperty, Element.Effectiveness.Ineffective, velocityToUse, physicsObject.GetSize());
    }

    private void ActivateNormalFx(ElementPropertyContainer otherElementPropertyContainer, Vector3 point, Vector3 normal)
    {
        if (IsMinVelocityMet(otherElementPropertyContainer, out var elementFxContainer, out var velocityToUse)) return;

        var rotation = Quaternion.LookRotation(normal);
        elementFxContainer.Spawn(rotation, point, true);
        elementFxContainer.PlayFx(elementProperty, Element.Effectiveness.Normal, velocityToUse, physicsObject.GetSize());
    }

    private void ActivateEffectiveFx(ElementPropertyContainer otherElementPropertyContainer, Vector3 point, Vector3 normal)
    {
        if (IsMinVelocityMet(otherElementPropertyContainer, out var elementFxContainer, out var velocityToUse)) return;

        var rotation = Quaternion.LookRotation(normal);
        elementFxContainer.Spawn(rotation, point, true);
        elementFxContainer.PlayFx(elementProperty, Element.Effectiveness.Effective, velocityToUse, physicsObject.GetSize());
    }
    
    public bool IsMinVelocityMet(ElementPropertyContainer otherElementPropertyContainer, out ElementFxContainer elementFxContainer,
        out float velocityToUse)
    {
        elementFxContainer = null;
        velocityToUse = -1;

        if (DebugLogger.IsNullError(elementFxObjectPool, this, "Must be set in editor.")) return true;

        var pooledObject = elementFxObjectPool.GetPooledObject();
        if (DebugLogger.IsNullError(pooledObject, this)) return true;

        elementFxContainer = (ElementFxContainer) pooledObject;
        if (DebugLogger.IsNullError(elementFxContainer, this)) return true;

        var isKinematic = physicsObject.GetIsKinematic();
        var otherIsKinematic = otherElementPropertyContainer.physicsObject.GetIsKinematic();
        var velocity = physicsObject.GetSpeed();
        var otherVelocity = otherElementPropertyContainer.GetPhysicsObject().GetSpeed();
        
        velocityToUse = isKinematic ? otherVelocity : velocity;
        if (isKinematic && otherIsKinematic)
        {
            var minVelocity = elementFxContainer.GetMinVelocity(elementProperty);
            velocityToUse = velocity > minVelocity ? velocity : minVelocity;
        }
        else if (!elementFxContainer || !elementFxContainer.MeetsMinVelocityThreshold(elementProperty, velocityToUse))
        {
            return true;
        }

        return false;
    }
    
    public void Activate()
    {
        _isActive = true;
    }

    public void Deactivate()
    {
        _isActive = false;
    }
}
