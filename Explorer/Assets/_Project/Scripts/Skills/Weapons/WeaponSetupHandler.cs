using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

public class WeaponSetupHandler : MonoBehaviour
{
    [SerializeField] private WeaponSO weaponSo;
    [SerializeField] private Transform thisTransform;
    
    [SerializeField] protected ConfigurableJoint WeaponConfigurableJoint;

    protected Rigidbody PuppetHandRigid;
    
    protected bool IsLeft;

    private float _defaultPuppetHandMass = 1.98f;

    private const float SETUP_DELAY = .1f;

    private void Awake()
    {
        if (DebugLogger.IsNullWarning(thisTransform, this, "Should be set in inspector."))
        {
            thisTransform = transform;
        }
    }

    protected virtual void OnEnable()
    {
        WeaponPlacement(PuppetHandRigid, IsLeft);
    }

    public virtual void WeaponPlacement(Rigidbody puppetHandRigid, bool isLeft)
    {
        gameObject.SetActive(true);
        CoroutineCaller.Instance.StartCoroutine(SetupHandAndJointCoroutine(puppetHandRigid, isLeft));
    }

    private void OnDestroy()
    {
        if (PuppetHandRigid)
        {
            PuppetHandRigid.mass = _defaultPuppetHandMass;
        }
    }
    
    public void SetupHandAndJoint(Transform puppetHandTransform, Rigidbody puppetHandRigid, ConfigurableJoint weaponConfigurableJoint, bool isLeft)
    {
        if (DebugLogger.IsNullError(puppetHandRigid, this)) return;
        if (DebugLogger.IsNullWarning(thisTransform, this, "Should be set in inspector."))
        {
            thisTransform = transform;
        }

        SetPuppetHandRigid(puppetHandRigid);
        SetWeaponParent(puppetHandTransform);
        _defaultPuppetHandMass = puppetHandRigid.mass;
        this.WeaponConfigurableJoint = weaponConfigurableJoint;
        IsLeft = isLeft;
        
        DisconnectJoint(puppetHandRigid);
        SetPositionRotationAndScale(isLeft);
    }

    private void DisconnectJoint(Rigidbody puppetHandRigid)
    {
        if (!WeaponConfigurableJoint) return;
        
        WeaponConfigurableJoint.connectedBody = puppetHandRigid;
        WeaponConfigurableJoint.autoConfigureConnectedAnchor = false;
    }

    private void SetPositionRotationAndScale(bool isLeft)
    {
        var xPosition = weaponSo.localPosition.x;
        var yPosition = weaponSo.localPosition.y;
        var zPosition = weaponSo.localPosition.z;
        var xEuler = weaponSo.localEuler.x;
        var yEuler = weaponSo.localEuler.y;
        var zEuler = weaponSo.localEuler.z;
        if (isLeft)
        {
            xPosition *= -1;
            yEuler *= -1;
            zEuler *= -1;
        }

        thisTransform.localPosition = new Vector3(xPosition, yPosition, zPosition);
        thisTransform.localEulerAngles = new Vector3(xEuler, yEuler, zEuler);

        var localScale = thisTransform.localScale;
        var localScaleX = localScale.x;
        if (isLeft)    //negative x on localScale to flip the model, if used by left hand
        {
            if (weaponSo.isDefaultLocalXScalePositive)
            {
                localScaleX = -Math.Abs(localScaleX);
            }
            else
            {
                localScaleX = Math.Abs(localScaleX);
            }
        }
        else
        {
            if (weaponSo.isDefaultLocalXScalePositive)
            {
                localScaleX = Math.Abs(localScaleX);
            }
            else
            {
                localScaleX = -Math.Abs(localScaleX);
            }
        }
        
        thisTransform.localScale = new Vector3(localScaleX, localScale.y, localScale.z);

        //Properly sets shield
        if (WeaponConfigurableJoint)
        {
            WeaponConfigurableJoint.anchor = isLeft ? thisTransform.localPosition : new Vector3(-xPosition, yPosition, zPosition);
        }
    }

    public void SetPuppetHandRigid(Rigidbody handRigid)
    {
        PuppetHandRigid = handRigid;
    }

    private void SetWeaponParent(Transform handTransform)
    {
        thisTransform.SetParent(handTransform);
    }

    private IEnumerator SetupHandAndJointCoroutine(Rigidbody puppetHandRigid, bool isLeft)
    {
        SetPuppetHandRigid(puppetHandRigid);
        IsLeft = isLeft;
        
        yield return new WaitForEndOfFrame();
        if (PuppetHandRigid)
        {
            PuppetHandRigid.mass = weaponSo.weaponRigidMass;
        }
        
        yield return new WaitForSeconds(SETUP_DELAY);

        DisconnectJoint(puppetHandRigid);
        SetWeaponParent(puppetHandRigid.transform);
        SetPositionRotationAndScale(isLeft);
        ConnectWeaponJointToHand(puppetHandRigid);
    }

    private void ConnectWeaponJointToHand(Rigidbody puppetHandRigid)
    {
        if (!WeaponConfigurableJoint) return;
        
        WeaponConfigurableJoint.connectedBody = puppetHandRigid;
        WeaponConfigurableJoint.autoConfigureConnectedAnchor = true;
    }
}
