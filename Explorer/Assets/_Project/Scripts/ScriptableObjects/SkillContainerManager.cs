using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterData/SkillContainerManager", order = 1)]
public class SkillContainerManager : ScriptableObject
{
    public List<SkillContainer> skillContainers = new List<SkillContainer>();
    
    private Dictionary<SkillContainer, GameObject> _skillContainerDictionary = new Dictionary<SkillContainer, GameObject>();


    private void OnEnable()
    {
        SetupDictionary();
    }

    private void SetupDictionary()
    {
        _skillContainerDictionary = new Dictionary<SkillContainer, GameObject>();
        foreach (var container in skillContainers)
        {
            _skillContainerDictionary.Add(container, container.skillPrefab);
        }
    }

    public GameObject GetSkillCreatorPrefab(SkillContainer skill)
    {
        return _skillContainerDictionary[skill];
    }
}
