using System;
using UnityEngine;

public class MortarProjectile : Projectile
{
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private AudioSource flybySound;
    [SerializeField] private LocalizedSkillAbility localizedSkillAbility;
    [SerializeField] private float indicatorSizeModifier;
    private float _time;
    private bool _hasLanded = false;
    private bool _hasPlayedSound = false; 
    private float _timeOfLaunch;
    private float _timeUntilHit;
    private Transform _indicator;
    public float speed = 1.0f;
    [SerializeField] private LayerMask layersForCeilingCheck;

    
    protected override void Awake()
    {
        base.Awake();
        
        //This is the radius of explosion
        localizedSkillAbility.SetLevel(1);
        localizedSkillAbility.SetValue(1);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _hasLanded = false;
    }

    private void Update()
    {
        if (!_hasLanded)
        {
            
            
            var value = Vector3.Distance(transform.position, _indicator.position) / indicatorSizeModifier;
            _indicator.transform.localScale = new Vector3(value, value, value);
            _time += Time.deltaTime;
            
            var rotatedUpVector = Quaternion.AngleAxis(speed, Vector3.up) * transform.up;
            if (rigidbody.velocity != Vector3.zero)
            {
                rigidbody.MoveRotation(Quaternion.LookRotation(rigidbody.velocity, rotatedUpVector));
            }

            if ((_timeUntilHit - (_time - _timeOfLaunch)) <= GetAudioClipLength() && !_hasPlayedSound)
            {
                flybySound.Play();
                _hasPlayedSound = true;
            }
        }
    }

    protected override void OnCollisionEnter(Collision other)
    {
        base.OnCollisionEnter(other);

        ResetProjectile();
        DisableIndicator();
    }

    public Rigidbody GetRigidbody()
    {
        return rigidbody;
    }

    private float GetAudioClipLength()
    {
        return flybySound.clip.length;
    }

    private void DisableIndicator()
    {
        _indicator.gameObject.SetActive(false);
    }

    private void ResetProjectile()
    {
        _hasLanded = true;
        _hasPlayedSound = false;
        _time = 0f;
        _indicator.localScale = Vector3.zero;
    }

    public void SetIndicator(Transform indicator)
    {
        this._indicator = indicator;
    }
    
    public void SetTimeUntilHit(float timeUntilHit)
    {
        this._timeUntilHit = timeUntilHit;
    }

    public void SetTimeOfLaunch(float timeOfLaunch)
    {
        this._timeOfLaunch = timeOfLaunch;
    }
    
    private void CheckCeiling()
    {
        RaycastHit hit;
        
        if(Physics.Raycast(transform.position, _indicator.position, out hit, 10, layersForCeilingCheck))
        {
            //DebugLogger.Info(hit.transform.name);
            //DebugLogger.Info("Houston we have a problem...");
        }
    }
}
