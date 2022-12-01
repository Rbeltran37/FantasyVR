using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterData/HeroClass", order = 1)]
public class HeroClass : ScriptableObject
{
    public SkillContainerManager skillContainerManager;
    public string heroClassName;
}
