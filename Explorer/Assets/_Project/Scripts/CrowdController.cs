using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class CrowdController : MonoBehaviour
{
    public AudioSource[] ambientCrowdSources;
    public AudioSource[] reactionCrowdSources;
    
    public Transform xAxisAudioSourceOne;
    public Transform xAxisAudioSourceTwo;
    public Transform zAxisAudioSourceOne;
    public Transform zAxisAudioSourceTwo;
    
    [SerializeField] private List<AudioClip> positiveReactionCrowdSounds;
    [SerializeField] private List<AudioClip> negativeReactionCrowdSounds;
    [SerializeField] private PhotonView photonView;

    [Tooltip("Fade Buffer is a percentage of the audio clip where the volume fading begins. For example, " +
             "if the audio clip length is 60 seconds and the Fade Buffer is set to .10f (10% of the clip), " +
             "then the AudioSource will begin to fade out the volume at 54 seconds.")]
    [Range(0, 1)] public float fadeBuffer = .10f;
    [SerializeField] private Material crowdXAxisMaterial;
    [SerializeField] private Material crowdZAxisMaterial;
    
    private float _crowdFadeInSpeed = 3f;
    private Transform _player;
    private const float X_AXIS_MIN = -40f;
    private const float X_AXIS_MAX = 40f;
    private const float Z_AXIS_MIN = 0f;
    private const float Z_AXIS_MAX = 60f;
    private const float CROWD_AUDIO_PLAY_DELAY_MIN = 0.1f;
    private const float CROWD_AUDIO_PLAY_DELAY_MAX = 0.5f;
    private const float AUDIO_SOURCE_MOVEMENT_DELAY = .25F;
    private const string REACTION_JUMP_HEIGHT = "Crowd_Jump_Height";
    private const float REACTION_JUMP_HEIGHT_VALUE = 2f;
    private const float IDLE_JUMP_HEIGHT_VALUE = .5f;


    private void Start()
    {
        StartCoroutine(UpdateAudioPosition());
    }

    [Button]
    public void SynchronizePositiveCrowdAudio()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            var indexValue = Random.Range(0, positiveReactionCrowdSounds.Count);
            photonView.RPC("PlayPositiveCrowdReaction", RpcTarget.All, indexValue);
        }
    }
    
    public void SynchronizeNegativeCrowdAudio()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            var indexValue = Random.Range(0, negativeReactionCrowdSounds.Count);
            photonView.RPC("PlayNegativeCrowdReaction", RpcTarget.All, indexValue);
        }
    }

    [PunRPC]
    public void PlayPositiveCrowdReaction(int indexValue)
    {
        AudioClip positiveAudioClip = positiveReactionCrowdSounds[indexValue];
        float lengthOfClip = positiveAudioClip.length;
        StartCoroutine(StartCrowdJumping());
        
        foreach (var audioSource in reactionCrowdSources)
        {
            audioSource.clip = positiveAudioClip;
            StartCoroutine(WaitToPlaySound(audioSource));
            StartCoroutine(WaitToFadeAudioClip(lengthOfClip, audioSource));
        }
    }

    [PunRPC]
    public void PlayNegativeCrowdReaction(int indexValue)
    {
        AudioClip negativeAudioClip = negativeReactionCrowdSounds[indexValue];
        float lengthOfClip = negativeAudioClip.length;

        foreach (var audioSource in reactionCrowdSources)
        {
            audioSource.clip = negativeAudioClip;
            audioSource.Play();
            StartCoroutine(WaitToFadeAudioClip(lengthOfClip, audioSource));
        }
    }
    
    private IEnumerator WaitToPlaySound(AudioSource source)
    {
        yield return new WaitForSeconds(Random.Range(CROWD_AUDIO_PLAY_DELAY_MIN, CROWD_AUDIO_PLAY_DELAY_MAX));
        source.Play();
    }

    private IEnumerator AudioSourceDelay(AudioSource source, AudioClip clip)
    {
        yield return new WaitForSeconds(Random.Range(.1f, .5f));
        source.clip = clip;
        source.Play();
    }

    private IEnumerator WaitToFadeAudioClip(float clipLength, AudioSource audioToFade)
    {
        yield return new WaitForSeconds(clipLength - (clipLength * (fadeBuffer + Random.Range(.1f,.5f))));
        StartCoroutine(StopCrowdReaction());
        StartCoroutine(InitiateAudioFadeOut(audioToFade));
    }

    private IEnumerator InitiateAudioFadeOut(AudioSource reactionAudio)
    {
        float lengthOfClip = reactionAudio.clip.length;
        float totalFadeTime = lengthOfClip * fadeBuffer;

        float currentTime = totalFadeTime;
        while (currentTime > 0)
        {
            yield return null;
            currentTime -= Time.deltaTime;
            reactionAudio.volume = currentTime/totalFadeTime;
        }
        
        //Stop audio because we have faded out and reset volume for next clip. 
        reactionAudio.Stop();
        reactionAudio.volume = 1f;
    }

    private IEnumerator StartCrowdJumping()
    {
        float currentCrowdJumpHeight = crowdXAxisMaterial.GetFloat(REACTION_JUMP_HEIGHT);

        if (currentCrowdJumpHeight < REACTION_JUMP_HEIGHT_VALUE)
        {
            var t = 0f;
            while(t < 1)
            {
                t += Time.deltaTime / _crowdFadeInSpeed;
                crowdXAxisMaterial.SetFloat(REACTION_JUMP_HEIGHT, Mathf.Lerp(currentCrowdJumpHeight, REACTION_JUMP_HEIGHT_VALUE, t));
                crowdZAxisMaterial.SetFloat(REACTION_JUMP_HEIGHT, Mathf.Lerp(currentCrowdJumpHeight, REACTION_JUMP_HEIGHT_VALUE, t));
                yield return null;
            }
        }
    }

    private IEnumerator StopCrowdReaction()
    {
        float currentCrowdJumpHeight = crowdXAxisMaterial.GetFloat(REACTION_JUMP_HEIGHT);
        
        if (currentCrowdJumpHeight > IDLE_JUMP_HEIGHT_VALUE)
        {
            var t = 0f;
            while(t < 1)
            {
                t += Time.deltaTime / _crowdFadeInSpeed;
                crowdXAxisMaterial.SetFloat(REACTION_JUMP_HEIGHT, Mathf.Lerp(currentCrowdJumpHeight, IDLE_JUMP_HEIGHT_VALUE, t));
                crowdZAxisMaterial.SetFloat(REACTION_JUMP_HEIGHT, Mathf.Lerp(currentCrowdJumpHeight, IDLE_JUMP_HEIGHT_VALUE, t));
                yield return null;
            }
        }
    }
    
    public bool IsAudioPlaying()
    {
        foreach (var audioSource in reactionCrowdSources)
        {
            if (audioSource.isPlaying)
            {
                return true;
            }
        }
        return false;
    }

    public void SetPlayerPosition(Transform player)
    {
        this._player = player;
    }

    private IEnumerator UpdateAudioPosition()
    {
        while (true)
        {
            yield return new WaitForSeconds(AUDIO_SOURCE_MOVEMENT_DELAY);
            MoveAudioSource();
        }
    }

    public void MoveAudioSource()
    {
        if (!_player)
            return;
        
        var xAxis = _player.position.x;
        var zAxis = _player.position.z;
        xAxisAudioSourceOne.position = new Vector3(Mathf.Clamp(xAxis, X_AXIS_MIN, X_AXIS_MAX), xAxisAudioSourceOne.position.y, xAxisAudioSourceOne.position.z);
        xAxisAudioSourceTwo.position = new Vector3(Mathf.Clamp(xAxis, X_AXIS_MIN, X_AXIS_MAX), xAxisAudioSourceTwo.position.y, xAxisAudioSourceTwo.position.z);
        zAxisAudioSourceOne.position = new Vector3(zAxisAudioSourceOne.position.x, zAxisAudioSourceOne.position.y, Mathf.Clamp(zAxis, Z_AXIS_MIN, Z_AXIS_MAX));
        zAxisAudioSourceTwo.position = new Vector3(zAxisAudioSourceTwo.position.x, zAxisAudioSourceTwo.position.y, Mathf.Clamp(zAxis, Z_AXIS_MIN, Z_AXIS_MAX));
    }
    
}
