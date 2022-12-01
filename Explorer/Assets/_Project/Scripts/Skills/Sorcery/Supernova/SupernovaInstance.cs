using System.Collections;
using UnityEngine;

public class SupernovaInstance : SkillInstance
{
    [SerializeField] private GameObject damageCollider;
    [SerializeField] private GameObject endFx;
    
    private float _damageLifetime = .5f;
    

    protected override void OnEnable()
    {
        base.OnEnable();
        
        CoroutineCaller.Instance.StartCoroutine(DamageCoroutine());
    }

    protected override void ResetObject()
    {
        base.ResetObject();
        
        damageCollider.SetActive(false);
        endFx.SetActive(false);
    }

    public void SetDamageLifetime(float value)
    {
        _damageLifetime = value;
    }


    private IEnumerator DamageCoroutine() 
    {
        if (DebugLogger.IsNullError(damageCollider, this, "Must be set in editor.")) yield break;

        yield return null;
        
        damageCollider.SetActive(true);

        yield return new WaitForSeconds(_damageLifetime);

        damageCollider.SetActive(false);
        endFx.SetActive(true);
    }
}