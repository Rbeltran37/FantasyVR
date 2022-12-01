using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Settings/DifficultyManager", order = 1)]
public class DifficultyManager : ScriptableObject
{
    [SerializeField] private CustomCharacterAnimationThirdPersonSO[] customCharacterAnimationThirdPersonDatas;
    [SerializeField] private HealthData[] healthDatas;
    [SerializeField] private String reactionTimeMin = "ReactionTimeMin";
    [SerializeField] private String reactionTimeMax = "ReactionTimeMax";
    [SerializeField] private String timeBetweenActionsMin = "TimeBetweenActionsMin";
    [SerializeField] private String timeBetweenActionsMax = "TimeBetweenActionsMax";
    
    [Button]
    public void SetDifficulty(DifficultySetting difficultySetting)
    {
        foreach (var customCharacterAnimationThirdPersonData in customCharacterAnimationThirdPersonDatas)
        {
            customCharacterAnimationThirdPersonData.animSpeedMultiplier = difficultySetting.animatorSpeed;
        }

        foreach (var healthData in healthDatas)
        {
            //TODO add multiplier to health, or change health data
            //healthData.healthMultiplier = difficultySetting.healthMultiplier;
        }

        GlobalVariables.Instance.GetVariable(reactionTimeMin)?.SetValue(difficultySetting.reactionTime.minValue);
        GlobalVariables.Instance.GetVariable(reactionTimeMax)?.SetValue(difficultySetting.reactionTime.maxValue);
        GlobalVariables.Instance.GetVariable(timeBetweenActionsMin)?.SetValue(difficultySetting.timeBetweenActions.minValue);
        GlobalVariables.Instance.GetVariable(timeBetweenActionsMax)?.SetValue(difficultySetting.timeBetweenActions.maxValue);
    }
}
