using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BehaviorDesigner.Runtime;
using Photon.Pun;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemySetup : MonoBehaviour
{
    [SerializeField] private PhotonView photonView;
    [SerializeField] private EnemySetupSO enemySetupSo;
    [SerializeField] private Animator animator;
    [SerializeField] private BehaviorTree behaviorTree;
    [SerializeField] private SimpleAI simpleAi;
    [SerializeField] private CustomUserControlAI customUserControlAi;
    [SerializeField] private EnemyTargetAcquisition enemyTargetAcquisition;
    [SerializeField] private DropGearHandler dropGearHandler;
    [SerializeField] private AnimationEventHandler animationEventHandler;
    [SerializeField] private AimIK weaponAimIkLeft;
    [SerializeField] private AimIK weaponAimIkRight;
    [SerializeField] private EnemyWeaponInstance[] leftEnemyWeaponInstances;
    [SerializeField] private EnemyWeaponInstance[] rightEnemyWeaponInstances;

    private bool _isInitialized = false;
    private EnemyWeaponInstance _primaryLeft;
    private EnemyWeaponInstance _primaryRight;
    private EnemyWeaponInstance _secondaryLeft;
    private EnemyWeaponInstance _secondaryRight;
    private Dictionary<EnemyWeaponSO, EnemyWeaponInstance> _leftEnemyWeapons = new Dictionary<EnemyWeaponSO, EnemyWeaponInstance>();
    private Dictionary<EnemyWeaponSO, EnemyWeaponInstance> _rightEnemyWeapons = new Dictionary<EnemyWeaponSO, EnemyWeaponInstance>();

    private const string SIMPLE_AI = "SimpleAI";
    private const string CUSTOM_USER_CONTROL_AI = "CustomUserControlAI";
    private const string ENEMY_TARGET_ACQUISITION = "EnemyTargetAcquisition";
    private const string ANIMATION_CONTROLLER = "AnimationController";


    private void Awake()
    {
        Initialize();
    }

    [Button]
    public void PopulateParameters()
    {
        if (photonView) photonView = GetComponent<PhotonView>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!behaviorTree) behaviorTree = GetComponentInChildren<BehaviorTree>();
        if (!simpleAi) simpleAi = GetComponentInChildren<SimpleAI>();
        if (!customUserControlAi) GetComponentInChildren<CustomUserControlAI>();
        if (!dropGearHandler)
        {
            dropGearHandler = GetComponentInChildren<DropGearHandler>();
        }

        if (!animationEventHandler) animationEventHandler = GetComponentInChildren<AnimationEventHandler>();
    }

    private void Initialize()
    {
        if (DebugLogger.IsNullError(behaviorTree, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(simpleAi, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(customUserControlAi, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(enemyTargetAcquisition, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(animator, this, "Must be set in editor.")) return;

        behaviorTree.SetVariable(SIMPLE_AI, (SharedGameObject) simpleAi.gameObject);
        behaviorTree.SetVariable(CUSTOM_USER_CONTROL_AI, (SharedGameObject) customUserControlAi.gameObject);
        behaviorTree.SetVariable(ENEMY_TARGET_ACQUISITION, (SharedGameObject) enemyTargetAcquisition.gameObject);
        behaviorTree.SetVariable(ANIMATION_CONTROLLER, (SharedGameObject) animator.gameObject);
    }

    private void InitializeDictionaries()
    {
        if (_isInitialized) return;

        _isInitialized = true;

        _leftEnemyWeapons = leftEnemyWeaponInstances.ToDictionary(key => key.EnemyWeaponSo, value => value);
        _rightEnemyWeapons = rightEnemyWeaponInstances.ToDictionary(key => key.EnemyWeaponSo, value => value);
        
        foreach (var enemyWeaponInstance in leftEnemyWeaponInstances)
        {
            enemyWeaponInstance.ThisGameObject.SetActive(false);
        }
        
        foreach (var enemyWeaponInstance in rightEnemyWeaponInstances)
        {
            enemyWeaponInstance.ThisGameObject.SetActive(false);
        }
    }

    [Button]
    public void Setup(EnemySetupSO otherEnemySetupSo)
    {
        this.enemySetupSo = otherEnemySetupSo;
        Clear();
        InitializeDictionaries();
        Setup();
    }
    
    private void Setup()
    {
        if (DebugLogger.IsNullError(enemySetupSo, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(behaviorTree, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(animationEventHandler, this, "Must be set in editor.")) return;
        
        if (animator == null) DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, $"animator is null.", this);
        
        if (DebugLogger.IsNullError(animator, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(animator.runtimeAnimatorController, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(enemySetupSo.externalBehaviorTree, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(enemySetupSo.runtimeAnimatorController, this, "Must be set in editor.")) return;

        animator.runtimeAnimatorController = enemySetupSo.runtimeAnimatorController;
        behaviorTree.ExternalBehavior = enemySetupSo.externalBehaviorTree;
        behaviorTree.BehaviorName = enemySetupSo.externalBehaviorTree.name;
        
        var primaryEnemyWeaponSetupSo = enemySetupSo.primaryEnemyWeaponSetupSo;
        SetupWeapon(primaryEnemyWeaponSetupSo, true);

        var secondaryEnemyWeaponSetupSo = enemySetupSo.secondaryEnemyWeaponSetupSo;
        if (secondaryEnemyWeaponSetupSo) SetupWeapon(secondaryEnemyWeaponSetupSo, false);
        
        animationEventHandler.PrimaryWasEquipped += SetPrimaryWeaponAimIk; //TODO make sure unsubscribing
        animationEventHandler.SecondaryWasEquipped += SetSecondaryWeaponAimIk; //TODO make sure unsubscribing
        animationEventHandler.EquipPrimaryWeapons();
    }

    private void SetupWeapon(EnemyWeaponSetupSO enemyWeaponSetupSo, bool isPrimary)
    {
        if (DebugLogger.IsNullError(enemyWeaponSetupSo, this)) return;

        if (enemyWeaponSetupSo.LeftEnemyWeaponSo)
        {
            if (!_leftEnemyWeapons.ContainsKey(enemyWeaponSetupSo.LeftEnemyWeaponSo))
            {
                DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, $"{nameof(_leftEnemyWeapons)} does not contain {enemyWeaponSetupSo.LeftEnemyWeaponSo}. Count={_leftEnemyWeapons.Count}");
                return;
            }
            
            var enemyWeaponInstanceLeft = _leftEnemyWeapons[enemyWeaponSetupSo.LeftEnemyWeaponSo];
            if (DebugLogger.IsNullError(enemyWeaponInstanceLeft, this)) return;
            
            dropGearHandler.Add(enemyWeaponInstanceLeft);

            var weaponInstanceGameObject = enemyWeaponInstanceLeft.ThisGameObject;
            animationEventHandler.SetLeftHitCollider(enemyWeaponInstanceLeft.DamageDealt);
            animationEventHandler.AddWeapon(weaponInstanceGameObject, isPrimary);
            
            if (isPrimary)
            {
                _primaryLeft = enemyWeaponInstanceLeft;
            }
            else
            {
                _secondaryLeft = enemyWeaponInstanceLeft;
            }

            CheckForBowSetup(enemyWeaponInstanceLeft);
            
            enemyWeaponInstanceLeft.ThisGameObject.SetActive(true);
        }
        
        if (enemyWeaponSetupSo.RightEnemyWeaponSo)
        {
            if (!_rightEnemyWeapons.ContainsKey(enemyWeaponSetupSo.RightEnemyWeaponSo))
            {
                DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, $"{nameof(_rightEnemyWeapons)} does not contain {enemyWeaponSetupSo.RightEnemyWeaponSo}");
                return;
            }
            
            var enemyWeaponInstanceRight = _rightEnemyWeapons[enemyWeaponSetupSo.RightEnemyWeaponSo];
            if (DebugLogger.IsNullError(enemyWeaponInstanceRight, this)) return;
            
            dropGearHandler.Add(enemyWeaponInstanceRight);

            var weaponInstanceGameObject = enemyWeaponInstanceRight.ThisGameObject;
            animationEventHandler.SetRightHitCollider(enemyWeaponInstanceRight.DamageDealt);
            animationEventHandler.AddWeapon(weaponInstanceGameObject, isPrimary);

            if (isPrimary)
            {
                _primaryRight = enemyWeaponInstanceRight;
            }
            else
            {
                _secondaryRight = enemyWeaponInstanceRight;
            }

            CheckForBowSetup(enemyWeaponInstanceRight);
            
            enemyWeaponInstanceRight.ThisGameObject.SetActive(true);
        }
        
        if (DebugLogger.IsNullInfo(dropGearHandler, this, "Must be set in editor.")) return;

        dropGearHandler.Setup();
    }

    private void SetPrimaryWeaponAimIk()
    {
        if (DebugLogger.IsNullError(enemySetupSo.primaryEnemyWeaponSetupSo, this, "Must be set in editor.")) return;

        simpleAi.SetSimpleAiData(enemySetupSo.primaryEnemyWeaponSetupSo.GetSimpleAiData());
        if (_primaryLeft) enemySetupSo.primaryEnemyWeaponSetupSo.SetupAimIk(weaponAimIkLeft, _primaryLeft, true);
        if (_primaryRight) enemySetupSo.primaryEnemyWeaponSetupSo.SetupAimIk(weaponAimIkRight, _primaryRight, false);
    }
    
    private void SetSecondaryWeaponAimIk()
    {
        if (DebugLogger.IsNullError(enemySetupSo.secondaryEnemyWeaponSetupSo, this, "Must be set in editor.")) return;

        simpleAi.SetSimpleAiData(enemySetupSo.secondaryEnemyWeaponSetupSo.GetSimpleAiData());
        if (_secondaryLeft) enemySetupSo.secondaryEnemyWeaponSetupSo.SetupAimIk(weaponAimIkLeft, _secondaryLeft, true);
        if (_secondaryRight) enemySetupSo.secondaryEnemyWeaponSetupSo.SetupAimIk(weaponAimIkRight, _secondaryRight, false);
    }

    private void CheckForBowSetup(EnemyWeaponInstance enemyWeaponInstance)
    {
        if (!enemyWeaponInstance.IsBow) return;

        var enemyBowInstance = enemyWeaponInstance.GetComponent<EnemyBowInstance>();
        if (DebugLogger.IsNullError(enemyBowInstance, this)) return;
        
        enemyBowInstance.AddKnockArrowToDropHandler(dropGearHandler);
    }

    public void ResetObject()
    {
        //Clear();
    }
    
    public void Clear()
    {
        if (DebugLogger.IsNullError(dropGearHandler, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(animationEventHandler, this, "Must be set in editor.")) return;

        dropGearHandler.Clear();
        animationEventHandler.Clear();
        
        if (_primaryLeft) _primaryLeft.ThisGameObject.SetActive(false);
        if (_primaryRight) _primaryRight.ThisGameObject.SetActive(false);
        if (_secondaryLeft) _secondaryLeft.ThisGameObject.SetActive(false);
        if (_secondaryRight) _secondaryRight.ThisGameObject.SetActive(false);
    }
}
