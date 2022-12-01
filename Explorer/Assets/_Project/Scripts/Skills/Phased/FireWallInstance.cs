using System.Collections;
using UnityEngine;

public class FireWallInstance : SkillInstance
{
    [SerializeField] private SimpleAudioEvent loopingAudioEvent;
    [SerializeField] private float loopInterval = .75f;

    private GameObject _thisGameObject;
    
    protected override void OnEnable()
    {
        base.OnEnable();

        CoroutineCaller.Instance.StartCoroutine(LoopingAudioEventCoroutine());
    }
    
    private IEnumerator LoopingAudioEventCoroutine()
    {
        while (_thisGameObject.activeSelf)
        {
            loopingAudioEvent.Play(SkillAudioSource);
            yield return new WaitForSeconds(loopInterval);
        }
    }
    
    protected override void Initialize()
    {
        base.Initialize();
        
        _thisGameObject = gameObject;
        
        if (DebugLogger.IsNullError(SkillAudioSource, this, "Must be set in editor.")) return;
    }
}