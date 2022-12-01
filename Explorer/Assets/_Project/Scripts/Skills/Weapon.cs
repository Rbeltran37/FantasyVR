using System;
using System.Collections;
using UnityEngine;

public abstract class Weapon : Skill
{
    [SerializeField] private WeaponSO weaponSo;

    protected WeaponInstance WeaponInstance;
    
    protected Transform PuppetGrabTransform;
    protected Rigidbody PuppetGrabRigid;

    private ControllerHaptics _controllerHaptics;
    private PhysicsDamageDealt _physicsDamageDealt;
    private ManaRecover _manaRecover;
    private ElementPropertyContainer _elementPropertyContainer;
    

    public override void Setup(PlayerHand playerHand, ManaPool manaPool)
    {
        if (DebugLogger.IsNullError(playerHand, this)) return;
        if (DebugLogger.IsNullError(weaponSo, this)) return;
        if (DebugLogger.IsNullError(weaponSo.weaponPrefab, this)) return;
        if (DebugLogger.IsNullError(InstanceObjectPool, this)) return;

        base.Setup(playerHand, manaPool);

        var weaponReferenceHelper = weaponSo.weaponPrefab.GetComponent<WeaponReferenceHelper>();
        if (DebugLogger.IsNullError(weaponReferenceHelper, this)) return;

        SetGrabTransformAndRigid(playerHand);

        GetWeaponReferenceHelperValues(weaponReferenceHelper);
        _controllerHaptics = playerHand.GetControllerHaptics();

        InstanceObjectPool.InitializePool();
    }

    protected virtual void SetGrabTransformAndRigid(PlayerHand playerHand)
    {
        PuppetGrabTransform = playerHand.GetPuppetHandTransform();
        PuppetGrabRigid = playerHand.GetPuppetHandRigidbody();
    }

    public override void Equip()
    {
        base.Equip();
        
        if (DebugLogger.IsNullError(weaponSo, this)) return;
        if (DebugLogger.IsNullError(weaponSo.weaponPrefab, this)) return;

        SetCurrentInstance();
        if (DebugLogger.IsNullError(WeaponInstance, this)) return;
        
        var weaponReferenceHelper = WeaponInstance.GetWeaponReferenceHelper();
        if (DebugLogger.IsNullError(weaponReferenceHelper, this)) return;

        SetupHandAndJoint(weaponReferenceHelper);
        SetupWeaponFeedback(weaponReferenceHelper);
        SetHandScripts();

        weaponReferenceHelper.weaponFeedback.controllerHaptics = _controllerHaptics;
        
        WeaponInstance.SetSkill(this);
        WeaponInstance.Spawn();

        DeactivateHandScripts();
    }
    
    protected void DeactivateHandScripts()
    {
        if (_physicsDamageDealt) _physicsDamageDealt.enabled = false;
        if (_manaRecover) _manaRecover.Deactivate();
        if (_elementPropertyContainer) _elementPropertyContainer.Deactivate();
    }

    public override void UnEquip()
    {
        base.UnEquip();
        
        if (!WeaponInstance) return;
        
        WeaponInstance.Despawn();

        ActivateHandScripts();
    }
    
    protected void ActivateHandScripts()
    {
        if (_physicsDamageDealt) _physicsDamageDealt.enabled = true;
        if (_manaRecover) _manaRecover.Activate();
        if (_elementPropertyContainer) _elementPropertyContainer.Activate();
    }

    public override void UseAbility()
    {
        if (!WeaponInstance) return;
        
        WeaponInstance.UseAbility();
    }

    private void GetWeaponReferenceHelperValues(WeaponReferenceHelper weaponReferenceHelper)
    {
        var weaponFeedback = weaponReferenceHelper.weaponFeedback;
        if (weaponFeedback)
        {
            weaponFeedback.weaponRigidbody = PuppetGrabRigid;
        }

        var weaponSetupHandler = weaponReferenceHelper.weaponSetupHandler;
        if (weaponSetupHandler)
        {
            weaponSetupHandler.SetPuppetHandRigid(PuppetGrabRigid);
        }
    }

    private void SetupWeaponFeedback(WeaponReferenceHelper weaponReferenceHelper)
    {
        var weaponFeedback = weaponReferenceHelper.weaponFeedback;
        if (weaponFeedback)
        {
            weaponFeedback.weaponRigidbody = PuppetGrabRigid;
        }
    }

    private void SetCurrentInstance()
    {
        if (DebugLogger.IsNullError(InstanceObjectPool, this, "Must be set in editor.")) return;
        
        var pooledObject = InstanceObjectPool.GetPooledObject();
        if (DebugLogger.IsNullError(pooledObject, this)) return;

        WeaponInstance = (WeaponInstance) pooledObject;
        if (DebugLogger.IsNullError(WeaponInstance, this, "Must be set in editor.")) return;
    }

    private void SetupHandAndJoint(WeaponReferenceHelper weaponReferenceHelper)
    {
        if (DebugLogger.IsNullError(weaponReferenceHelper, this)) return;
        
        var prefabJoint = weaponReferenceHelper.configurableJoint;
        if (prefabJoint)
        {
            prefabJoint.connectedBody = PuppetGrabRigid;
            prefabJoint.autoConfigureConnectedAnchor = false;
        }
        
        weaponReferenceHelper.SetupHandAndJoint(PuppetGrabTransform, PuppetGrabRigid, PlayerHand.IsLeftHand());
    }

    private void SetHandScripts()
    {
        _physicsDamageDealt = PuppetGrabRigid.GetComponent<PhysicsDamageDealt>();
        _manaRecover = PuppetGrabRigid.GetComponent<ManaRecover>();
        _elementPropertyContainer = PuppetGrabRigid.GetComponent<ElementPropertyContainer>();
    }
}