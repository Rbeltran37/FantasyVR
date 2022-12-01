using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    public Transform headset;
    public Transform playArea;
    public Transform menuParent;
    public GameObject menu;
    public GameObject controller;
    public float distanceFromPlayer = 3;
    public float menuHeight = 2;
    
    
    #region UI_TEXT

    [Header("UI Text")]
    public Text leftWaistPowerText;
    public Text rightWaistPowerText;
    public Text leftBackPowerText;
    public Text rightBackPowerText;
    public Text collisionThresholdText;
    public Text collisionResistanceText;
    public Text regainPinSpeedText;
    public Text knockoutDistanceText;
    public Text unpinnedMuscleWeightText;
    public Text pinWeightThresholdText;
    public Text lifeText;
    public Text headHitDamageText;
    public Text bodyHitDamageText;
    public Text hitBufferText;
    public Text minHeadImpulseText;
    public Text handSpeedText;
    public Text handUseBlockingText;
    public Text handBlockRecoveryTimeText;
    public Text handHitVelocityThresholdText;
    public Text handHitCooldownText;
    public Text handSwingVelocityThresholdText;
    public Text handSwingCooldownText;
    public Text handMaxImpactVelocityText;
    public Text handHeadDamageText;
    public Text handBodyDamageText;
    public Text handMinAmplitudeText;
    public Text handMinDurationText;
    public Text tunnelIsOn;
    public Text tunnelEffectCoverageText;
    public Text tunnelEffectFeatherText;
    public Text angularStrengthText;
    public Text angularMinText;
    public Text angularMaxText;
    public Text angularSmoothingText;
    public Text linearStrengthText;
    public Text linearMinText;
    public Text linearMaxText;
    public Text linearSmoothingText;
    public Text stickIsLeftHandText;
    public Text stickUseHeadsetForwardText;
    public Text stickSpeedText;
    public Text smoothIsLeftHandText;
    public Text smoothSpeedText;
    public Text combatMaxDashDistanceText;
    public Text combatSpeedText;
    public Text combatSphereRadiusText;
    public Text combatSetDistanceText;
    public Text combatAgentStunTimeText;
    public Text combatStunOnStartText;
    public Text forcePushRangeText;
    public Text forcePushRadiusText;
    public Text forcePushForceText;
    public Text forcePushCapText;
    public Text forcePushHoverText;
    public Text forcePushUnpinText;
    public Text forcePullRangeText;
    public Text forcePullRadiusText;
    public Text forcePullSpeedText;
    public Text forcePullSetPositionText;
    public Text forceRepulseRadiusText;
    public Text forceRepulseForceText;
    public Text forceRepulseCapText;
    public Text forceRepulseHoverText;
    public Text forceRepulseUnpinText;
    public Text grabRangeText;
    public Text grabRadiusText;
    public Text grabSpeedText;
    public Text grabDistanceLimitText;
    public Text waistDistanceFromHeadsetText;
    public Text waistDistanceFromCenterText;
    public Text backDistanceFromHeadsetText;
    public Text backDistanceFromCenterText;
    public Text hapticAmplitudeText;
    public Text hapticDurationText;
    public Text unequipOnReleaseText;
    public Text kiBlastLifetimeText;
    public Text kiBlastSphereRadiusText;
    public Text kiBlastSpherecastDistanceText;
    public Text kiBlastSpeedText;
    public Text kiBlastTurnRateText;
    public Text kiBlastBodyDamageText;
    public Text kiBlastHeadDamageText;
    public Text kiBlastForceText;
    public Text busterSmallChargeText;
    public Text busterMediumChargeText;
    public Text busterLargeChargeText;
    public Text busterShotForceText;
    public Text busterLifetimeText;
    public Text noChargeBodyDamageText;
    public Text noChargeHeadDamageText;
    public Text noChargeForceText;
    public Text noChargeNumHitsText;
    public Text smallChargeBodyDamageText;
    public Text smallChargeHeadDamageText;
    public Text smallChargeForceText;
    public Text smallChargeNumHitsText;
    public Text mediumChargeBodyDamageText;
    public Text mediumChargeHeadDamageText;
    public Text mediumChargeForceText;
    public Text mediumChargeNumHitsText;
    public Text largeChargeBodyDamageText;
    public Text largeChargeHeadDamageText;
    public Text largeChargeForceText;
    public Text largeChargeNumHitsText;
    public Text kamehamehaMinBodyDamageText;
    public Text kamehamehaMinHeadDamageText;
    public Text kamehamehaMaxBodyDamageText;
    public Text kamehamehaMaxHeadDamageText;
    public Text kamehamehaMinForceText;
    public Text kamehamehaMaxForceText;
    public Text kamehamehaMinChargeSphereRadiusText;
    public Text kamehamehaMaxChargeSphereRadiusText;
    public Text kamehamehaMinBeamRadiusText;
    public Text kamehamehaMaxBeamRadiusText;
    public Text kamehamehaMinExplosionRadiusText;
    public Text kamehamehaMaxExplosionRadiusText;
    public Text kamehamehaMinChargeTimeText;
    public Text kamehamehaMaxChargeTimeText;
    public Text kamehamehaLaunchForceText;
    public Text kamehamehaLifetimeText;
    public Text kamehamehaExplosionTimeText;
    public Text finalFlashRangeText;
    public Text finalFlashRadiusText;
    public Text finalFlashHeadDamageText;
    public Text finalFlashBodyDamageText;
    public Text finalFlashHitForceText;
    public Text finalFlashChargeTimeText;
    public Text finalFlashDurationText;
    public Text fireBombThrowForceMultiplierText;
    public Text fireBombBodyDamageText;
    public Text fireBombHeadDamageText;
    public Text fireBombHitForceText;
    public Text fireBombExplosionRadiusText;
    public Text fireBombLifetimeText;
    public Text fireFistAddedDamageText;
    public Text fireFistDurationText;
    public Text fireBreathFlameSpawnTimeText;
    public Text fireBreathFlameLaunchForceText;
    public Text fireBreathFlameColliderRadiusText;
    public Text fireBreathHeadDamageText;
    public Text fireBreathBodyDamageText;
    public Text fireBreathHitForceText;
    public Text fireBreathLifetimeText;
    public Text combustionColliderRadiusText;
    public Text combustionHeadDamageText;
    public Text combustionBodyDamageText;
    public Text combustionHitForceText;
    public Text combustionLifetimeText;

    #endregion UI_TEXT



    // Start is called before the first frame update
    void Start()
    {
        menu.SetActive(false);
        controller.SetActive(false);
    }

    private void OnEnable() {

        menu.SetActive(true);
        controller.SetActive(true);

        menuParent.transform.position = headset.position;
        menuParent.transform.position += headset.forward * distanceFromPlayer;
        menuParent.transform.position = new Vector3(menuParent.transform.position.x, menuHeight + playArea.position.y, menuParent.transform.position.z);

        menuParent.transform.LookAt(headset);
        menuParent.transform.localEulerAngles = new Vector3(0, menuParent.transform.localEulerAngles.y, 0);
    }

    private void OnDisable() {
        menu.SetActive(false);
        if (controller)
            controller.SetActive(false);
    }

    public void toggleMenu() {
        gameObject.SetActive(!gameObject.activeSelf);
    }
    
    /*
    private void updateText() {

        collisionThresholdText.text = "collision Threshold: " + collisionThreshold;
        collisionResistanceText.text = "Collision Resistance: " + collisionResistance;
        regainPinSpeedText.text = "regain Pin Speed: " + regainPinSpeed;
        knockoutDistanceText.text = "knockout Distance: " + knockoutDistance;
        unpinnedMuscleWeightText.text = "unpinned Muscle Weight: " + unpinnedMuscleWeight;
        pinWeightThresholdText.text = "pin Weight Threshold: " + pinWeightThreshold;

        lifeText.text = "life: " + life;
        headHitDamageText.text = "headHit Damage: " + headHitDamage;
        bodyHitDamageText.text = "bodyHit Damage: " + bodyHitDamage;
        hitBufferText.text = "hit Buffer: " + hitBuffer;
        minHeadImpulseText.text = "minHead Impulse: " + minHeadImpulse;

        handSpeedText.text = "Speed: " + handSpeed;
        handUseBlockingText.text = "Use Blocking: " + handUseBlocking;
        handBlockRecoveryTimeText.text = "Block Recovery Time: " + handBlockRevoveryTime;

        handHitVelocityThresholdText.text = "Hit Velocity Threshold: " + handHitVelocityThreshold;
        handHitCooldownText.text = "Hit Cooldown: " + handHitCooldown;
        handSwingVelocityThresholdText.text = "Swing Velocity Threshold: " + handSwingVelocityThreshold;
        handSwingCooldownText.text = "Swing Velocity Cooldown: " + handSwingCooldown;
        handMaxImpactVelocityText.text = "Max Impact Velocity: " + handMaxImpactVelocity;
        handHeadDamageText.text = "Head Damage: " + handHeadDamage;
        handBodyDamageText.text = "Body Damage: " + handBodyDamage;

        handMinAmplitudeText.text = "Min Amplitude: " + handMinAmplitude;
        handMinDurationText.text = "Min Duration: " + handMinDuration;

        tunnelIsOn.text = "Use Tunnelling: " + tunnellingBase.gameObject.activeSelf;
        tunnelEffectCoverageText.text = "Effect Coverage: " + tunnelEffectCoverage;
        tunnelEffectFeatherText.text = "Effect Feather: " + tunnelEffectFeather;
        angularStrengthText.text = "Angular Strength: " + angularStrength;
        angularMinText.text = "Angular Min: " + angularMin;
        angularMaxText.text = "Angular Max: " + angularMax;
        angularSmoothingText.text = "Angular Smoothing: " + angularSmoothing;
        linearStrengthText.text = "Linear Strength: " + linearStrength;
        linearMinText.text = "Linear Min: " + linearMin;
        linearMaxText.text = "Linear Max: " + linearMax;
        linearSmoothingText.text = "Linear Smoothing: " + linearSmoothing;

        stickIsLeftHandText.text = "Is Left Hand:: " + stickIsLeftHand;
        stickUseHeadsetForwardText.text = "Use Headset Forward: " + stickUseHeadsetForward;
        stickSpeedText.text = "Speed: " + stickSpeed;

        smoothIsLeftHandText.text = "Is Left Hand: " + smoothIsLeftHand;
        smoothSpeedText.text = "Speed: " + smoothSpeed;

        combatMaxDashDistanceText.text = "Max Dash Distance: " + combatMaxDashDistance;
        combatSpeedText.text = "Speed: " + combatSpeed;
        combatSphereRadiusText.text = "Sphere Radius: " + combatSphereRadius;
        combatSetDistanceText.text = "Set Distance: " + combatSetDistance;
        combatAgentStunTimeText.text = "Agent Stun Time: " + combatAgentStunTime;
        combatStunOnStartText.text = "Stun On Start: " + combatStunOnStart;

        forcePushRangeText.text = "Range: " + forcePushRange;
        forcePushRadiusText.text = "Radius: " + forcePushRadius;
        forcePushForceText.text = "Force: " + forcePushForce;
        forcePushCapText.text = "Cap: " + forcePushCap;
        forcePushHoverText.text = "Hover: " + forcePushHover;
        forcePushUnpinText.text = "Unpin: " + forcePushUnpin;

        forcePullRangeText.text = "Range: " + forcePullRange;
        forcePullRadiusText.text = "Radius: " + forcePullRadius;
        forcePullSpeedText.text = "Speed: " + forcePullSpeed;
        forcePullSetPositionText.text = "Set Position: " + forcePullSetPosition;

        forceRepulseRadiusText.text = "Radius: " + forceRepulseRadius;
        forceRepulseForceText.text = "Force: " + forceRepulseForce;
        forceRepulseCapText.text = "Cap: " + forceRepulseCap;
        forceRepulseHoverText.text = "Hover: " + forceRepulseHover;
        forceRepulseUnpinText.text = "Unpin: " + forceRepulseUnpin;

        grabRangeText.text = "Range: " + grabRange;
        grabRadiusText.text = "Radius: " + grabRadius;
        grabSpeedText.text = "Speed: " + grabSpeed;
        grabDistanceLimitText.text = "Distance Limit: " + grabDistanceLimit;

        waistDistanceFromHeadsetText.text = "Waist Distance: " + waistDistanceFromHeadset;
        waistDistanceFromCenterText.text = "Waist Off Center: " + waistDistanceFromCenter;
        backDistanceFromHeadsetText.text = "Back Distance: " + backDistanceFromHeadset;
        backDistanceFromCenterText.text = "Back Off Center: " + backDistanceFromCenter;
        hapticAmplitudeText.text = "Haptic Amplitude: " + hapticAmplitude;
        hapticDurationText.text = "Haptic Duration: " + hapticDuration;
        unequipOnReleaseText.text = "Unequip On Release: " + unequipOnRelease;

        kiBlastLifetimeText.text = "Lifetime: " + kiBlastLifetime;
        kiBlastSphereRadiusText.text = "Sphere Radius: " + kiBlastSphereRadius;
        kiBlastSpherecastDistanceText.text = "Spherecast Dist: " + kiBlastSpherecastDistance;
        kiBlastSpeedText.text = "Speed: " + kiBlastSpeed;
        kiBlastTurnRateText.text = "Turn: " + kiBlastTurnRate;
        kiBlastBodyDamageText.text = "Body Dmg: " + kiBlastBodyDamage;
        kiBlastHeadDamageText.text = "Head Dmg: " + kiBlastHeadDamage;
        kiBlastForceText.text = "Force: " + kiBlastForce;

        busterSmallChargeText.text = "Small Charge: " + busterSmallCharge;
        busterMediumChargeText.text = "Medium Charge: " + busterMediumCharge;
        busterLargeChargeText.text = "Large Charge: " + busterLargeCharge;
        busterShotForceText.text = "Shot Force: " + busterShotForce;
        busterLifetimeText.text = "Lifetime: " + busterLifetime;
        noChargeBodyDamageText.text = "No Charge Body Dmg: " + noChargeBodyDamage;
        noChargeHeadDamageText.text = "No Charge Head Dmg: " + noChargeHeadDamage;
        noChargeForceText.text = "No Charge Force: " + noChargeForce;
        noChargeNumHitsText.text = "No Charge Num Hits: " + noChargeNumHits;
        smallChargeBodyDamageText.text = "Small Charge Body Dmg: " + smallChargeBodyDamage;
        smallChargeHeadDamageText.text = "Small Charge Head Dmg: " + smallChargeHeadDamage;
        smallChargeForceText.text = "Small Charge Force: " + smallChargeForce;
        smallChargeNumHitsText.text = "Small Charge Num Hits: " + smallChargeNumHits;
        mediumChargeBodyDamageText.text = "Med Charge Body Dmg: " + mediumChargeBodyDamage;
        mediumChargeHeadDamageText.text = "Med Charge Head Dmg: " + mediumChargeHeadDamage;
        mediumChargeForceText.text = "Med Charge Force: " + mediumChargeForce;
        mediumChargeNumHitsText.text = "Med Charge Num Hits: " + mediumChargeNumHits;
        largeChargeBodyDamageText.text = "Lrg Charge Body Dmg: " + largeChargeBodyDamage;
        largeChargeHeadDamageText.text = "Lrg Charge Head Dmg: " + largeChargeHeadDamage;
        largeChargeForceText.text = "Lrg Charge Force: " + largeChargeForce;
        largeChargeNumHitsText.text = "Lrg Charge Num Hits: " + largeChargeNumHits;

        kamehamehaMinBodyDamageText.text = "Min Body Dmg: " + kamehamehaMinBodyDamage;
        kamehamehaMinHeadDamageText.text = "Min Head Dmg: " + kamehamehaMinHeadDamage;
        kamehamehaMaxBodyDamageText.text = "Max Body Dmg: " + kamehamehaMaxBodyDamage;
        kamehamehaMaxHeadDamageText.text = "Max Head Dmg: " + kamehamehaMaxHeadDamage;
        kamehamehaMinForceText.text = "Min Force: " + kamehamehaMinForce;
        kamehamehaMaxForceText.text = "Max Force: " + kamehamehaMaxForce;
        kamehamehaMinChargeSphereRadiusText.text = "Min Charge Sphere: " + kamehamehaMinChargeSphereRadius;
        kamehamehaMaxChargeSphereRadiusText.text = "Max Charge Sphere: " + kamehamehaMaxChargeSphereRadius;
        kamehamehaMinBeamRadiusText.text = "Min Beam Radius: " + kamehamehaMinBeamRadius;
        kamehamehaMaxBeamRadiusText.text = "Max Beam Radius: " + kamehamehaMaxBeamRadius;
        kamehamehaMinExplosionRadiusText.text = "Min Expl Radius: " + kamehamehaMinExplosionRadius;
        kamehamehaMaxExplosionRadiusText.text = "Max Expl Radius: " + kamehamehaMaxExplosionRadius;
        kamehamehaMinChargeTimeText.text = "Min Charge Time: " + kamehamehaMinChargeTime;
        kamehamehaMaxChargeTimeText.text = "Max Charge Time: " + kamehamehaMaxChargeTime;
        kamehamehaLaunchForceText.text = "Launch Force: " + kamehamehaLaunchForce;
        kamehamehaLifetimeText.text = "Lifetime: " + kamehamehaLifetime;
        kamehamehaExplosionTimeText.text = "Explosion Time: " + kamehamehaExplosionTime;

        finalFlashRangeText.text = "Range: " + finalFlashRange;
        finalFlashRadiusText.text = "Radius: " + finalFlashRadius;
        finalFlashHeadDamageText.text = "Head Damage: " + finalFlashHeadDamage;
        finalFlashBodyDamageText.text = "Body Damage: " + finalFlashBodyDamage;
        finalFlashHitForceText.text = "Hit Force: " + finalFlashHitForce;
        finalFlashChargeTimeText.text = "Charge Time: " + finalFlashChargeTime;
        finalFlashDurationText.text = "Duration: " + finalFlashDuration;

        fireBombThrowForceMultiplierText.text = "Throw Force Multiplier: " + fireBombThrowForceMultiplier;
        fireBombBodyDamageText.text = "Body Damage: " + fireBombBodyDamage;
        fireBombHeadDamageText.text = "Head Damage: " + fireBombHeadDamage;
        fireBombHitForceText.text = "Hit Force: " + fireBombHitForce;
        fireBombExplosionRadiusText.text = "Explosion Radius: " + fireBombExplosionRadius;
        fireBombLifetimeText.text = "Lifetime: " + fireBombLifetime;

        fireFistAddedDamageText.text = "Added Damage: " + fireFistAddedDamage;
        fireFistDurationText.text = "Duration: " + fireFistDuration;

        fireBreathFlameSpawnTimeText.text = "Spawn Time: " + fireBreathFlameSpawnTime;
        fireBreathFlameLaunchForceText.text = "Launch Force: " + fireBreathFlameLaunchForce;
        fireBreathFlameColliderRadiusText.text = "Collider Radius: " + fireBreathFlameColliderRadius;
        fireBreathHeadDamageText.text = "Head Damage: " + fireBreathHeadDamage;
        fireBreathBodyDamageText.text = "Body Damage: " + fireBreathBodyDamage;
        fireBreathHitForceText.text = "Hit Force: " + fireBreathHitForce;
        fireBreathLifetimeText.text = "Lifetime: " + fireBreathLifetime;

        combustionColliderRadiusText.text = "Collider Radius: " + combustionColliderRadius;
        combustionHeadDamageText.text = "Head Damage: " + combustionHeadDamage;
        combustionBodyDamageText.text = "Body Damage: " + combustionBodyDamage;
        combustionHitForceText.text = "Hit Force: " + combustionHitForce;
        combustionLifetimeText.text = "Lifetime: " + combustionLifetime;


        leftWaistPowerText.text = "Left Waist: " + powerSelector.getPowerString(currentLeftWaist);
        rightWaistPowerText.text = "Right Waist: " + powerSelector.getPowerString(currentRightWaist);
        leftBackPowerText.text = "Left Back: " + powerSelector.getPowerString(currentLeftBack);
        rightBackPowerText.text = "Right Back: " + powerSelector.getPowerString(currentRightBack);
    }
    */
}
