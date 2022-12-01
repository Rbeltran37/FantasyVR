using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName="ScriptableObjects/Audio/HolsterAudioManager")]
public class HolsterAudioManager : ScriptableObject
{
    [SerializeField] private AudioClip[] enterHolsterClips;
    [SerializeField] private RangedFloat enterHolsterVolume;
    [MinMaxFloatRange(0, 3)]
    [SerializeField] private RangedFloat enterHolsterPitch;
    
    [SerializeField] private AudioClip[] equipSkillClips;
    [SerializeField] private RangedFloat equipSkillVolume;
    [MinMaxFloatRange(0, 3)]
    [SerializeField] private RangedFloat equipSkillPitch;
    
    [SerializeField] private AudioClip[] invalidEquipClips;
    [SerializeField] private RangedFloat invalidEquipVolume;
    [MinMaxFloatRange(0, 3)]
    [SerializeField] private RangedFloat invalidEquipPitch;

    public void PlayEnterHolsterSound(AudioSource audioSource)
    {
        if (enterHolsterClips.Length == 0) return;

        audioSource.clip = enterHolsterClips[Random.Range(0, enterHolsterClips.Length)];
        audioSource.volume = Random.Range(enterHolsterVolume.minValue, enterHolsterVolume.maxValue);
        audioSource.pitch = Random.Range(enterHolsterPitch.minValue, enterHolsterPitch.maxValue);
        audioSource.Play();
    }
    
    public void PlayEquipSkillSound(AudioSource audioSource)
    {
        if (equipSkillClips.Length == 0) return;

        audioSource.clip = equipSkillClips[Random.Range(0, equipSkillClips.Length)];
        audioSource.volume = Random.Range(equipSkillVolume.minValue, equipSkillVolume.maxValue);
        audioSource.pitch = Random.Range(equipSkillPitch.minValue, equipSkillPitch.maxValue);
        audioSource.Play();
    }
    
    public void PlayInvalidEquipSound(AudioSource audioSource)
    {
        if (invalidEquipClips.Length == 0) return;

        audioSource.clip = invalidEquipClips[Random.Range(0, invalidEquipClips.Length)];
        audioSource.volume = Random.Range(invalidEquipVolume.minValue, invalidEquipVolume.maxValue);
        audioSource.pitch = Random.Range(invalidEquipPitch.minValue, invalidEquipPitch.maxValue);
        audioSource.Play();
    }
}