using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Portal : PooledObject
{
    [SerializeField] private Animator animator;
    [SerializeField] private float timeLeftOpen = 1;
    [SerializeField] private string openParameterName = "isOpen";

    private int _openId;
    private Transform _mainCameraTransform;


    protected override void Awake()
    {
        base.Awake();

        _openId = Animator.StringToHash(openParameterName);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        FaceCamera();

        if (ThisGameObject.activeSelf) StartCoroutine(AnimationCoroutine());
    }

    public override void PopulateParameters()
    {
        base.PopulateParameters();

        if (!animator)
        {
            animator = GetComponent<Animator>();
            if (!animator) animator = ThisGameObject.AddComponent<Animator>();
        }
    }

    protected override void Initialize()
    {
        base.Initialize();
        
        var mainCamera = Camera.main;
        if (!mainCamera) return;
        
        _mainCameraTransform = mainCamera.transform;
    }

    private void FaceCamera()
    {
        ThisTransform.LookAt(_mainCameraTransform);
        
        var eulerAngles = ThisTransform.eulerAngles;
        ThisTransform.eulerAngles = new Vector3(0, eulerAngles.y, 0);
    }

    private IEnumerator AnimationCoroutine()
    {
        animator.SetBool(_openId, true);

        yield return new WaitForSeconds(timeLeftOpen);
        
        animator.SetBool(_openId, false);
    }
}
