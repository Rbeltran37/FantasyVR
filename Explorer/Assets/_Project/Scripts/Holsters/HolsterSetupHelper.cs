using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

public class HolsterSetupHelper : MonoBehaviour
{
    [SerializeField] private HolsterManager holsterManager;
    [SerializeField] private HolsterCollider leftWaistHolsterCollider;
    [SerializeField] private HolsterCollider rightWaistHolsterCollider;
    [SerializeField] private HolsterCollider leftBackHolsterCollider;
    [SerializeField] private HolsterCollider rightBackHolsterCollider;
    [SerializeField] private PlayerHand leftPlayerHand;
    [SerializeField] private PlayerHand rightPlayerHand;
    [SerializeField] private ManaPool manaPool;
    
    private Dictionary<HolsterCollider, Action<SkillContainer>> _holsterDictionary = new Dictionary<HolsterCollider, Action<SkillContainer>>();
    
    
    private void Awake()
    {
        _holsterDictionary.Add(leftWaistHolsterCollider, SetupLeftWaistHolster);
        _holsterDictionary.Add(rightWaistHolsterCollider, SetupRightWaistHolster);
        _holsterDictionary.Add(leftBackHolsterCollider, SetupLeftBackHolster);
        _holsterDictionary.Add(rightBackHolsterCollider, SetupRightBackHolster);

        StartCoroutine(InitializeHolstersCoroutine());
    }

    private IEnumerator InitializeHolstersCoroutine()
    {
        if (DebugLogger.IsNullError(holsterManager, this)) yield break;

        SetupLeftWaistHolster(holsterManager.leftWaistSkill);
        yield return new WaitForEndOfFrame();
        
        SetupRightWaistHolster(holsterManager.rightWaistSkill);
        yield return new WaitForEndOfFrame();
        
        SetupLeftBackHolster(holsterManager.leftBackSkill);
        yield return new WaitForEndOfFrame();
        
        SetupRightBackHolster(holsterManager.rightBackSkill);
    }

    [Button]
    public void SetupLeftWaistHolster(SkillContainer skillContainer)
    {
        SetupHolster(skillContainer, leftWaistHolsterCollider);

        holsterManager.leftWaistSkill = skillContainer;
    }

    [Button]
    public void SetupRightWaistHolster(SkillContainer skillContainer)
    {
        SetupHolster(skillContainer, rightWaistHolsterCollider);

        holsterManager.rightWaistSkill = skillContainer;
    }

    [Button]
    public void SetupLeftBackHolster(SkillContainer skillContainer)
    {
        SetupHolster(skillContainer, leftBackHolsterCollider);

        holsterManager.leftBackSkill = skillContainer;
    }

    [Button]
    public void SetupRightBackHolster(SkillContainer skillContainer)
    {
        SetupHolster(skillContainer, rightBackHolsterCollider);

        holsterManager.rightBackSkill = skillContainer;
    }
    
    private void SetupHolster(SkillContainer skillContainer, HolsterCollider holsterCollider)
    {
        if (DebugLogger.IsNullError(skillContainer, this)) return;
        if (DebugLogger.IsNullError(holsterCollider, this)) return;

        var leftHandSkill = skillContainer.skillPrefab;
        var rightHandSkill = skillContainer.skillPrefab;
        if (!leftHandSkill || !rightHandSkill) // Empty or null skillContainer
        {
            holsterCollider.gameObject.SetActive(false);
            return;
        }

        holsterCollider.gameObject.SetActive(true);

        var leftSkill = leftHandSkill.GetComponent<Skill>();
        var rightSkill = rightHandSkill.GetComponent<Skill>();
        if (DebugLogger.IsNullError(leftSkill, this) || DebugLogger.IsNullError(rightSkill, this))
        {
            holsterCollider.gameObject.SetActive(false);
            return;
        }

        holsterCollider.SetManaPool(manaPool);
        holsterCollider.SetupNewSkill(leftSkill, rightSkill, leftPlayerHand, rightPlayerHand);
    }

    public void SetupHolster(HolsterCollider holsterCollider, SkillContainer skillContainer)
    {
        if (DebugLogger.IsNullOrEmptyError(_holsterDictionary, this)) return;
        if (!_holsterDictionary.ContainsKey(holsterCollider))
        {
            DebugLogger.Error(MethodBase.GetCurrentMethod().Name, $"{nameof(_holsterDictionary)} does not contain key={holsterCollider}.", this);
            return;
        }
        
        _holsterDictionary[holsterCollider].Invoke(skillContainer);
    }
}
