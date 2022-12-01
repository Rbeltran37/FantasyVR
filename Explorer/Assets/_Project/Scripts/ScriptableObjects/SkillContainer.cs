using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_SkillContainer", menuName = "ScriptableObjects/CharacterData/SkillContainer", order = 1)]
public class SkillContainer : ScriptableObject
{
    public string skillName;
    public GameObject skillPrefab;
    public SkillSO SkillSo;
}
